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
        public int lIntIdRepuestoFP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto.</summary>
        public string lStrCodigoFP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string lStrNombreFP { get; set; }

        /// <summary>Obtiene o establece el total de unidades utilizadas en el rango.</summary>
        public int lIntUnidadesFP { get; set; }

        /// <summary>Obtiene o establece en cuántas órdenes distintas se utilizó en el rango.</summary>
        public int lIntOrdenesFP { get; set; }

        /// <summary>Obtiene o establece la fecha (UTC) del último uso dentro del rango.</summary>
        public DateTime lDatUltimoUsoUtcFP { get; set; }

        /// <summary>Obtiene o establece las unidades que quedan hoy en bodega.</summary>
        public int lIntStockActualFP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsRepuestoUtilizadoENFP()
        {
            lStrCodigoFP = string.Empty;
            lStrNombreFP = string.Empty;
        }
    }
}
