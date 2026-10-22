//==============================================================
// Nombre: Fátima Palacios Ordoñez
// Carné: 0900-21-1810
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
        public int gIntIdRepuestoFP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto.</summary>
        public string gStrCodigoFP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string gStrNombreFP { get; set; }

        /// <summary>Obtiene o establece la marca, el modelo y el año del vehículo compatible.</summary>
        public string gStrVehiculoFP { get; set; }

        /// <summary>Obtiene o establece las unidades disponibles en bodega.</summary>
        public int gIntStockFP { get; set; }

        /// <summary>Obtiene o establece el total de unidades utilizadas en órdenes de reparación.</summary>
        public int gIntUnidadesUsadasFP { get; set; }

        /// <summary>Obtiene o establece el nivel de stock: «Agotado», «Stock bajo» o «Disponible».</summary>
        public string gStrNivelStockFP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsExistenciaRepuestoENFP()
        {
            gStrCodigoFP = string.Empty;
            gStrNombreFP = string.Empty;
            gStrVehiculoFP = string.Empty;
            gStrNivelStockFP = string.Empty;
        }
    }
}
