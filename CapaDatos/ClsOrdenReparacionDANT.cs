
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
                ClsConexionDA.fncObtenerConexion())
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
                            lIntIdOrdenNT =
                                Convert.ToInt32(lObjLectorNT["IdOrden"]),

                            lIntIdVehiculoNT =
                                Convert.ToInt32(lObjLectorNT["IdVehiculo"]),

                            lStrVinNT =
                                Convert.ToString(lObjLectorNT["VIN"]),

                            lStrPlacaNT =
                                Convert.ToString(lObjLectorNT["Placa"]),

                            lDatFechaAperturaNT =
                                Convert.ToDateTime(
                                    lObjLectorNT["FechaApertura"]),

                            lStrMotivoIngresoNT =
                                Convert.ToString(
                                    lObjLectorNT["MotivoIngreso"]),

                            lStrDiagnosticoNT =
                                lObjLectorNT["Diagnostico"] == DBNull.Value
                                    ? string.Empty
                                    : Convert.ToString(
                                        lObjLectorNT["Diagnostico"]),

                            lStrEstadoNT =
                                Convert.ToString(lObjLectorNT["Estado"]),

                            lDatFechaCierreNT =
                                lObjLectorNT["FechaCierre"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(
                                        lObjLectorNT["FechaCierre"]),

                            lStrObservacionesCierreNT =
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
                ClsConexionDA.fncObtenerConexion())
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
                                lIntIdOrdenNT = lObjLectorNT.GetInt32(0),
                                lIntIdVehiculoNT = lObjLectorNT.GetInt32(1),
                                lStrVinNT = lObjLectorNT.IsDBNull(2)
                                    ? string.Empty : lObjLectorNT.GetString(2),
                                lStrPlacaNT = lObjLectorNT.IsDBNull(3)
                                    ? string.Empty : lObjLectorNT.GetString(3),
                                lDatFechaAperturaNT = lObjLectorNT.GetDateTime(4),
                                lStrMotivoIngresoNT = lObjLectorNT.GetString(5),
                                lStrDiagnosticoNT = lObjLectorNT.IsDBNull(6)
                                    ? string.Empty : lObjLectorNT.GetString(6),
                                lStrEstadoNT = lObjLectorNT.GetString(7),
                                lDatFechaCierreNT = lObjLectorNT.IsDBNull(8)
                                    ? (DateTime?)null : lObjLectorNT.GetDateTime(8),
                                lStrObservacionesCierreNT = lObjLectorNT.IsDBNull(9)
                                    ? string.Empty : lObjLectorNT.GetString(9)
                            };

                        lObjOrdenesNT.Add(lObjOrdenNT);
                    }
                }
            }

            return lObjOrdenesNT;
        }


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
                ClsConexionDA.fncObtenerConexion())
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
                        lIntIdOrdenNT =
                            Convert.ToInt32(lObjLectorNT["IdOrden"]),

                        lIntIdVehiculoNT =
                            Convert.ToInt32(lObjLectorNT["IdVehiculo"]),

                        lStrVinNT =
                            lObjLectorNT["VIN"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(lObjLectorNT["VIN"]),

                        lStrPlacaNT =
                            lObjLectorNT["Placa"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(lObjLectorNT["Placa"]),

                        lDatFechaAperturaNT =
                            Convert.ToDateTime(
                                lObjLectorNT["FechaApertura"]),

                        lStrMotivoIngresoNT =
                            Convert.ToString(
                                lObjLectorNT["MotivoIngreso"]),

                        lStrDiagnosticoNT =
                            lObjLectorNT["Diagnostico"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(
                                    lObjLectorNT["Diagnostico"]),

                        lStrEstadoNT =
                            Convert.ToString(lObjLectorNT["Estado"]),

                        lDatFechaCierreNT =
                            lObjLectorNT["FechaCierre"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(
                                    lObjLectorNT["FechaCierre"]),

                        lStrObservacionesCierreNT =
                            lObjLectorNT["ObservacionesCierre"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(
                                    lObjLectorNT["ObservacionesCierre"])
                    };
                }
            }
        }



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
                ClsConexionDA.fncObtenerConexion())
            using (SqlCommand lObjComandoNT =
                new SqlCommand(lStrConsultaNT, lObjConexionNT))
            {
                lObjComandoNT.Parameters.Add(
                    "@IdVehiculo", SqlDbType.Int).Value =
                    pObjOrdenNT.lIntIdVehiculoNT;

                lObjComandoNT.Parameters.Add(
                    "@VIN", SqlDbType.VarChar, 17).Value =
                    pObjOrdenNT.lStrVinNT;

                lObjComandoNT.Parameters.Add(
                    "@Placa", SqlDbType.VarChar, 20).Value =
                    (object)pObjOrdenNT.lStrPlacaNT ?? DBNull.Value;

                lObjComandoNT.Parameters.Add(
                    "@MotivoIngreso", SqlDbType.NVarChar, 500).Value =
                    pObjOrdenNT.lStrMotivoIngresoNT;

                lObjComandoNT.Parameters.Add(
                    "@Diagnostico", SqlDbType.NVarChar, 1000).Value =
                    (object)pObjOrdenNT.lStrDiagnosticoNT ?? DBNull.Value;

                lObjComandoNT.Parameters.Add(
                    "@Estado", SqlDbType.NVarChar, 20).Value =
                    "Abierta";

                lObjConexionNT.Open();

                return Convert.ToInt32(lObjComandoNT.ExecuteScalar());
            }
        }


        public bool FncCerrarOrdenNT(
            int pIntIdOrdenNT,
            string pStrObservacionesNT)
        {
            const string lStrConsultaNT = @"
        UPDATE dbo.OrdenesReparacion
        SET
            Estado = N'Cerrada',
            FechaCierre = SYSDATETIME(),
            ObservacionesCierre = @Observaciones
        WHERE IdOrden = @IdOrden
          AND Estado = N'Abierta';";

            using (SqlConnection lObjConexionNT =
                ClsConexionDA.fncObtenerConexion())
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
