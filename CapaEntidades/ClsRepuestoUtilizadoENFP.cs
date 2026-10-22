//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Una fila del reporte de repuestos utilizados: un repuesto y cuánto se usó en las órdenes
    /// de reparación dentro de un rango de fechas.
    /// </summary>
    public class ClsRepuestoUtilizadoENFP
    {
        /// <summary>Obtiene o establece el identificador del repuesto.</summary>
        public int gIntIdRepuestoFP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto.</summary>
        public string gStrCodigoFP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string gStrNombreFP { get; set; }

        /// <summary>Obtiene o establece el total de unidades utilizadas en el rango.</summary>
        public int gIntUnidadesFP { get; set; }

        /// <summary>Obtiene o establece en cuántas órdenes distintas se utilizó en el rango.</summary>
        public int gIntOrdenesFP { get; set; }

        /// <summary>Obtiene o establece la fecha (UTC) del último uso dentro del rango.</summary>
        public DateTime gDatUltimoUsoUtcFP { get; set; }

        /// <summary>Obtiene o establece las unidades que quedan hoy en bodega.</summary>
        public int gIntStockActualFP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsRepuestoUtilizadoENFP()
        {
            gStrCodigoFP = string.Empty;
            gStrNombreFP = string.Empty;
        }
    }
}
