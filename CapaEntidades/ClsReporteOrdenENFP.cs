//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Una fila de los reportes de órdenes de reparación: reúne en un solo objeto los datos
    /// de la orden, de su vehículo y del cliente dueño del vehículo, para que la página no
    /// tenga que consultar tres tablas por separado.
    /// </summary>
    public class ClsReporteOrdenENFP
    {
        /// <summary>Obtiene o establece el número de la orden (dbo.OrdenesReparacion.IdOrden).</summary>
        public int gIntIdOrdenFP { get; set; }

        /// <summary>Obtiene o establece el identificador del vehículo de la orden.</summary>
        public int gIntIdVehiculoFP { get; set; }

        /// <summary>Obtiene o establece la placa actual del vehículo.</summary>
        public string gStrPlacaFP { get; set; }

        /// <summary>Obtiene o establece el VIN del vehículo.</summary>
        public string gStrVinFP { get; set; }

        /// <summary>Obtiene o establece la marca del vehículo.</summary>
        public string gStrMarcaFP { get; set; }

        /// <summary>Obtiene o establece el modelo del vehículo.</summary>
        public string gStrModeloFP { get; set; }

        /// <summary>Obtiene o establece el identificador del cliente dueño del vehículo.</summary>
        public int gIntIdClienteFP { get; set; }

        /// <summary>Obtiene o establece el nombre completo del cliente (nombres y apellidos).</summary>
        public string gStrClienteFP { get; set; }

        /// <summary>Obtiene o establece el estado de la orden: Abierta o Cerrada.</summary>
        public string gStrEstadoFP { get; set; }

        /// <summary>Obtiene o establece el motivo por el que ingresó el vehículo.</summary>
        public string gStrMotivoIngresoFP { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de apertura en UTC, tal como la guarda SQL Server.
        /// Se convierte a la hora de Guatemala solo al mostrarla.
        /// </summary>
        public DateTime gDatFechaAperturaUtcFP { get; set; }

        /// <summary>Obtiene o establece la fecha de cierre en UTC; es nula mientras la orden está abierta.</summary>
        public DateTime? gDatFechaCierreUtcFP { get; set; }

        /// <summary>
        /// Obtiene o establece cuántos días lleva abierta la orden (contados con el calendario
        /// de Guatemala). Lo calcula la Capa Lógica; solo tiene sentido en órdenes abiertas.
        /// </summary>
        public int gIntDiasAbiertaFP { get; set; }

        /// <summary>
        /// Obtiene o establece cuántos minutos duró la reparación (de la apertura al cierre).
        /// Lo calcula la Capa Lógica; solo tiene sentido en órdenes cerradas.
        /// </summary>
        public int gIntMinutosReparacionFP { get; set; }

        /// <summary>Inicializa los textos con valores vacíos para no trabajar con nulos.</summary>
        public ClsReporteOrdenENFP()
        {
            gStrPlacaFP = string.Empty;
            gStrVinFP = string.Empty;
            gStrMarcaFP = string.Empty;
            gStrModeloFP = string.Empty;
            gStrClienteFP = string.Empty;
            gStrEstadoFP = string.Empty;
            gStrMotivoIngresoFP = string.Empty;
        }
    }
}
