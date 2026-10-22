//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Capa de Datos
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>Ejecuta las operaciones parametrizadas de la tabla dbo.Vehiculos.</summary>
    public class ClsVehiculoDASC
    {
        /// <summary>Consulta base: cada vehículo junto con los datos de su propietario.</summary>
        private const string gStrConsultaBaseSC = @"
            SELECT v.IdVehiculo, v.IdCliente, v.Placa, v.VIN, v.Marca, v.Modelo, v.Anio, v.FechaRegistro,
                   c.DPI, c.Nombres, c.Apellidos, c.Telefono, c.Correo
            FROM dbo.Vehiculos AS v
            INNER JOIN dbo.Clientes AS c ON c.IdCliente = v.IdCliente";

        /// <summary>Comprueba si una placa ya está registrada.</summary>
        /// <param name="pStrPlacaSC">Placa normalizada.</param>
        /// <returns><c>true</c> si la placa existe.</returns>
        public bool FncExistePlacaSC(string pStrPlacaSC)
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Vehiculos WHERE Placa = @Placa;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@Placa", SqlDbType.VarChar, 10).Value = pStrPlacaSC;
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar()) > 0;
            }
        }

        /// <summary>Comprueba si un VIN ya está registrado.</summary>
        /// <param name="pStrVinSC">VIN normalizado de 17 caracteres.</param>
        /// <returns><c>true</c> si el VIN existe.</returns>
        public bool FncExisteVinSC(string pStrVinSC)
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Vehiculos WHERE VIN = @VIN;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@VIN", SqlDbType.VarChar, 17).Value = pStrVinSC;
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar()) > 0;
            }
        }

        /// <summary>Inserta un vehículo validado y devuelve el identificador generado.</summary>
        /// <param name="pObjVehiculoSC">Vehículo validado y normalizado por la Capa Lógica.</param>
        /// <returns>Identificador asignado por SQL Server.</returns>
        /// <exception cref="ClsDatoDuplicadoExcepcionDASC">
        /// La placa (UQ_Vehiculos_Placa) o el VIN (UQ_Vehiculos_VIN) ya existen.
        /// </exception>
        public int FncInsertarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC)
        {
            const string lStrConsultaSC = @"
                INSERT INTO dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
                OUTPUT INSERTED.IdVehiculo
                VALUES (@IdCliente, @Placa, @VIN, @Marca, @Modelo, @Anio);";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@IdCliente", SqlDbType.Int).Value = pObjVehiculoSC.gIntIdClienteSC;
                lObjComandoSC.Parameters.Add("@Placa", SqlDbType.VarChar, 10).Value = pObjVehiculoSC.gStrPlacaSC;
                lObjComandoSC.Parameters.Add("@VIN", SqlDbType.VarChar, 17).Value = pObjVehiculoSC.gStrVinSC;
                lObjComandoSC.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = pObjVehiculoSC.gStrMarcaSC;
                lObjComandoSC.Parameters.Add("@Modelo", SqlDbType.NVarChar, 50).Value = pObjVehiculoSC.gStrModeloSC;
                lObjComandoSC.Parameters.Add("@Anio", SqlDbType.SmallInt).Value = pObjVehiculoSC.gIntAnioSC;

                try
                {
                    lObjConexionSC.Open();
                    return Convert.ToInt32(lObjComandoSC.ExecuteScalar());
                }
                catch (SqlException lObjExcepcionSC) when (ClsDatoDuplicadoExcepcionDASC.FncEsDuplicadoSC(lObjExcepcionSC))
                {
                    throw new ClsDatoDuplicadoExcepcionDASC(lObjExcepcionSC);
                }
            }
        }

        /// <summary>
        /// Busca vehículos cuya placa o VIN contenga el texto indicado. Sin texto devuelve
        /// los registrados más recientemente. Las coincidencias exactas aparecen primero.
        /// </summary>
        /// <param name="pStrTerminoSC">Placa o VIN normalizado (letras y números), o <c>null</c> para listar.</param>
        /// <param name="pIntMaximoSC">Cantidad máxima de filas a devolver.</param>
        /// <returns>Vehículos encontrados, cada uno con su propietario.</returns>
        public List<ClsVehiculoENSC> FncBuscarVehiculosSC(string pStrTerminoSC, int pIntMaximoSC)
        {
            const string lStrConsultaSC = "SELECT TOP (@Maximo) * FROM (" + gStrConsultaBaseSC + @") AS r
                WHERE @Patron IS NULL
                   OR r.Placa LIKE @Patron ESCAPE '\'
                   OR r.VIN LIKE @Patron ESCAPE '\'
                ORDER BY CASE WHEN @Patron IS NULL THEN r.IdVehiculo END DESC,
                         CASE WHEN r.Placa = @Exacto OR r.VIN = @Exacto THEN 0 ELSE 1 END,
                         r.Placa;";

            List<ClsVehiculoENSC> lLstVehiculosSC = new List<ClsVehiculoENSC>();

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                bool lBlnSinTerminoSC = string.IsNullOrEmpty(pStrTerminoSC);
                lObjComandoSC.Parameters.Add("@Maximo", SqlDbType.Int).Value = pIntMaximoSC;
                lObjComandoSC.Parameters.Add("@Patron", SqlDbType.VarChar, 40).Value = lBlnSinTerminoSC
                    ? (object)DBNull.Value
                    : "%" + ClsSqlDASC.FncEscaparPatronLikeSC(pStrTerminoSC) + "%";
                lObjComandoSC.Parameters.Add("@Exacto", SqlDbType.VarChar, 17).Value = lBlnSinTerminoSC
                    ? (object)DBNull.Value
                    : pStrTerminoSC;

                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    while (lObjLectorSC.Read())
                    {
                        lLstVehiculosSC.Add(FncLeerVehiculoSC(lObjLectorSC));
                    }
                }
            }

            return lLstVehiculosSC;
        }

        /// <summary>Obtiene un vehículo y su propietario por el identificador del vehículo.</summary>
        /// <param name="pIntIdVehiculoSC">Identificador del vehículo.</param>
        /// <returns>El vehículo encontrado o <c>null</c> si no existe.</returns>
        public ClsVehiculoENSC FncObtenerVehiculoPorIdSC(int pIntIdVehiculoSC)
        {
            const string lStrConsultaSC = gStrConsultaBaseSC + " WHERE v.IdVehiculo = @IdVehiculo;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@IdVehiculo", SqlDbType.Int).Value = pIntIdVehiculoSC;
                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    return lObjLectorSC.Read() ? FncLeerVehiculoSC(lObjLectorSC) : null;
                }
            }
        }

        /// <summary>Lista los vehículos de un cliente, del registrado más recientemente al más antiguo.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente propietario.</param>
        /// <returns>Vehículos del cliente (vacío si no tiene).</returns>
        public List<ClsVehiculoENSC> FncListarVehiculosPorClienteSC(int pIntIdClienteSC)
        {
            const string lStrConsultaSC = gStrConsultaBaseSC + @"
                WHERE v.IdCliente = @IdCliente
                ORDER BY v.IdVehiculo DESC;";

            List<ClsVehiculoENSC> lLstVehiculosSC = new List<ClsVehiculoENSC>();

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@IdCliente", SqlDbType.Int).Value = pIntIdClienteSC;
                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    while (lObjLectorSC.Read())
                    {
                        lLstVehiculosSC.Add(FncLeerVehiculoSC(lObjLectorSC));
                    }
                }
            }

            return lLstVehiculosSC;
        }

        /// <summary>Cuenta todos los vehículos registrados.</summary>
        /// <returns>Cantidad total de vehículos.</returns>
        public int FncContarVehiculosSC()
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Vehiculos;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar());
            }
        }

        /// <summary>Convierte la fila actual (vehículo + propietario) en una entidad.</summary>
        private static ClsVehiculoENSC FncLeerVehiculoSC(SqlDataReader pObjLectorSC)
        {
            int lIntIdClienteSC = pObjLectorSC.GetInt32(1);
            return new ClsVehiculoENSC
            {
                gIntIdVehiculoSC = pObjLectorSC.GetInt32(0),
                gIntIdClienteSC = lIntIdClienteSC,
                gStrPlacaSC = pObjLectorSC.GetString(2),
                gStrVinSC = pObjLectorSC.GetString(3),
                gStrMarcaSC = pObjLectorSC.GetString(4),
                gStrModeloSC = pObjLectorSC.GetString(5),
                gIntAnioSC = pObjLectorSC.GetInt16(6),
                // SQL Server devuelve DATETIME2 sin zona; la columna se guarda en UTC.
                gDatFechaRegistroUtcSC = DateTime.SpecifyKind(pObjLectorSC.GetDateTime(7), DateTimeKind.Utc),
                gObjPropietarioSC = new ClsClienteENSC
                {
                    gIntIdClienteSC = lIntIdClienteSC,
                    gStrDpiSC = pObjLectorSC.GetString(8),
                    gStrNombresSC = pObjLectorSC.GetString(9),
                    gStrApellidosSC = pObjLectorSC.GetString(10),
                    gStrTelefonoSC = pObjLectorSC.GetString(11),
                    gStrCorreoSC = pObjLectorSC.IsDBNull(12) ? null : pObjLectorSC.GetString(12)
                }
            };
        }
    }
}
