//==============================================================
// Nombre: Pavel Alexander Palma Carrillo
// Carné: 0900-24-4296
// Tema: AutoManager DMS - Módulo 3 Inventario - Capa de Entidades
//==============================================================
namespace CapaEntidades
{
    /// <summary>
    /// Representa un repuesto del inventario y el vehículo con el que es compatible.
    /// Corresponde a la tabla dbo.Repuesto.
    /// </summary>
    public class ClsRepuestoENPP
    {
        /// <summary>Obtiene o establece el identificador generado por SQL Server.</summary>
        public int gIntIdRepuestoPP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto (único, en mayúsculas).</summary>
        public string gStrCodigoPP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string gStrNombrePP { get; set; }

        /// <summary>Obtiene o establece la descripción del repuesto.</summary>
        public string gStrDescripcionPP { get; set; }

        /// <summary>Obtiene o establece las unidades disponibles en bodega (nunca negativas).</summary>
        public int gIntStockPP { get; set; }

        /// <summary>Obtiene o establece la marca del vehículo compatible.</summary>
        public string gStrMarcaCarroPP { get; set; }

        /// <summary>Obtiene o establece el modelo del vehículo compatible.</summary>
        public string gStrModeloCarroPP { get; set; }

        /// <summary>Obtiene o establece el año del vehículo compatible.</summary>
        public int gIntAnioCarroPP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsRepuestoENPP()
        {
            gStrCodigoPP = string.Empty;
            gStrNombrePP = string.Empty;
            gStrDescripcionPP = string.Empty;
            gStrMarcaCarroPP = string.Empty;
            gStrModeloCarroPP = string.Empty;
        }
    }
}
