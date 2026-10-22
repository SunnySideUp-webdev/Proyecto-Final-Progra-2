//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Datos
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>
    /// Consultas de solo lectura para los reportes. No inserta ni modifica datos: lee las
    /// tablas de los módulos 1 (Clientes y Vehiculos) y 2 (OrdenesReparacion).
    /// Todas las consultas usan parámetros (nunca texto concatenado) para evitar inyección SQL.
    /// </summary>
    public class ClsReporteDAFP
    {
        /// <summary>
        /// Consulta base: cada orden junto con su vehículo y el cliente dueño del vehículo.
        /// Las órdenes se relacionan con el vehículo por IdVehiculo (y VIN), y el vehículo
        /// con su cliente por IdCliente.
        /// </summary>
        private const string lStrConsultaBaseFP = @"
            SELECT o.IdOrden, o.IdVehiculo, v.Placa, v.VIN, v.Marca, v.Modelo,
                   c.IdCliente, c.Nombres + N' ' + c.Apellidos AS Cliente,
                   o.Estado, o.MotivoIngreso, o.FechaApertura, o.FechaCierre
            FROM dbo.OrdenesReparacion AS o
            INNER JOIN dbo.Vehiculos AS v ON v.IdVehiculo = o.IdVehiculo
            INNER JOIN dbo.Clientes AS c ON c.IdCliente = v.IdCliente";

        /// <summary>Lista las órdenes abiertas, de la más antigua a la más reciente.</summary>
        /// <returns>Órdenes abiertas con su vehículo y su cliente.</returns>
        public List<ClsReporteOrdenENFP> FncListarOrdenesAbiertasFP()
        {
            const string lStrConsultaFP = lStrConsultaBaseFP + @"
                WHERE o.Estado = N'Abierta'
                ORDER BY o.FechaApertura, o.IdOrden;";

            using (SqlConnection lObjConexionFP = ClsConexionDA.fncObtenerConexion())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjConexionFP.Open();
                return FncLeerOrdenesFP(lObjComandoFP);
            }
        }

        /// <summary>
        /// Lista las órdenes cerradas dentro de un rango de fechas de cierre, de la más
        /// reciente a la más antigua. El rango incluye <paramref name="pDatDesdeUtcFP"/>
        /// y excluye <paramref name="pDatHastaUtcFP"/>.
        /// </summary>
        /// <param name="pDatDesdeUtcFP">Inicio del rango, en UTC (incluido).</param>
        /// <param name="pDatHastaUtcFP">Fin del rango, en UTC (no incluido).</param>
        /// <returns>Órdenes cerradas en el rango, con su vehículo y su cliente.</returns>
        public List<ClsReporteOrdenENFP> FncListarOrdenesCerradasFP(DateTime pDatDesdeUtcFP, DateTime pDatHastaUtcFP)
        {
            const string lStrConsultaFP = lStrConsultaBaseFP + @"
                WHERE o.Estado = N'Cerrada'
                  AND o.FechaCierre >= @Desde
                  AND o.FechaCierre < @Hasta
                ORDER BY o.FechaCierre DESC, o.IdOrden DESC;";

            using (SqlConnection lObjConexionFP = ClsConexionDA.fncObtenerConexion())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjComandoFP.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = pDatDesdeUtcFP;
                lObjComandoFP.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = pDatHastaUtcFP;
                lObjConexionFP.Open();
                return FncLeerOrdenesFP(lObjComandoFP);
            }
        }

        /// <summary>
        /// Lista todas las órdenes (abiertas y cerradas) de todos los vehículos de un cliente,
        /// de la más reciente a la más antigua.
        /// </summary>
        /// <param name="pIntIdClienteFP">Identificador del cliente.</param>
        /// <returns>Historial de reparaciones del cliente; vacío si no tiene órdenes.</returns>
        public List<ClsReporteOrdenENFP> FncListarOrdenesPorClienteFP(int pIntIdClienteFP)
        {
            const string lStrConsultaFP = lStrConsultaBaseFP + @"
                WHERE v.IdCliente = @IdCliente
                ORDER BY o.FechaApertura DESC, o.IdOrden DESC;";

            using (SqlConnection lObjConexionFP = ClsConexionDA.fncObtenerConexion())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjComandoFP.Parameters.Add("@IdCliente", SqlDbType.Int).Value = pIntIdClienteFP;
                lObjConexionFP.Open();
                return FncLeerOrdenesFP(lObjComandoFP);
            }
        }

        /// <summary>
        /// Calcula los indicadores del resumen en una sola consulta: órdenes abiertas, órdenes
        /// cerradas en el mes, vehículos atendidos, tiempo promedio de reparación y la fecha
        /// de la orden abierta más antigua.
        /// </summary>
        /// <param name="pDatInicioMesUtcFP">Inicio del mes actual de Guatemala, en UTC (incluido).</param>
        /// <param name="pDatFinMesUtcFP">Inicio del mes siguiente de Guatemala, en UTC (no incluido).</param>
        /// <returns>Indicadores del taller.</returns>
        public ClsResumenReportesENFP FncObtenerResumenFP(DateTime pDatInicioMesUtcFP, DateTime pDatFinMesUtcFP)
        {
            // Cada subconsulta calcula un indicador. El promedio se calcula en segundos y luego se
            // pasa a minutos: DATEDIFF(MINUTE, …) contaría cambios de minuto en el reloj (una
            // reparación de 30 segundos entre 17:46 y 17:47 contaría 1 minuto).
            // AVG y MIN devuelven NULL cuando no hay filas (sin órdenes cerradas o abiertas).
            const string lStrConsultaFP = @"
                SELECT
                    (SELECT COUNT(*) FROM dbo.OrdenesReparacion WHERE Estado = N'Abierta') AS Abiertas,
                    (SELECT COUNT(*) FROM dbo.OrdenesReparacion
                     WHERE Estado = N'Cerrada' AND FechaCierre >= @Desde AND FechaCierre < @Hasta) AS CerradasMes,
                    (SELECT COUNT(DISTINCT IdVehiculo) FROM dbo.OrdenesReparacion) AS VehiculosAtendidos,
                    (SELECT CAST(ROUND(AVG(CAST(DATEDIFF(SECOND, FechaApertura, FechaCierre) AS FLOAT)) / 60, 0) AS BIGINT)
                     FROM dbo.OrdenesReparacion WHERE Estado = N'Cerrada' AND FechaCierre IS NOT NULL) AS MinutosPromedio,
                    (SELECT MIN(FechaApertura) FROM dbo.OrdenesReparacion WHERE Estado = N'Abierta') AS AperturaMasAntigua;";

            using (SqlConnection lObjConexionFP = ClsConexionDA.fncObtenerConexion())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjComandoFP.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = pDatInicioMesUtcFP;
                lObjComandoFP.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = pDatFinMesUtcFP;
                lObjConexionFP.Open();

                using (SqlDataReader lObjLectorFP = lObjComandoFP.ExecuteReader())
                {
                    lObjLectorFP.Read();
                    return new ClsResumenReportesENFP
                    {
                        lIntOrdenesAbiertasFP = lObjLectorFP.GetInt32(0),
                        lIntOrdenesCerradasMesFP = lObjLectorFP.GetInt32(1),
                        lIntVehiculosAtendidosFP = lObjLectorFP.GetInt32(2),
                        lIntMinutosPromedioReparacionFP = lObjLectorFP.IsDBNull(3) ? (int?)null : (int)lObjLectorFP.GetInt64(3),
                        lDatAperturaMasAntiguaUtcFP = lObjLectorFP.IsDBNull(4)
                            ? (DateTime?)null
                            : DateTime.SpecifyKind(lObjLectorFP.GetDateTime(4), DateTimeKind.Utc)
                    };
                }
            }
        }

        /// <summary>Ejecuta una consulta basada en la consulta base y convierte cada fila en una entidad.</summary>
        private static List<ClsReporteOrdenENFP> FncLeerOrdenesFP(SqlCommand pObjComandoFP)
        {
            List<ClsReporteOrdenENFP> lLstOrdenesFP = new List<ClsReporteOrdenENFP>();
            using (SqlDataReader lObjLectorFP = pObjComandoFP.ExecuteReader())
            {
                while (lObjLectorFP.Read())
                {
                    lLstOrdenesFP.Add(new ClsReporteOrdenENFP
                    {
                        lIntIdOrdenFP = lObjLectorFP.GetInt32(0),
                        lIntIdVehiculoFP = lObjLectorFP.GetInt32(1),
                        lStrPlacaFP = lObjLectorFP.GetString(2),
                        lStrVinFP = lObjLectorFP.GetString(3),
                        lStrMarcaFP = lObjLectorFP.GetString(4),
                        lStrModeloFP = lObjLectorFP.GetString(5),
                        lIntIdClienteFP = lObjLectorFP.GetInt32(6),
                        lStrClienteFP = lObjLectorFP.GetString(7),
                        lStrEstadoFP = lObjLectorFP.GetString(8),
                        lStrMotivoIngresoFP = lObjLectorFP.GetString(9),
                        // SQL Server devuelve DATETIME2 sin zona; las fechas se guardan en UTC.
                        lDatFechaAperturaUtcFP = DateTime.SpecifyKind(lObjLectorFP.GetDateTime(10), DateTimeKind.Utc),
                        lDatFechaCierreUtcFP = lObjLectorFP.IsDBNull(11)
                            ? (DateTime?)null
                            : DateTime.SpecifyKind(lObjLectorFP.GetDateTime(11), DateTimeKind.Utc)
                    });
                }
            }
            return lLstOrdenesFP;
        }
    }
}
