//==============================================================
// Nombre: Fátima PENDIENTE (nombre completo)
// Carné: PENDIENTE
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Entidades
//==============================================================
namespace CapaEntidades
{
    /// <summary>
    /// Una fila del reporte de existencias: un repuesto del inventario, sus unidades en bodega,
    /// cuántas se han utilizado en órdenes y su nivel de stock (lo calcula la Capa Lógica).
    /// </summary>
    public class ClsExistenciaRepuestoENFP
    {
        /// <summary>Obtiene o establece el identificador del repuesto.</summary>
        public int lIntIdRepuestoFP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto.</summary>
        public string lStrCodigoFP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string lStrNombreFP { get; set; }

        /// <summary>Obtiene o establece la marca, el modelo y el año del vehículo compatible.</summary>
        public string lStrVehiculoFP { get; set; }

        /// <summary>Obtiene o establece las unidades disponibles en bodega.</summary>
        public int lIntStockFP { get; set; }

        /// <summary>Obtiene o establece el total de unidades utilizadas en órdenes de reparación.</summary>
        public int lIntUnidadesUsadasFP { get; set; }

        /// <summary>Obtiene o establece el nivel de stock: «Agotado», «Stock bajo» o «Disponible».</summary>
        public string lStrNivelStockFP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsExistenciaRepuestoENFP()
        {
            lStrCodigoFP = string.Empty;
            lStrNombreFP = string.Empty;
            lStrVehiculoFP = string.Empty;
            lStrNivelStockFP = string.Empty;
        }
    }
}
