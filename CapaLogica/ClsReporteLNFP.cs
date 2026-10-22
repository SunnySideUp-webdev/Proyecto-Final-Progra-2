//==============================================================
// Nombre: Fátima Palacios Ordoñez
// Carné: 0900-21-1810
// Tema: AutoManager DMS - Módulo 4 Reportes - Lógica de Negocio
//==============================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    /// <summary>
    /// Reglas de los reportes: define los rangos de fechas (con el calendario de Guatemala),
    /// valida los filtros que escribe el usuario, calcula los tiempos de cada orden (días
    /// abierta y duración de la reparación) y el nivel de stock de cada repuesto. Las
    /// consultas las hace la Capa de Datos.
    /// </summary>
    public class ClsReporteLNFP
    {
        /// <summary>Cantidad máxima de días que puede abarcar un filtro de fechas.</summary>
        public const int gIntRangoMaximoDiasFP = 366;

        /// <summary>Un repuesto con esta cantidad de unidades o menos se considera con stock bajo.</summary>
        public const int gIntStockBajoFP = 3;

        // Niveles de stock del reporte de existencias.
        public const string gStrNivelAgotadoFP = "Agotado";
        public const string gStrNivelBajoFP = "Stock bajo";
        public const string gStrNivelDisponibleFP = "Disponible";

        /// <summary>Formato de fecha que envía el campo de fecha del navegador (año-mes-día).</summary>
        public const string gStrFormatoFechaFP = "yyyy-MM-dd";

        // Nombres de campo usados como clave en ClsValidacionExcepcionLNSC.
        public const string gStrCampoDesdeFP = "Desde";
        public const string gStrCampoHastaFP = "Hasta";

        private static readonly DateTime gDatFechaMinimaFP = new DateTime(2000, 1, 1);

        private readonly ClsReporteDAFP gObjReporteDAFP = new ClsReporteDAFP();

        /// <summary>
        /// Obtiene los indicadores del resumen. «Este mes» se calcula con el calendario de
        /// Guatemala y luego se convierte a UTC, porque así se guardan las fechas.
        /// </summary>
        /// <returns>Indicadores del taller, con los días de la orden abierta más antigua ya calculados.</returns>
        public ClsResumenReportesENFP FncObtenerResumenFP()
        {
            DateTime lDatHoyFP = ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC();
            DateTime lDatInicioMesFP = new DateTime(lDatHoyFP.Year, lDatHoyFP.Month, 1);

            ClsResumenReportesENFP lObjResumenFP = gObjReporteDAFP.FncObtenerResumenFP(
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(lDatInicioMesFP),
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(lDatInicioMesFP.AddMonths(1)));

            if (lObjResumenFP.gDatAperturaMasAntiguaUtcFP.HasValue)
            {
                lObjResumenFP.gIntDiasOrdenMasAntiguaFP =
                    FncCalcularDiasDesdeFP(lObjResumenFP.gDatAperturaMasAntiguaUtcFP.Value, lDatHoyFP);
            }
            return lObjResumenFP;
        }

        /// <summary>Lista las órdenes abiertas con los días que lleva abierta cada una.</summary>
        /// <returns>Órdenes abiertas, de la más antigua a la más reciente.</returns>
        public List<ClsReporteOrdenENFP> FncListarOrdenesAbiertasFP()
        {
            List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteDAFP.FncListarOrdenesAbiertasFP();
            SubCalcularTiemposFP(lLstOrdenesFP);
            return lLstOrdenesFP;
        }

        /// <summary>
        /// Lista las órdenes cerradas entre dos fechas (de cierre) escritas por el usuario.
        /// Si no se indica una fecha, se usa el mes actual: desde el día 1 hasta hoy.
        /// </summary>
        /// <param name="pStrDesdeFP">Fecha inicial en formato año-mes-día, o vacío.</param>
        /// <param name="pStrHastaFP">Fecha final en formato año-mes-día, o vacío. Se incluye el día completo.</param>
        /// <param name="pDatDesdeFP">Devuelve la fecha inicial que se usó (calendario de Guatemala).</param>
        /// <param name="pDatHastaFP">Devuelve la fecha final que se usó (calendario de Guatemala).</param>
        /// <returns>Órdenes cerradas en el rango, con la duración de cada reparación.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">Alguna fecha no es válida o el rango no es correcto.</exception>
        public List<ClsReporteOrdenENFP> FncListarOrdenesCerradasFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            SubLeerRangoFP(pStrDesdeFP, pStrHastaFP, out pDatDesdeFP, out pDatHastaFP);

            // El día final se incluye completo: se busca hasta el inicio del día siguiente.
            List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteDAFP.FncListarOrdenesCerradasFP(
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(pDatDesdeFP),
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(pDatHastaFP.AddDays(1)));
            SubCalcularTiemposFP(lLstOrdenesFP);
            return lLstOrdenesFP;
        }

        /// <summary>Lista el historial de reparaciones de todos los vehículos de un cliente.</summary>
        /// <param name="pIntIdClienteFP">Identificador del cliente.</param>
        /// <returns>Órdenes del cliente; vacío si el identificador no es válido o no tiene órdenes.</returns>
        public List<ClsReporteOrdenENFP> FncListarOrdenesPorClienteFP(int pIntIdClienteFP)
        {
            if (pIntIdClienteFP <= 0)
            {
                return new List<ClsReporteOrdenENFP>();
            }

            List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteDAFP.FncListarOrdenesPorClienteFP(pIntIdClienteFP);
            SubCalcularTiemposFP(lLstOrdenesFP);
            return lLstOrdenesFP;
        }

        /// <summary>
        /// Lista las existencias del inventario con el nivel de stock de cada repuesto, de los
        /// que tienen menos unidades a los que tienen más.
        /// </summary>
        /// <returns>Existencias de cada repuesto.</returns>
        public List<ClsExistenciaRepuestoENFP> FncListarExistenciasFP()
        {
            List<ClsExistenciaRepuestoENFP> lLstExistenciasFP = gObjReporteDAFP.FncListarExistenciasFP();
            foreach (ClsExistenciaRepuestoENFP lObjExistenciaFP in lLstExistenciasFP)
            {
                lObjExistenciaFP.gStrNivelStockFP = FncObtenerNivelStockFP(lObjExistenciaFP.gIntStockFP);
            }
            return lLstExistenciasFP;
        }

        /// <summary>
        /// Lista los repuestos utilizados en órdenes entre dos fechas (de registro del uso), con
        /// las mismas reglas de fechas que las órdenes cerradas. Sin fechas, el mes actual.
        /// </summary>
        /// <param name="pStrDesdeFP">Fecha inicial en formato año-mes-día, o vacío.</param>
        /// <param name="pStrHastaFP">Fecha final en formato año-mes-día, o vacío. Se incluye el día completo.</param>
        /// <param name="pDatDesdeFP">Devuelve la fecha inicial que se usó (calendario de Guatemala).</param>
        /// <param name="pDatHastaFP">Devuelve la fecha final que se usó (calendario de Guatemala).</param>
        /// <returns>Repuestos utilizados en el rango, del más utilizado al menos utilizado.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">Alguna fecha no es válida o el rango no es correcto.</exception>
        public List<ClsRepuestoUtilizadoENFP> FncListarRepuestosUtilizadosFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            SubLeerRangoFP(pStrDesdeFP, pStrHastaFP, out pDatDesdeFP, out pDatHastaFP);
            return gObjReporteDAFP.FncListarRepuestosUtilizadosFP(
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(pDatDesdeFP),
                ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(pDatHastaFP.AddDays(1)));
        }

        /// <summary>
        /// Nivel de stock de un repuesto: «Agotado» sin unidades, «Stock bajo» con
        /// <see cref="gIntStockBajoFP"/> unidades o menos y «Disponible» con más.
        /// </summary>
        /// <param name="pIntStockFP">Unidades en bodega.</param>
        /// <returns>Texto del nivel.</returns>
        public static string FncObtenerNivelStockFP(int pIntStockFP)
        {
            if (pIntStockFP <= 0)
            {
                return gStrNivelAgotadoFP;
            }
            return pIntStockFP <= gIntStockBajoFP ? gStrNivelBajoFP : gStrNivelDisponibleFP;
        }

        /// <summary>
        /// Lee y valida el rango de fechas de un filtro. Sin fechas usa el mes actual (del día 1
        /// a hoy); el rango no puede estar invertido ni pasar de <see cref="gIntRangoMaximoDiasFP"/> días.
        /// </summary>
        /// <exception cref="ClsValidacionExcepcionLNSC">Alguna fecha no es válida o el rango no es correcto.</exception>
        private static void SubLeerRangoFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            DateTime lDatHoyFP = ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC();
            Dictionary<string, string> lDicErroresFP = new Dictionary<string, string>();

            pDatDesdeFP = FncLeerFechaFP(pStrDesdeFP, new DateTime(lDatHoyFP.Year, lDatHoyFP.Month, 1),
                gStrCampoDesdeFP, "La fecha inicial", lDicErroresFP);
            pDatHastaFP = FncLeerFechaFP(pStrHastaFP, lDatHoyFP, gStrCampoHastaFP, "La fecha final", lDicErroresFP);

            if (lDicErroresFP.Count == 0)
            {
                if (pDatDesdeFP > pDatHastaFP)
                {
                    lDicErroresFP[gStrCampoHastaFP] = "La fecha final no puede ser anterior a la fecha inicial.";
                }
                else if ((pDatHastaFP - pDatDesdeFP).TotalDays >= gIntRangoMaximoDiasFP)
                {
                    lDicErroresFP[gStrCampoHastaFP] = "El rango puede abarcar como máximo " + gIntRangoMaximoDiasFP + " días.";
                }
            }

            if (lDicErroresFP.Count > 0)
            {
                throw new ClsValidacionExcepcionLNSC(lDicErroresFP);
            }
        }

        /// <summary>
        /// Calcula los días que lleva abierta cada orden abierta y los minutos que duró cada
        /// orden cerrada.
        /// </summary>
        private static void SubCalcularTiemposFP(List<ClsReporteOrdenENFP> pLstOrdenesFP)
        {
            DateTime lDatHoyFP = ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC();
            foreach (ClsReporteOrdenENFP lObjOrdenFP in pLstOrdenesFP)
            {
                if (lObjOrdenFP.gDatFechaCierreUtcFP.HasValue)
                {
                    lObjOrdenFP.gIntMinutosReparacionFP = (int)Math.Round(
                        (lObjOrdenFP.gDatFechaCierreUtcFP.Value - lObjOrdenFP.gDatFechaAperturaUtcFP).TotalMinutes);
                }
                else
                {
                    lObjOrdenFP.gIntDiasAbiertaFP = FncCalcularDiasDesdeFP(lObjOrdenFP.gDatFechaAperturaUtcFP, lDatHoyFP);
                }
            }
        }

        /// <summary>
        /// Cuenta los días de calendario de Guatemala entre una fecha (UTC) y hoy. Una orden
        /// abierta hoy lleva 0 días; una abierta ayer, 1 día, sin importar la hora.
        /// </summary>
        private static int FncCalcularDiasDesdeFP(DateTime pDatUtcFP, DateTime pDatHoyFP)
        {
            DateTime lDatDiaFP = ClsZonaHorariaLNSC.FncConvertirUtcAGuatemalaSC(pDatUtcFP).Date;
            return Math.Max(0, (pDatHoyFP - lDatDiaFP).Days);
        }

        /// <summary>
        /// Convierte el texto de un campo de fecha (año-mes-día) en una fecha. Si está vacío,
        /// devuelve el valor por defecto; si no es válido, agrega el error al diccionario.
        /// </summary>
        private static DateTime FncLeerFechaFP(string pStrFechaFP, DateTime pDatPorDefectoFP, string pStrCampoFP,
            string pStrEtiquetaFP, Dictionary<string, string> pDicErroresFP)
        {
            string lStrFechaFP = (pStrFechaFP ?? string.Empty).Trim();
            if (lStrFechaFP.Length == 0)
            {
                return pDatPorDefectoFP;
            }

            DateTime lDatFechaFP;
            if (!DateTime.TryParseExact(lStrFechaFP, gStrFormatoFechaFP, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out lDatFechaFP))
            {
                pDicErroresFP[pStrCampoFP] = pStrEtiquetaFP + " no es válida. Elígela en el calendario.";
                return pDatPorDefectoFP;
            }
            if (lDatFechaFP < gDatFechaMinimaFP)
            {
                pDicErroresFP[pStrCampoFP] = pStrEtiquetaFP + " debe ser del año 2000 en adelante.";
                return pDatPorDefectoFP;
            }
            return lDatFechaFP;
        }
    }
}
