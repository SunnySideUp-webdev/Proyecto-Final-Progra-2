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
    internal static class Program
    {
        private static int lIntFallos;

        private static int Main()
        {
            Console.WriteLine("== Regla del año (Capa Lógica) ==");
            var lDatReferencia = new DateTime(2026, 10, 19);
            Verificar("1980 se rechaza (antes del VIN de 17 caracteres)", !ClsVehiculoLNSC.fncEsAnioValidoSC(1980, lDatReferencia));
            Verificar("1981 se acepta", ClsVehiculoLNSC.fncEsAnioValidoSC(1981, lDatReferencia));
            Verificar("2027 se acepta con fecha 2026-10-19", ClsVehiculoLNSC.fncEsAnioValidoSC(2027, lDatReferencia));
            Verificar("2028 se rechaza con fecha 2026-10-19", !ClsVehiculoLNSC.fncEsAnioValidoSC(2028, lDatReferencia));
            Verificar("2028 se acepta con fecha 2027-01-01 (regla dinámica)",
                ClsVehiculoLNSC.fncEsAnioValidoSC(2028, new DateTime(2027, 1, 1)));
            int lIntAnioSiguiente = ClsZonaHorariaLN.fncObtenerHoyGuatemala().Year + 1;
            Verificar($"Hoy, el máximo es {ClsVehiculoLNSC.fncObtenerAnioMaximoSC()}",
                ClsVehiculoLNSC.fncObtenerAnioMaximoSC() == lIntAnioSiguiente);

            Console.WriteLine();
            Console.WriteLine("== Zona horaria (UTC en la base, Guatemala al mostrar) ==");
            TimeZoneInfo lObjZona = ClsZonaHorariaLN.lObjZona;
            Verificar($"Zona encontrada: {lObjZona.Id}, UTC{lObjZona.BaseUtcOffset.TotalHours:+0;-0}",
                lObjZona.BaseUtcOffset == TimeSpan.FromHours(-6));
            Verificar("Guatemala no aplica horario de verano (UTC-6 en enero y en julio)",
                lObjZona.GetUtcOffset(new DateTime(2026, 1, 15)) == TimeSpan.FromHours(-6)
                && lObjZona.GetUtcOffset(new DateTime(2026, 7, 15)) == TimeSpan.FromHours(-6));
            DateTime lDatUtc = new DateTime(2026, 10, 20, 2, 53, 15, DateTimeKind.Utc);
            Verificar("20/10/2026 02:53 UTC -> 19/10/2026 20:53 en Guatemala",
                ClsZonaHorariaLN.fncConvertirUtcAGuatemala(lDatUtc) == new DateTime(2026, 10, 19, 20, 53, 15));
            Verificar("Fecha sin zona (como llega de SQL Server) se trata como UTC",
                ClsZonaHorariaLN.fncConvertirUtcAGuatemala(DateTime.SpecifyKind(lDatUtc, DateTimeKind.Unspecified))
                    == new DateTime(2026, 10, 19, 20, 53, 15));
            Verificar("Inicio de noviembre en Guatemala = 01/11/2026 06:00 UTC",
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(new DateTime(2026, 11, 1)) == new DateTime(2026, 11, 1, 6, 0, 0));
            bool lBolRechazaLocal = false;
            try
            {
                ClsZonaHorariaLN.fncConvertirUtcAGuatemala(new DateTime(2026, 10, 19, 20, 53, 0, DateTimeKind.Local));
            }
            catch (ArgumentException)
            {
                lBolRechazaLocal = true;
            }
            Verificar("Rechaza convertir una hora local (evita la doble conversión)", lBolRechazaLocal);
            Verificar("'Hoy' en Guatemala coincide con UTC-6",
                ClsZonaHorariaLN.fncObtenerHoyGuatemala() == DateTime.UtcNow.AddHours(-6).Date);

            Console.WriteLine();
            Console.WriteLine("== Conexión mediante ClsConexionDA (Capa de Datos) ==");
            try
            {
                using (SqlConnection lObjConexion = ClsConexionDA.fncObtenerConexion())
                {
                    lObjConexion.Open();
                    Verificar("Base conectada: AutoManagerDMS", Escalar(lObjConexion, "SELECT DB_NAME();") == "AutoManagerDMS");
                    Verificar("Usuario: taller_app", Escalar(lObjConexion, "SELECT SUSER_SNAME();") == "taller_app");
                    Verificar("Existe dbo.Clientes", Escalar(lObjConexion, "SELECT OBJECT_ID(N'dbo.Clientes', N'U');") != "");
                    Verificar("Existe dbo.Vehiculos", Escalar(lObjConexion, "SELECT OBJECT_ID(N'dbo.Vehiculos', N'U');") != "");

                    Console.WriteLine();
                    Console.WriteLine("== Permisos limitados de taller_app ==");
                    Verificar("Puede leer (db_datareader)", Escalar(lObjConexion, "SELECT IS_ROLEMEMBER(N'db_datareader');") == "1");
                    Verificar("Puede escribir (db_datawriter)", Escalar(lObjConexion, "SELECT IS_ROLEMEMBER(N'db_datawriter');") == "1");
                    Verificar("No es db_owner", Escalar(lObjConexion, "SELECT IS_ROLEMEMBER(N'db_owner');") == "0");
                    ProbarEscrituraYCreacion(lObjConexion);
                }
            }
            catch (Exception ex)
            {
                Verificar($"Conexión ({ex.GetType().Name}: {ex.Message})", false);
            }

            Console.WriteLine();
            Console.WriteLine("== Reglas de vehículos (Capa Lógica; no llegan a insertar) ==");
            var lObjVehiculoLN = new ClsVehiculoLNSC();
            VerificarValidacion("Propietario inexistente (id 999999) se rechaza", () => lObjVehiculoLN.fncRegistrarVehiculoSC(
                new ClsVehiculoENSC { lIntIdClienteSC = 999999, lStrPlacaSC = "Z-999 ZZZ", lStrMarcaSC = "Kia", lStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.lStrCampoPropietarioSC, "ya no existe");
            VerificarValidacion("Año con letras: pide 4 dígitos", () => lObjVehiculoLN.fncRegistrarVehiculoSC(
                new ClsVehiculoENSC { lIntIdClienteSC = 1, lStrPlacaSC = "Z999ZZZ", lStrMarcaSC = "Kia", lStrModeloSC = "Rio" }, "20a0"),
                ClsVehiculoLNSC.lStrCampoAnioSC, "4 dígitos");
            VerificarValidacion($"Año {ClsVehiculoLNSC.fncObtenerAnioMaximoSC() + 1}: fuera del rango dinámico", () => lObjVehiculoLN.fncRegistrarVehiculoSC(
                new ClsVehiculoENSC { lIntIdClienteSC = 1, lStrPlacaSC = "Z999ZZZ", lStrMarcaSC = "Kia", lStrModeloSC = "Rio" },
                (ClsVehiculoLNSC.fncObtenerAnioMaximoSC() + 1).ToString()),
                ClsVehiculoLNSC.lStrCampoAnioSC, "debe estar entre 1981 y " + ClsVehiculoLNSC.fncObtenerAnioMaximoSC());
            VerificarValidacion("VIN vacío: es obligatorio", () => lObjVehiculoLN.fncRegistrarVehiculoSC(
                new ClsVehiculoENSC { lIntIdClienteSC = 1, lStrPlacaSC = "Z999ZZZ", lStrVinSC = "  ", lStrMarcaSC = "Kia", lStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.lStrCampoVinSC, "El VIN es obligatorio");
            VerificarValidacion("VIN con la letra O se rechaza", () => lObjVehiculoLN.fncRegistrarVehiculoSC(
                new ClsVehiculoENSC { lIntIdClienteSC = 1, lStrPlacaSC = "Z999ZZZ", lStrVinSC = "1HGCM82633A00435O", lStrMarcaSC = "Kia", lStrModeloSC = "Rio" }, "2020"),
                ClsVehiculoLNSC.lStrCampoVinSC, "sin I, O ni Q");

            Console.WriteLine();
            Console.WriteLine("== Vehículos por cliente (Capa Lógica -> Capa de Datos) ==");
            try
            {
                var lLstVehiculosAna = lObjVehiculoLN.fncListarVehiculosPorClienteSC(1);
                Verificar($"Cliente 1 tiene {lLstVehiculosAna.Count} vehículos y todos le pertenecen",
                    lLstVehiculosAna.Count >= 2 && lLstVehiculosAna.TrueForAll(x => x.lIntIdClienteSC == 1 && x.lObjPropietarioSC.lIntIdClienteSC == 1));
                Verificar("Fechas leídas marcadas como UTC (evita conversiones dobles)",
                    lLstVehiculosAna.TrueForAll(x => x.lDatFechaRegistroUtcSC.Kind == DateTimeKind.Utc));
                Verificar("Cliente inexistente (999999) -> lista vacía", lObjVehiculoLN.fncListarVehiculosPorClienteSC(999999).Count == 0);
                Verificar("Identificadores 0 y -1 -> lista vacía sin consultar la base",
                    lObjVehiculoLN.fncListarVehiculosPorClienteSC(0).Count == 0 && lObjVehiculoLN.fncListarVehiculosPorClienteSC(-1).Count == 0);
            }
            catch (Exception ex)
            {
                Verificar($"Vehículos por cliente ({ex.GetType().Name}: {ex.Message})", false);
            }

            Console.WriteLine();
            var lObjCliente = new ClsClienteENSC();
            var lObjVehiculo = new ClsVehiculoENSC();
            Verificar("Entidades creadas desde CapaEntidades",
                lObjCliente.lStrDpiSC == string.Empty && lObjVehiculo.lStrPlacaSC == string.Empty);

            Console.WriteLine();
            Console.WriteLine(lIntFallos == 0 ? "RESULTADO: todas las pruebas pasaron." : $"RESULTADO: {lIntFallos} prueba(s) fallaron.");
            return lIntFallos == 0 ? 0 : 1;
        }

        /// <summary>Inserta un cliente e intenta crear una tabla; deshace todo al final.</summary>
        private static void ProbarEscrituraYCreacion(SqlConnection pObjConexion)
        {
            using (SqlTransaction lObjTransaccion = pObjConexion.BeginTransaction())
            {
                using (var lObjComando = new SqlCommand(
                    "INSERT INTO dbo.Clientes (DPI, Nombres, Apellidos, Telefono) VALUES (@Dpi, @Nombres, @Apellidos, @Telefono);",
                    pObjConexion, lObjTransaccion))
                {
                    lObjComando.Parameters.Add("@Dpi", SqlDbType.Char, 13).Value = "9999999999999";
                    lObjComando.Parameters.Add("@Nombres", SqlDbType.NVarChar, 80).Value = "Prueba";
                    lObjComando.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 80).Value = "Permisos";
                    lObjComando.Parameters.Add("@Telefono", SqlDbType.VarChar, 15).Value = "55550000";
                    Verificar("Puede insertar un cliente (se deshace)", lObjComando.ExecuteNonQuery() == 1);
                }

                try
                {
                    using (var lObjComando = new SqlCommand("CREATE TABLE dbo.TablaNoPermitida (Id INT);", pObjConexion, lObjTransaccion))
                    {
                        lObjComando.ExecuteNonQuery();
                    }
                    Verificar("No puede crear tablas", false);
                }
                catch (SqlException ex)
                {
                    Verificar($"No puede crear tablas (SQL Server respondió error {ex.Number})", ex.Number == 262);
                }

                lObjTransaccion.Rollback();
            }
        }

        private static string Escalar(SqlConnection pObjConexion, string pStrConsulta)
        {
            using (var lObjComando = new SqlCommand(pStrConsulta, pObjConexion))
            {
                object lObjValor = lObjComando.ExecuteScalar();
                return lObjValor == null || lObjValor == DBNull.Value ? "" : Convert.ToString(lObjValor);
            }
        }

        /// <summary>Comprueba que una operación falle con un error de validación en el campo indicado.</summary>
        private static void VerificarValidacion(string pStrDescripcion, Action pObjAccion, string pStrCampo, string pStrTextoEsperado)
        {
            try
            {
                pObjAccion();
                Verificar(pStrDescripcion + " (no se rechazó)", false);
            }
            catch (ClsValidacionExcepcionLN ex)
            {
                string lStrMensaje;
                bool lBolCorrecto = ex.lDicErroresCampos.TryGetValue(pStrCampo, out lStrMensaje)
                    && lStrMensaje.Contains(pStrTextoEsperado);
                Verificar($"{pStrDescripcion} -> «{lStrMensaje}»", lBolCorrecto);
            }
            catch (Exception ex)
            {
                Verificar($"{pStrDescripcion} ({ex.GetType().Name}: {ex.Message})", false);
            }
        }

        private static void Verificar(string pStrDescripcion, bool pBolCorrecto)
        {
            if (!pBolCorrecto)
            {
                lIntFallos++;
            }
            Console.WriteLine($"  {(pBolCorrecto ? "OK   " : "FALLO")} {pStrDescripcion}");
        }
    }
}
