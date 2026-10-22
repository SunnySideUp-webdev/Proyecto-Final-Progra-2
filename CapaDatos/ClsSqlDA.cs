//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Capa de Datos
//==============================================================
namespace CapaDatos
{
    /// <summary>Utilidades de SQL usadas por las clases de acceso a datos.</summary>
    internal static class ClsSqlDA
    {
        /// <summary>
        /// Escapa los comodines de LIKE (%, _ y [) para que el texto del usuario se busque
        /// literalmente. Se usa junto con la cláusula ESCAPE '\' en la consulta.
        /// </summary>
        /// <param name="pStrTexto">Texto que se usará dentro de un patrón LIKE.</param>
        /// <returns>Texto con los comodines escapados.</returns>
        internal static string fncEscaparPatronLike(string pStrTexto)
        {
            return pStrTexto
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_")
                .Replace("[", @"\[");
        }
    }
}
