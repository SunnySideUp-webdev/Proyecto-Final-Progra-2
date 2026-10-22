//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Capa de Datos
//==============================================================
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CapaDatos
{
    /// <summary>
    /// Indica que SQL Server rechazó un registro porque viola una restricción UNIQUE
    /// o un índice único. Permite que la Capa Lógica responda con un mensaje claro
    /// sin depender de los números de error de SQL Server.
    /// </summary>
    public class ClsDatoDuplicadoExcepcionDA : Exception
    {
        /// <summary>Error 2627: violación de una restricción UNIQUE o PRIMARY KEY.</summary>
        private const int lIntErrorRestriccionUnica = 2627;

        /// <summary>Error 2601: fila duplicada en un índice único.</summary>
        private const int lIntErrorIndiceUnico = 2601;

        private static readonly Regex lRegNombreRestriccion = new Regex(@"'((?:UQ|UX|PK)_[A-Za-z0-9_]+)'");

        /// <summary>Obtiene el nombre de la restricción violada, por ejemplo UQ_Clientes_DPI.</summary>
        public string lStrRestriccion { get; }

        /// <summary>Crea la excepción a partir del error original de SQL Server.</summary>
        /// <param name="pObjExcepcionSql">Error devuelto por SQL Server.</param>
        public ClsDatoDuplicadoExcepcionDA(SqlException pObjExcepcionSql)
            : base("El registro ya existe en la base de datos.", pObjExcepcionSql)
        {
            Match lObjCoincidencia = lRegNombreRestriccion.Match(pObjExcepcionSql.Message);
            lStrRestriccion = lObjCoincidencia.Success ? lObjCoincidencia.Groups[1].Value : string.Empty;
        }

        /// <summary>Indica si un error de SQL Server corresponde a un dato duplicado.</summary>
        /// <param name="pObjExcepcionSql">Error devuelto por SQL Server.</param>
        /// <returns><c>true</c> si el error es 2627 o 2601.</returns>
        public static bool fncEsDuplicado(SqlException pObjExcepcionSql)
        {
            return pObjExcepcionSql.Number == lIntErrorRestriccionUnica
                || pObjExcepcionSql.Number == lIntErrorIndiceUnico;
        }
    }
}
