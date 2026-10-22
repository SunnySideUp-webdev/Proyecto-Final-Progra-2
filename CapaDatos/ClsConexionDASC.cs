//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Capa de Datos
//==============================================================
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CapaDatos
{
    /// <summary>
    /// Centraliza la creación de conexiones hacia la base compartida AutoManagerDMS.
    /// Todos los módulos del equipo deben obtener sus conexiones desde aquí.
    /// </summary>
    public static class ClsConexionDASC
    {
        /// <summary>Nombre de la cadena de conexión definida en ConnectionStrings.config.</summary>
        public const string gStrNombreCadenaConexionSC = "AutoManagerDMS";

        /// <summary>Crea una conexión usando la configuración local del proyecto.</summary>
        /// <returns>Una conexión cerrada y lista para utilizarse dentro de un bloque using.</returns>
        /// <exception cref="InvalidOperationException">
        /// Se produce cuando no existe la cadena de conexión esperada.
        /// </exception>
        public static SqlConnection FncObtenerConexionSC()
        {
            ConnectionStringSettings lObjConfiguracionSC =
                ConfigurationManager.ConnectionStrings[gStrNombreCadenaConexionSC];

            if (lObjConfiguracionSC == null || string.IsNullOrWhiteSpace(lObjConfiguracionSC.ConnectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión '" + gStrNombreCadenaConexionSC + "'. " +
                    "Crea ConnectionStrings.config a partir de ConnectionStrings.config.example.");
            }

            return new SqlConnection(lObjConfiguracionSC.ConnectionString);
        }
    }
}
