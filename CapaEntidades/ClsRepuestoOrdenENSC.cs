//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Repuestos utilizados en órdenes - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa un repuesto utilizado en una orden de reparación: qué repuesto, cuántas
    /// unidades y cuándo se registró. Corresponde a la tabla dbo.RepuestosOrden.
    /// </summary>
    public class ClsRepuestoOrdenENSC
    {
        /// <summary>Obtiene o establece el identificador del registro generado por SQL Server.</summary>
        public int lIntIdRepuestoOrdenSC { get; set; }

        /// <summary>Obtiene o establece la orden de reparación en la que se usó el repuesto.</summary>
        public int lIntIdOrdenSC { get; set; }

        /// <summary>Obtiene o establece el repuesto del inventario que se usó.</summary>
        public int lIntIdRepuestoSC { get; set; }

        /// <summary>Obtiene o establece el código del repuesto (dato de dbo.Repuesto).</summary>
        public string lStrCodigoSC { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto (dato de dbo.Repuesto).</summary>
        public string lStrNombreRepuestoSC { get; set; }

        /// <summary>Obtiene o establece las unidades utilizadas (mayor que cero).</summary>
        public int lIntCantidadSC { get; set; }

        /// <summary>Obtiene o establece la fecha de registro en UTC; se muestra en hora de Guatemala.</summary>
        public DateTime lDatFechaRegistroUtcSC { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsRepuestoOrdenENSC()
        {
            lStrCodigoSC = string.Empty;
            lStrNombreRepuestoSC = string.Empty;
        }
    }
}
