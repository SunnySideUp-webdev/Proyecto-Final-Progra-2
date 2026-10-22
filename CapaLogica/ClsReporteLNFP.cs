//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
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
        public const int lIntRangoMaximoDiasFP = 366;

        /// <summary>Un repuesto con esta cantidad de unidades o menos se considera con stock bajo.</summary>
        public const int lIntStockBajoFP = 3;

        // Niveles de stock del reporte de existencias.
        public const string lStrNivelAgotadoFP = "Agotado";
        public const string lStrNivelBajoFP = "Stock bajo";
        public const string lStrNivelDisponibleFP = "Disponible";

        /// <summary>Formato de fecha que envía el campo de fecha del navegador (año-mes-día).</summary>
        public const string lStrFormatoFechaFP = "yyyy-MM-dd";

        // Nombres de campo usados como clave en ClsValidacionExcepcionLN.
        public const string lStrCampoDesdeFP = "Desde";
        public const string lStrCampoHastaFP = "Hasta";

        private static readonly DateTime lDatFechaMinimaFP = new DateTime(2000, 1, 1);

        private readonly ClsReporteDAFP gObjReporteDAFP = new ClsReporteDAFP();

        /// <summary>
        /// Obtiene los indicadores del resumen. «Este mes» se calcula con el calendario de
        /// Guatemala y luego se convierte a UTC, porque así se guardan las fechas.
        /// </summary>
        /// <returns>Indicadores del taller, con los días de la orden abierta más antigua ya calculados.</returns>
        public ClsResumenReportesENFP FncObtenerResumenFP()
        {
            DateTime lDatHoyFP = ClsZonaHorariaLN.fncObtenerHoyGuatemala();
            DateTime lDatInicioMesFP = new DateTime(lDatHoyFP.Year, lDatHoyFP.Month, 1);

            ClsResumenReportesENFP lObjResumenFP = gObjReporteDAFP.FncObtenerResumenFP(
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(lDatInicioMesFP),
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(lDatInicioMesFP.AddMonths(1)));

            if (lObjResumenFP.lDatAperturaMasAntiguaUtcFP.HasValue)
            {
                lObjResumenFP.lIntDiasOrdenMasAntiguaFP =
                    FncCalcularDiasDesdeFP(lObjResumenFP.lDatAperturaMasAntiguaUtcFP.Value, lDatHoyFP);
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
        /// <exception cref="ClsValidacionExcepcionLN">Alguna fecha no es válida o el rango no es correcto.</exception>
        public List<ClsReporteOrdenENFP> FncListarOrdenesCerradasFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            SubLeerRangoFP(pStrDesdeFP, pStrHastaFP, out pDatDesdeFP, out pDatHastaFP);

            // El día final se incluye completo: se busca hasta el inicio del día siguiente.
            List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteDAFP.FncListarOrdenesCerradasFP(
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(pDatDesdeFP),
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(pDatHastaFP.AddDays(1)));
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
                lObjExistenciaFP.lStrNivelStockFP = FncObtenerNivelStockFP(lObjExistenciaFP.lIntStockFP);
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
        /// <exception cref="ClsValidacionExcepcionLN">Alguna fecha no es válida o el rango no es correcto.</exception>
        public List<ClsRepuestoUtilizadoENFP> FncListarRepuestosUtilizadosFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            SubLeerRangoFP(pStrDesdeFP, pStrHastaFP, out pDatDesdeFP, out pDatHastaFP);
            return gObjReporteDAFP.FncListarRepuestosUtilizadosFP(
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(pDatDesdeFP),
                ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(pDatHastaFP.AddDays(1)));
        }

        /// <summary>
        /// Nivel de stock de un repuesto: «Agotado» sin unidades, «Stock bajo» con
        /// <see cref="lIntStockBajoFP"/> unidades o menos y «Disponible» con más.
        /// </summary>
        /// <param name="pIntStockFP">Unidades en bodega.</param>
        /// <returns>Texto del nivel.</returns>
        public static string FncObtenerNivelStockFP(int pIntStockFP)
        {
            if (pIntStockFP <= 0)
            {
                return lStrNivelAgotadoFP;
            }
            return pIntStockFP <= lIntStockBajoFP ? lStrNivelBajoFP : lStrNivelDisponibleFP;
        }

        /// <summary>
        /// Lee y valida el rango de fechas de un filtro. Sin fechas usa el mes actual (del día 1
        /// a hoy); el rango no puede estar invertido ni pasar de <see cref="lIntRangoMaximoDiasFP"/> días.
        /// </summary>
        /// <exception cref="ClsValidacionExcepcionLN">Alguna fecha no es válida o el rango no es correcto.</exception>
        private static void SubLeerRangoFP(string pStrDesdeFP, string pStrHastaFP,
            out DateTime pDatDesdeFP, out DateTime pDatHastaFP)
        {
            DateTime lDatHoyFP = ClsZonaHorariaLN.fncObtenerHoyGuatemala();
            Dictionary<string, string> lDicErroresFP = new Dictionary<string, string>();

            pDatDesdeFP = FncLeerFechaFP(pStrDesdeFP, new DateTime(lDatHoyFP.Year, lDatHoyFP.Month, 1),
                lStrCampoDesdeFP, "La fecha inicial", lDicErroresFP);
            pDatHastaFP = FncLeerFechaFP(pStrHastaFP, lDatHoyFP, lStrCampoHastaFP, "La fecha final", lDicErroresFP);

            if (lDicErroresFP.Count == 0)
            {
                if (pDatDesdeFP > pDatHastaFP)
                {
                    lDicErroresFP[lStrCampoHastaFP] = "La fecha final no puede ser anterior a la fecha inicial.";
                }
                else if ((pDatHastaFP - pDatDesdeFP).TotalDays >= lIntRangoMaximoDiasFP)
                {
                    lDicErroresFP[lStrCampoHastaFP] = "El rango puede abarcar como máximo " + lIntRangoMaximoDiasFP + " días.";
                }
            }

            if (lDicErroresFP.Count > 0)
            {
                throw new ClsValidacionExcepcionLN(lDicErroresFP);
            }
        }

        /// <summary>
        /// Calcula los días que lleva abierta cada orden abierta y los minutos que duró cada
        /// orden cerrada.
        /// </summary>
        private static void SubCalcularTiemposFP(List<ClsReporteOrdenENFP> pLstOrdenesFP)
        {
            DateTime lDatHoyFP = ClsZonaHorariaLN.fncObtenerHoyGuatemala();
            foreach (ClsReporteOrdenENFP lObjOrdenFP in pLstOrdenesFP)
            {
                if (lObjOrdenFP.lDatFechaCierreUtcFP.HasValue)
                {
                    lObjOrdenFP.lIntMinutosReparacionFP = (int)Math.Round(
                        (lObjOrdenFP.lDatFechaCierreUtcFP.Value - lObjOrdenFP.lDatFechaAperturaUtcFP).TotalMinutes);
                }
                else
                {
                    lObjOrdenFP.lIntDiasAbiertaFP = FncCalcularDiasDesdeFP(lObjOrdenFP.lDatFechaAperturaUtcFP, lDatHoyFP);
                }
            }
        }

        /// <summary>
        /// Cuenta los días de calendario de Guatemala entre una fecha (UTC) y hoy. Una orden
        /// abierta hoy lleva 0 días; una abierta ayer, 1 día, sin importar la hora.
        /// </summary>
        private static int FncCalcularDiasDesdeFP(DateTime pDatUtcFP, DateTime pDatHoyFP)
        {
            DateTime lDatDiaFP = ClsZonaHorariaLN.fncConvertirUtcAGuatemala(pDatUtcFP).Date;
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
            if (!DateTime.TryParseExact(lStrFechaFP, lStrFormatoFechaFP, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out lDatFechaFP))
            {
                pDicErroresFP[pStrCampoFP] = pStrEtiquetaFP + " no es válida. Elígela en el calendario.";
                return pDatPorDefectoFP;
            }
            if (lDatFechaFP < lDatFechaMinimaFP)
            {
                pDicErroresFP[pStrCampoFP] = pStrEtiquetaFP + " debe ser del año 2000 en adelante.";
                return pDatPorDefectoFP;
            }
            return lDatFechaFP;
        }
    }
}
