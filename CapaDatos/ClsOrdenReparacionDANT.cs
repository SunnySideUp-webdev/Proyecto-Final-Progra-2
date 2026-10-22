//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Capa de Datos
// Parte de Neider: todo el código de este archivo es de su módulo.
// Ajuste de integración (Sharon): nombres según la convención del docente (prefijo g en campos y propiedades, Sub en procedimientos).
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>
    /// Acceso a datos del módulo de órdenes de reparación.
    /// Utiliza ADO.NET y la conexión compartida del equipo.
    /// </summary>
    public class ClsOrdenReparacionDANT
    {


        /// <summary>Consulta todas las órdenes (abiertas y cerradas) de un vehículo, de la más reciente a la más antigua.</summary>
        /// <param name="pStrVinNT">VIN normalizado de 17 caracteres.</param>
        /// <returns>Historial de órdenes del vehículo; vacío si no tiene.</returns>
        public List<ClsOrdenReparacionENT> FncConsultarHistorialPorVinNT(
            string pStrVinNT)
        {
            const string lStrConsultaNT = @"
        SELECT
            IdOrden,
            IdVehiculo,
            VIN,
            Placa,
            FechaApertura,
            MotivoIngreso,
            Diagnostico,
            Estado,
            FechaCierre,
            ObservacionesCierre
        FROM dbo.OrdenesReparacion
        WHERE VIN = @VIN
        ORDER BY FechaApertura DESC, IdOrden DESC;";

            List<ClsOrdenReparacionENT> lObjOrdenesNT =
                new List<ClsOrdenReparacionENT>();

            using (SqlConnection lObjConexionNT =
                ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@VIN", SqlDbType.VarChar, 17).Value =
                    pStrVinNT;

                lObjConexionNT.Open();

                using (SqlDataReader lObjLectorNT =
                    lObjComandoNT.ExecuteReader())
                {
                    while (lObjLectorNT.Read())
                    {
                        lObjOrdenesNT.Add(new ClsOrdenReparacionENT
                        {
                            gIntIdOrdenNT =
                                Convert.ToInt32(lObjLectorNT["IdOrden"]),

                            gIntIdVehiculoNT =
                                Convert.ToInt32(lObjLectorNT["IdVehiculo"]),

                            gStrVinNT =
                                Convert.ToString(lObjLectorNT["VIN"]),

                            gStrPlacaNT =
                                Convert.ToString(lObjLectorNT["Placa"]),

                            gDatFechaAperturaNT =
                                Convert.ToDateTime(
                                    lObjLectorNT["FechaApertura"]),

                            gStrMotivoIngresoNT =
                                Convert.ToString(
                                    lObjLectorNT["MotivoIngreso"]),

                            gStrDiagnosticoNT =
                                lObjLectorNT["Diagnostico"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(
                                        lObjLectorNT["Diagnostico"]),

                            gStrEstadoNT =
                                Convert.ToString(lObjLectorNT["Estado"]),

                            gDatFechaCierreNT =
                                lObjLectorNT["FechaCierre"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(
                                        lObjLectorNT["FechaCierre"]),

                            gStrObservacionesCierreNT =
                                lObjLectorNT["ObservacionesCierre"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(
                                        lObjLectorNT["ObservacionesCierre"])
                        });
                    }
                }
            }

            return lObjOrdenesNT;
        }

        /// <summary>Consulta las órdenes en estado Abierta, de la más reciente a la más antigua.</summary>
        /// <returns>Órdenes abiertas.</returns>
        public List<ClsOrdenReparacionENT> FncConsultarOrdenesAbiertasNT()
        {
            const string lStrConsultaNT = @"
        SELECT
            IdOrden,
            IdVehiculo,
            VIN,
            Placa,
            FechaApertura,
            MotivoIngreso,
            Diagnostico,
            Estado,
            FechaCierre,
            ObservacionesCierre
        FROM dbo.OrdenesReparacion
        WHERE Estado = @Estado
        ORDER BY FechaApertura DESC;";

            List<ClsOrdenReparacionENT> lObjOrdenesNT =
                new List<ClsOrdenReparacionENT>();

            using (SqlConnection lObjConexionNT =
                ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@Estado", SqlDbType.NVarChar, 20).Value = "Abierta";

                lObjConexionNT.Open();

                using (SqlDataReader lObjLectorNT =
                    lObjComandoNT.ExecuteReader())
                {
                    while (lObjLectorNT.Read())
                    {
                        ClsOrdenReparacionENT lObjOrdenNT =
                            new ClsOrdenReparacionENT
                            {
                                gIntIdOrdenNT = lObjLectorNT.GetInt32(0),
                                gIntIdVehiculoNT = lObjLectorNT.GetInt32(1),
                                gStrVinNT = lObjLectorNT.IsDBNull(2)
                                    ? string.Empty : lObjLectorNT.GetString(2),
                                gStrPlacaNT = lObjLectorNT.IsDBNull(3)
                                    ? string.Empty : lObjLectorNT.GetString(3),
                                gDatFechaAperturaNT = lObjLectorNT.GetDateTime(4),
                                gStrMotivoIngresoNT = lObjLectorNT.GetString(5),
                                gStrDiagnosticoNT = lObjLectorNT.IsDBNull(6)
                                    ? string.Empty : lObjLectorNT.GetString(6),
                                gStrEstadoNT = lObjLectorNT.GetString(7),
                                gDatFechaCierreNT = lObjLectorNT.IsDBNull(8)
                                    ? (DateTime?)null : lObjLectorNT.GetDateTime(8),
                                gStrObservacionesCierreNT = lObjLectorNT.IsDBNull(9)
                                    ? string.Empty : lObjLectorNT.GetString(9)
                            };

                        lObjOrdenesNT.Add(lObjOrdenNT);
                    }
                }
            }

            return lObjOrdenesNT;
        }


        /// <summary>Obtiene una orden por su número.</summary>
        /// <param name="pIntIdOrdenNT">Número de la orden.</param>
        /// <returns>La orden o <c>null</c> si no existe.</returns>
        public ClsOrdenReparacionENT FncObtenerOrdenPorIdNT(
            int pIntIdOrdenNT)
        {
            const string lStrConsultaNT = @"
        SELECT
            IdOrden,
            IdVehiculo,
            VIN,
            Placa,
            FechaApertura,
            MotivoIngreso,
            Diagnostico,
            Estado,
            FechaCierre,
            ObservacionesCierre
        FROM dbo.OrdenesReparacion
        WHERE IdOrden = @IdOrden;";

            using (SqlConnection lObjConexionNT =
                ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@IdOrden", SqlDbType.Int).Value =
                    pIntIdOrdenNT;

                lObjConexionNT.Open();

                using (SqlDataReader lObjLectorNT =
                    lObjComandoNT.ExecuteReader())
                {
                    if (!lObjLectorNT.Read())
                    {
                        return null;
                    }

                    return new ClsOrdenReparacionENT
                    {
                        gIntIdOrdenNT =
                            Convert.ToInt32(lObjLectorNT["IdOrden"]),

                        gIntIdVehiculoNT =
                            Convert.ToInt32(lObjLectorNT["IdVehiculo"]),

                        gStrVinNT =
                            lObjLectorNT["VIN"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(lObjLectorNT["VIN"]),

                        gStrPlacaNT =
                            lObjLectorNT["Placa"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(lObjLectorNT["Placa"]),

                        gDatFechaAperturaNT =
                            Convert.ToDateTime(
                                lObjLectorNT["FechaApertura"]),

                        gStrMotivoIngresoNT =
                            Convert.ToString(
                                lObjLectorNT["MotivoIngreso"]),

                        gStrDiagnosticoNT =
                            lObjLectorNT["Diagnostico"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(
                                    lObjLectorNT["Diagnostico"]),

                        gStrEstadoNT =
                            Convert.ToString(lObjLectorNT["Estado"]),

                        gDatFechaCierreNT =
                            lObjLectorNT["FechaCierre"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(
                                    lObjLectorNT["FechaCierre"]),

                        gStrObservacionesCierreNT =
                            lObjLectorNT["ObservacionesCierre"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(
                                    lObjLectorNT["ObservacionesCierre"])
                    };
                }
            }
        }



        /// <summary>Inserta una orden nueva en estado Abierta; la fecha de apertura la asigna SQL Server (UTC).</summary>
        /// <param name="pObjOrdenNT">Orden validada por la Capa Lógica (vehículo, VIN, placa, motivo y diagnóstico).</param>
        /// <returns>Número asignado a la orden.</returns>
        public int FncRegistrarOrdenNT(
            ClsOrdenReparacionENT pObjOrdenNT)
        {
            const string lStrConsultaNT = @"
        INSERT INTO dbo.OrdenesReparacion
        (
            IdVehiculo,
            VIN,
            Placa,
            MotivoIngreso,
            Diagnostico,
            Estado
        )
        OUTPUT INSERTED.IdOrden
        VALUES
        (
            @IdVehiculo,
            @VIN,
            @Placa,
            @MotivoIngreso,
            @Diagnostico,
            @Estado
        );";

            using (SqlConnection lObjConexionNT =
                ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@IdVehiculo", SqlDbType.Int).Value =
                    pObjOrdenNT.gIntIdVehiculoNT;

                lObjComandoNT.Parameters.Add(
                    "@VIN", SqlDbType.VarChar, 17).Value =
                    pObjOrdenNT.gStrVinNT;

                lObjComandoNT.Parameters.Add(
                    "@Placa", SqlDbType.VarChar, 20).Value =
                    (object)pObjOrdenNT.gStrPlacaNT ?? DBNull.Value;

                lObjComandoNT.Parameters.Add(
                    "@MotivoIngreso", SqlDbType.NVarChar, 500).Value =
                    pObjOrdenNT.gStrMotivoIngresoNT;

                lObjComandoNT.Parameters.Add(
                    "@Diagnostico", SqlDbType.NVarChar, 1000).Value =
                    (object)pObjOrdenNT.gStrDiagnosticoNT ?? DBNull.Value;

                lObjComandoNT.Parameters.Add(
                    "@Estado", SqlDbType.NVarChar, 20).Value =
                    "Abierta";

                lObjConexionNT.Open();

                return Convert.ToInt32(lObjComandoNT.ExecuteScalar());
            }
        }


        /// <summary>Cierra una orden abierta: cambia el estado, registra la fecha de cierre y las observaciones.</summary>
        /// <param name="pIntIdOrdenNT">Número de la orden.</param>
        /// <param name="pStrObservacionesNT">Observaciones de cierre.</param>
        /// <returns><c>true</c> si la orden existía y estaba abierta; de lo contrario, <c>false</c>.</returns>
        public bool FncCerrarOrdenNT(
            int pIntIdOrdenNT,
            string pStrObservacionesNT)
        {
            const string lStrConsultaNT = @"
        UPDATE dbo.OrdenesReparacion
        SET
            Estado = N'Cerrada',
            -- Ajuste de integracion (Sharon): fecha en UTC, igual que el resto del sistema.
            FechaCierre = SYSUTCDATETIME(),
            ObservacionesCierre = @Observaciones
        WHERE IdOrden = @IdOrden
          AND Estado = N'Abierta';";

            using (SqlConnection lObjConexionNT =
                ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@IdOrden", SqlDbType.Int).Value =
                    pIntIdOrdenNT;

                lObjComandoNT.Parameters.Add(
                    "@Observaciones", SqlDbType.NVarChar, 1000).Value =
                    pStrObservacionesNT;

                lObjConexionNT.Open();

                int lIntFilasAfectadasNT =
                    lObjComandoNT.ExecuteNonQuery();

                return lIntFilasAfectadasNT == 1;
            }
        }




    }
}
