//==============================================================
// Nombre: Fátima Palacios Ordoñez
// Carné: 0900-21-1810
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
    /// tablas de los módulos 1 (Clientes y Vehiculos), 2 (OrdenesReparacion) y 3 (Repuesto),
    /// y la de repuestos utilizados en órdenes (RepuestosOrden).
    /// Todas las consultas usan parámetros (nunca texto concatenado) para evitar inyección SQL.
    /// </summary>
    public class ClsReporteDAFP
    {
        /// <summary>
        /// Consulta base: cada orden junto con su vehículo y el cliente dueño del vehículo.
        /// Las órdenes se relacionan con el vehículo por IdVehiculo (y VIN), y el vehículo
        /// con su cliente por IdCliente.
        /// </summary>
        private const string gStrConsultaBaseFP = @"
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
            const string lStrConsultaFP = gStrConsultaBaseFP + @"
                WHERE o.Estado = N'Abierta'
                ORDER BY o.FechaApertura, o.IdOrden;";

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
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
            const string lStrConsultaFP = gStrConsultaBaseFP + @"
                WHERE o.Estado = N'Cerrada'
                  AND o.FechaCierre >= @Desde
                  AND o.FechaCierre < @Hasta
                ORDER BY o.FechaCierre DESC, o.IdOrden DESC;";

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
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
            const string lStrConsultaFP = gStrConsultaBaseFP + @"
                WHERE v.IdCliente = @IdCliente
                ORDER BY o.FechaApertura DESC, o.IdOrden DESC;";

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
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

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
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
                        gIntOrdenesAbiertasFP = lObjLectorFP.GetInt32(0),
                        gIntOrdenesCerradasMesFP = lObjLectorFP.GetInt32(1),
                        gIntVehiculosAtendidosFP = lObjLectorFP.GetInt32(2),
                        gIntMinutosPromedioReparacionFP = lObjLectorFP.IsDBNull(3) ? (int?)null : (int)lObjLectorFP.GetInt64(3),
                        gDatAperturaMasAntiguaUtcFP = lObjLectorFP.IsDBNull(4)
                            ? (DateTime?)null
                            : DateTime.SpecifyKind(lObjLectorFP.GetDateTime(4), DateTimeKind.Utc)
                    };
                }
            }
        }

        /// <summary>
        /// Lista todos los repuestos del inventario con su stock y las unidades que se han
        /// utilizado en órdenes. Primero los que tienen menos unidades (los que requieren compra).
        /// </summary>
        /// <returns>Existencias de cada repuesto.</returns>
        public List<ClsExistenciaRepuestoENFP> FncListarExistenciasFP()
        {
            // LEFT JOIN: un repuesto que nunca se ha usado también aparece, con 0 unidades usadas.
            const string lStrConsultaFP = @"
                SELECT r.idrepuesto, r.codigo, r.nombre, r.marcacarro, r.modelocarro, r.aniocarro, r.stock,
                       COALESCE(SUM(ro.Cantidad), 0) AS UnidadesUsadas
                FROM dbo.Repuesto AS r
                LEFT JOIN dbo.RepuestosOrden AS ro ON ro.IdRepuesto = r.idrepuesto
                GROUP BY r.idrepuesto, r.codigo, r.nombre, r.marcacarro, r.modelocarro, r.aniocarro, r.stock
                ORDER BY r.stock, r.nombre, r.codigo;";

            List<ClsExistenciaRepuestoENFP> lLstExistenciasFP = new List<ClsExistenciaRepuestoENFP>();

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjConexionFP.Open();
                using (SqlDataReader lObjLectorFP = lObjComandoFP.ExecuteReader())
                {
                    while (lObjLectorFP.Read())
                    {
                        lLstExistenciasFP.Add(new ClsExistenciaRepuestoENFP
                        {
                            gIntIdRepuestoFP = lObjLectorFP.GetInt32(0),
                            gStrCodigoFP = lObjLectorFP.GetString(1),
                            gStrNombreFP = lObjLectorFP.GetString(2),
                            gStrVehiculoFP = lObjLectorFP.GetString(3) + " " + lObjLectorFP.GetString(4) + " " + lObjLectorFP.GetInt32(5),
                            gIntStockFP = lObjLectorFP.GetInt32(6),
                            gIntUnidadesUsadasFP = lObjLectorFP.GetInt32(7)
                        });
                    }
                }
            }

            return lLstExistenciasFP;
        }

        /// <summary>
        /// Suma, por repuesto, las unidades utilizadas en órdenes dentro de un rango de fechas
        /// de registro. Primero los repuestos más utilizados.
        /// </summary>
        /// <param name="pDatDesdeUtcFP">Inicio del rango en UTC (incluido).</param>
        /// <param name="pDatHastaUtcFP">Fin del rango en UTC (no incluido).</param>
        /// <returns>Repuestos utilizados en el rango.</returns>
        public List<ClsRepuestoUtilizadoENFP> FncListarRepuestosUtilizadosFP(DateTime pDatDesdeUtcFP, DateTime pDatHastaUtcFP)
        {
            const string lStrConsultaFP = @"
                SELECT r.idrepuesto, r.codigo, r.nombre,
                       SUM(ro.Cantidad) AS Unidades,
                       COUNT(DISTINCT ro.IdOrden) AS Ordenes,
                       MAX(ro.FechaRegistro) AS UltimoUso,
                       r.stock
                FROM dbo.RepuestosOrden AS ro
                INNER JOIN dbo.Repuesto AS r ON r.idrepuesto = ro.IdRepuesto
                WHERE ro.FechaRegistro >= @Desde AND ro.FechaRegistro < @Hasta
                GROUP BY r.idrepuesto, r.codigo, r.nombre, r.stock
                ORDER BY Unidades DESC, r.nombre;";

            List<ClsRepuestoUtilizadoENFP> lLstUtilizadosFP = new List<ClsRepuestoUtilizadoENFP>();

            using (SqlConnection lObjConexionFP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoFP = new SqlCommand(lStrConsultaFP, lObjConexionFP))
            {
                lObjComandoFP.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = pDatDesdeUtcFP;
                lObjComandoFP.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = pDatHastaUtcFP;
                lObjConexionFP.Open();
                using (SqlDataReader lObjLectorFP = lObjComandoFP.ExecuteReader())
                {
                    while (lObjLectorFP.Read())
                    {
                        lLstUtilizadosFP.Add(new ClsRepuestoUtilizadoENFP
                        {
                            gIntIdRepuestoFP = lObjLectorFP.GetInt32(0),
                            gStrCodigoFP = lObjLectorFP.GetString(1),
                            gStrNombreFP = lObjLectorFP.GetString(2),
                            gIntUnidadesFP = lObjLectorFP.GetInt32(3),
                            gIntOrdenesFP = lObjLectorFP.GetInt32(4),
                            gDatUltimoUsoUtcFP = DateTime.SpecifyKind(lObjLectorFP.GetDateTime(5), DateTimeKind.Utc),
                            gIntStockActualFP = lObjLectorFP.GetInt32(6)
                        });
                    }
                }
            }

            return lLstUtilizadosFP;
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
                        gIntIdOrdenFP = lObjLectorFP.GetInt32(0),
                        gIntIdVehiculoFP = lObjLectorFP.GetInt32(1),
                        gStrPlacaFP = lObjLectorFP.GetString(2),
                        gStrVinFP = lObjLectorFP.GetString(3),
                        gStrMarcaFP = lObjLectorFP.GetString(4),
                        gStrModeloFP = lObjLectorFP.GetString(5),
                        gIntIdClienteFP = lObjLectorFP.GetInt32(6),
                        gStrClienteFP = lObjLectorFP.GetString(7),
                        gStrEstadoFP = lObjLectorFP.GetString(8),
                        gStrMotivoIngresoFP = lObjLectorFP.GetString(9),
                        // SQL Server devuelve DATETIME2 sin zona; las fechas se guardan en UTC.
                        gDatFechaAperturaUtcFP = DateTime.SpecifyKind(lObjLectorFP.GetDateTime(10), DateTimeKind.Utc),
                        gDatFechaCierreUtcFP = lObjLectorFP.IsDBNull(11)
                            ? (DateTime?)null
                            : DateTime.SpecifyKind(lObjLectorFP.GetDateTime(11), DateTimeKind.Utc)
                    });
                }
            }
            return lLstOrdenesFP;
        }
    }
}
