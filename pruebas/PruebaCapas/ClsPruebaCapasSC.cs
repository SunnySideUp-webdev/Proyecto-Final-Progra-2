using System;
using System.Data;
using System.Data.SqlClient;
using CapaDatos;
using CapaEntidades;
using CapaLogica;

namespace PruebaCapas
{
    /// <summary>
    /// Comprueba en tiempo de ejecución que las capas se comunican con la base
    /// compartida y que las reglas de esta etapa funcionan. No deja datos guardados.
    /// </summary>
    internal static class ClsPruebaCapasSC
    {
        private static int gIntFallosSC;

        private static int Main()
        {
            Console.WriteLine("== Regla del año (Capa Lógica) ==");
            var lDatReferenciaSC = new DateTime(2026, 10, 19);
            SubVerificarSC("1980 se rechaza (antes del VIN de 17 caracteres)", !ClsVehiculoLNSC.FncEsAnioValidoSC(1980, lDatReferenciaSC));
            SubVerificarSC("1981 se acepta", ClsVehiculoLNSC.FncEsAnioValidoSC(1981, lDatReferenciaSC));
            SubVerificarSC("2027 se acepta con fecha 2026-10-19", ClsVehiculoLNSC.FncEsAnioValidoSC(2027, lDatReferenciaSC));
            SubVerificarSC("2028 se rechaza con fecha 2026-10-19", !ClsVehiculoLNSC.FncEsAnioValidoSC(2028, lDatReferenciaSC));
            SubVerificarSC("2028 se acepta con fecha 2027-01-01 (regla dinámica)",
                ClsVehiculoLNSC.FncEsAnioValidoSC(2028, new DateTime(2027, 1, 1)));
            int lIntAnioSiguienteSC = ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC().Year + 1;
            SubVerificarSC($"Hoy, el máximo es {ClsVehiculoLNSC.FncObtenerAnioMaximoSC()}",
                ClsVehiculoLNSC.FncObtenerAnioMaximoSC() == lIntAnioSiguienteSC);

            Console.WriteLine();
            Console.WriteLine("== Zona horaria (UTC en la base, Guatemala al mostrar) ==");
            TimeZoneInfo lObjZonaSC = ClsZonaHorariaLNSC.gObjZonaSC;
            SubVerificarSC($"Zona encontrada: {lObjZonaSC.Id}, UTC{lObjZonaSC.BaseUtcOffset.TotalHours:+0;-0}",
                lObjZonaSC.BaseUtcOffset == TimeSpan.FromHours(-6));
            SubVerificarSC("Guatemala no aplica horario de verano (UTC-6 en enero y en julio)",
                lObjZonaSC.GetUtcOffset(new DateTime(2026, 1, 15)) == TimeSpan.FromHours(-6)
                && lObjZonaSC.GetUtcOffset(new DateTime(2026, 7, 15)) == TimeSpan.FromHours(-6));
            DateTime lDatUtcSC = new DateTime(2026, 10, 20, 2, 53, 15, DateTimeKind.Utc);
            SubVerificarSC("20/10/2026 02:53 UTC -> 19/10/2026 20:53 en Guatemala",
                ClsZonaHorariaLNSC.FncConvertirUtcAGuatemalaSC(lDatUtcSC) == new DateTime(2026, 10, 19, 20, 53, 15));
            SubVerificarSC("Fecha sin zona (como llega de SQL Server) se trata como UTC",
                ClsZonaHorariaLNSC.FncConvertirUtcAGuatemalaSC(DateTime.SpecifyKind(lDatUtcSC, DateTimeKind.Unspecified))
                    == new DateTime(2026, 10, 19, 20, 53, 15));
            SubVerificarSC("Inicio de noviembre en Guatemala = 01/11/2026 06:00 UTC",
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(new DateTime(2026, 11, 1)) == new DateTime(2026, 11, 1, 6, 0, 0));
            bool lBlnRechazaLocalSC = false;
            try
            {
                ClsZonaHorariaLNSC.FncConvertirUtcAGuatemalaSC(new DateTime(2026, 10, 19, 20, 53, 0, DateTimeKind.Local));
            }
            catch (ArgumentException)
            {
                lBlnRechazaLocalSC = true;
            }
            SubVerificarSC("Rechaza convertir una hora local (evita la doble conversión)", lBlnRechazaLocalSC);
            SubVerificarSC("'Hoy' en Guatemala coincide con UTC-6",
                ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC() == DateTime.UtcNow.AddHours(-6).Date);

            Console.WriteLine();
            Console.WriteLine("== Conexión mediante ClsConexionDASC (Capa de Datos) ==");
            try
            {
                using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
                {
                    lObjConexionSC.Open();
                    SubVerificarSC("Base conectada: AutoManagerDMS", FncEscalarSC(lObjConexionSC, "SELECT DB_NAME();") == "AutoManagerDMS");
                    SubVerificarSC("Usuario: taller_app", FncEscalarSC(lObjConexionSC, "SELECT SUSER_SNAME();") == "taller_app");
                    SubVerificarSC("Existe dbo.Clientes", FncEscalarSC(lObjConexionSC, "SELECT OBJECT_ID(N'dbo.Clientes', N'U');") != "");
                    SubVerificarSC("Existe dbo.Vehiculos", FncEscalarSC(lObjConexionSC, "SELECT OBJECT_ID(N'dbo.Vehiculos', N'U');") != "");

                    Console.WriteLine();
                    Console.WriteLine("== Permisos limitados de taller_app ==");
                    SubVerificarSC("Puede leer (db_datareader)", FncEscalarSC(lObjConexionSC, "SELECT IS_ROLEMEMBER(N'db_datareader');") == "1");
                    SubVerificarSC("Puede escribir (db_datawriter)", FncEscalarSC(lObjConexionSC, "SELECT IS_ROLEMEMBER(N'db_datawriter');") == "1");
                    SubVerificarSC("No es db_owner", FncEscalarSC(lObjConexionSC, "SELECT IS_ROLEMEMBER(N'db_owner');") == "0");
                    SubProbarEscrituraYCreacionSC(lObjConexionSC);
                }
            }
            catch (Exception lObjExcepcionSC)
            {
                SubVerificarSC($"Conexión ({lObjExcepcionSC.GetType().Name}: {lObjExcepcionSC.Message})", false);
            }

            Console.WriteLine();
            Console.WriteLine("== Reglas de vehículos (Capa Lógica; no llegan a insertar) ==");
            var lObjVehiculoLNSC = new ClsVehiculoLNSC();
            SubVerificarValidacionSC("Propietario inexistente (id 999999) se rechaza", () => lObjVehiculoLNSC.FncRegistrarVehiculoSC(
                new ClsVehiculoENSC { gIntIdClienteSC = 999999, gStrPlacaSC = "Z-999 ZZZ", gStrMarcaSC = "Kia", gStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.gStrCampoPropietarioSC, "ya no existe");
            SubVerificarValidacionSC("Año con letras: pide 4 dígitos", () => lObjVehiculoLNSC.FncRegistrarVehiculoSC(
                new ClsVehiculoENSC { gIntIdClienteSC = 1, gStrPlacaSC = "Z999ZZZ", gStrMarcaSC = "Kia", gStrModeloSC = "Rio" }, "20a0"),
                ClsVehiculoLNSC.gStrCampoAnioSC, "4 dígitos");
            SubVerificarValidacionSC($"Año {ClsVehiculoLNSC.FncObtenerAnioMaximoSC() + 1}: fuera del rango dinámico", () => lObjVehiculoLNSC.FncRegistrarVehiculoSC(
                new ClsVehiculoENSC { gIntIdClienteSC = 1, gStrPlacaSC = "Z999ZZZ", gStrMarcaSC = "Kia", gStrModeloSC = "Rio" },
                (ClsVehiculoLNSC.FncObtenerAnioMaximoSC() + 1).ToString()),
                ClsVehiculoLNSC.gStrCampoAnioSC, "debe estar entre 1981 y " + ClsVehiculoLNSC.FncObtenerAnioMaximoSC());
            SubVerificarValidacionSC("VIN vacío: es obligatorio", () => lObjVehiculoLNSC.FncRegistrarVehiculoSC(
                new ClsVehiculoENSC { gIntIdClienteSC = 1, gStrPlacaSC = "Z999ZZZ", gStrVinSC = "  ", gStrMarcaSC = "Kia", gStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.gStrCampoVinSC, "El VIN es obligatorio");
            SubVerificarValidacionSC("VIN con la letra O se rechaza", () => lObjVehiculoLNSC.FncRegistrarVehiculoSC(
                new ClsVehiculoENSC { gIntIdClienteSC = 1, gStrPlacaSC = "Z999ZZZ", gStrVinSC = "1HGCM82633A00435O", gStrMarcaSC = "Kia", gStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.gStrCampoVinSC, "sin I, O ni Q");

            Console.WriteLine();
            Console.WriteLine("== Vehículos por cliente (Capa Lógica -> Capa de Datos) ==");
            try
            {
                var lLstVehiculosAnaSC = lObjVehiculoLNSC.FncListarVehiculosPorClienteSC(1);
                SubVerificarSC($"Cliente 1 tiene {lLstVehiculosAnaSC.Count} vehículos y todos le pertenecen",
                    lLstVehiculosAnaSC.Count >= 2 && lLstVehiculosAnaSC.TrueForAll(pObjVehiculoSC => pObjVehiculoSC.gIntIdClienteSC == 1 && pObjVehiculoSC.gObjPropietarioSC.gIntIdClienteSC == 1));
                SubVerificarSC("Fechas leídas marcadas como UTC (evita conversiones dobles)",
                    lLstVehiculosAnaSC.TrueForAll(pObjVehiculoSC => pObjVehiculoSC.gDatFechaRegistroUtcSC.Kind == DateTimeKind.Utc));
                SubVerificarSC("Cliente inexistente (999999) -> lista vacía", lObjVehiculoLNSC.FncListarVehiculosPorClienteSC(999999).Count == 0);
                SubVerificarSC("Identificadores 0 y -1 -> lista vacía sin consultar la base",
                    lObjVehiculoLNSC.FncListarVehiculosPorClienteSC(0).Count == 0 && lObjVehiculoLNSC.FncListarVehiculosPorClienteSC(-1).Count == 0);
            }
            catch (Exception lObjExcepcionSC)
            {
                SubVerificarSC($"Vehículos por cliente ({lObjExcepcionSC.GetType().Name}: {lObjExcepcionSC.Message})", false);
            }

            Console.WriteLine();
            var lObjClienteSC = new ClsClienteENSC();
            var lObjVehiculoSC = new ClsVehiculoENSC();
            SubVerificarSC("Entidades creadas desde CapaEntidades",
                lObjClienteSC.gStrDpiSC == string.Empty && lObjVehiculoSC.gStrPlacaSC == string.Empty);

            Console.WriteLine();
            Console.WriteLine(gIntFallosSC == 0 ? "RESULTADO: todas las pruebas pasaron." : $"RESULTADO: {gIntFallosSC} prueba(s) fallaron.");
            return gIntFallosSC == 0 ? 0 : 1;
        }

        /// <summary>Inserta un cliente e intenta crear una tabla; deshace todo al final.</summary>
        private static void SubProbarEscrituraYCreacionSC(SqlConnection pObjConexionSC)
        {
            using (SqlTransaction lObjTransaccionSC = pObjConexionSC.BeginTransaction())
            {
                using (var lObjComandoSC = new SqlCommand(
                    "INSERT INTO dbo.Clientes (DPI, Nombres, Apellidos, Telefono) VALUES (@Dpi, @Nombres, @Apellidos, @Telefono);",
                    pObjConexionSC, lObjTransaccionSC))
                {
                    lObjComandoSC.Parameters.Add("@Dpi", SqlDbType.Char, 13).Value = "9999999999999";
                    lObjComandoSC.Parameters.Add("@Nombres", SqlDbType.NVarChar, 80).Value = "Prueba";
                    lObjComandoSC.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 80).Value = "Permisos";
                    lObjComandoSC.Parameters.Add("@Telefono", SqlDbType.VarChar, 15).Value = "55550000";
                    SubVerificarSC("Puede insertar un cliente (se deshace)", lObjComandoSC.ExecuteNonQuery() == 1);
                }

                try
                {
                    using (var lObjComandoSC = new SqlCommand("CREATE TABLE dbo.TablaNoPermitida (Id INT);", pObjConexionSC, lObjTransaccionSC))
                    {
                        lObjComandoSC.ExecuteNonQuery();
                    }
                    SubVerificarSC("No puede crear tablas", false);
                }
                catch (SqlException lObjExcepcionSC)
                {
                    SubVerificarSC($"No puede crear tablas (SQL Server respondió error {lObjExcepcionSC.Number})", lObjExcepcionSC.Number == 262);
                }

                lObjTransaccionSC.Rollback();
            }
        }

        private static string FncEscalarSC(SqlConnection pObjConexionSC, string pStrConsultaSC)
        {
            using (var lObjComandoSC = new SqlCommand(pStrConsultaSC, pObjConexionSC))
            {
                object lObjValorSC = lObjComandoSC.ExecuteScalar();
                return lObjValorSC == null || lObjValorSC == DBNull.Value ? "" : Convert.ToString(lObjValorSC);
            }
        }

        /// <summary>Comprueba que una operación falle con un error de validación en el campo indicado.</summary>
        private static void SubVerificarValidacionSC(string pStrDescripcionSC, Action pObjAccionSC, string pStrCampoSC, string pStrTextoEsperadoSC)
        {
            try
            {
                pObjAccionSC();
                SubVerificarSC(pStrDescripcionSC + " (no se rechazó)", false);
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                string lStrMensajeSC;
                bool lBlnCorrectoSC = lObjExcepcionSC.gDicErroresCamposSC.TryGetValue(pStrCampoSC, out lStrMensajeSC)
                    && lStrMensajeSC.Contains(pStrTextoEsperadoSC);
                SubVerificarSC($"{pStrDescripcionSC} -> «{lStrMensajeSC}»", lBlnCorrectoSC);
            }
            catch (Exception lObjExcepcionSC)
            {
                SubVerificarSC($"{pStrDescripcionSC} ({lObjExcepcionSC.GetType().Name}: {lObjExcepcionSC.Message})", false);
            }
        }

        private static void SubVerificarSC(string pStrDescripcionSC, bool pBlnCorrectoSC)
        {
            if (!pBlnCorrectoSC)
            {
                gIntFallosSC++;
            }
            Console.WriteLine($"  {(pBlnCorrectoSC ? "OK   " : "FALLO")} {pStrDescripcionSC}");
        }
    }
}
