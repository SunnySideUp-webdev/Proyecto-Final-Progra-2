//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Indicadores generales del taller que se muestran en la sección «Resumen» de los
    /// reportes. Son cantidades y tiempos; no incluye montos de dinero (fuera del alcance).
    /// </summary>
    public class ClsResumenReportesENFP
    {
        /// <summary>Obtiene o establece cuántas órdenes están abiertas en este momento.</summary>
        public int gIntOrdenesAbiertasFP { get; set; }

        /// <summary>Obtiene o establece cuántas órdenes se cerraron en el mes actual (calendario de Guatemala).</summary>
        public int gIntOrdenesCerradasMesFP { get; set; }

        /// <summary>Obtiene o establece cuántos vehículos distintos han tenido al menos una orden.</summary>
        public int gIntVehiculosAtendidosFP { get; set; }

        /// <summary>
        /// Obtiene o establece el tiempo promedio de reparación en minutos (de la apertura al
        /// cierre de las órdenes cerradas); es nulo si todavía no hay órdenes cerradas.
        /// </summary>
        public int? gIntMinutosPromedioReparacionFP { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de apertura (UTC) de la orden abierta más antigua;
        /// es nula si no hay órdenes abiertas.
        /// </summary>
        public DateTime? gDatAperturaMasAntiguaUtcFP { get; set; }

        /// <summary>
        /// Obtiene o establece cuántos días lleva abierta la orden más antigua. Lo calcula la
        /// Capa Lógica a partir de <see cref="gDatAperturaMasAntiguaUtcFP"/>.
        /// </summary>
        public int gIntDiasOrdenMasAntiguaFP { get; set; }
    }
}
