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
        public int lIntIdRepuestoPP { get; set; }

        /// <summary>Obtiene o establece el código del repuesto (único, en mayúsculas).</summary>
        public string lStrCodigoPP { get; set; }

        /// <summary>Obtiene o establece el nombre del repuesto.</summary>
        public string lStrNombrePP { get; set; }

        /// <summary>Obtiene o establece la descripción del repuesto.</summary>
        public string lStrDescripcionPP { get; set; }

        /// <summary>Obtiene o establece las unidades disponibles en bodega (nunca negativas).</summary>
        public int lIntStockPP { get; set; }

        /// <summary>Obtiene o establece la marca del vehículo compatible.</summary>
        public string lStrMarcaCarroPP { get; set; }

        /// <summary>Obtiene o establece el modelo del vehículo compatible.</summary>
        public string lStrModeloCarroPP { get; set; }

        /// <summary>Obtiene o establece el año del vehículo compatible.</summary>
        public int lIntAnioCarroPP { get; set; }

        /// <summary>Inicializa los textos con valores no nulos.</summary>
        public ClsRepuestoENPP()
        {
            lStrCodigoPP = string.Empty;
            lStrNombrePP = string.Empty;
            lStrDescripcionPP = string.Empty;
            lStrMarcaCarroPP = string.Empty;
            lStrModeloCarroPP = string.Empty;
        }
    }
}
