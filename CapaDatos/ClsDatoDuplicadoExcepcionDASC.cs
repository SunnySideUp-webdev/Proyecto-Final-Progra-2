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
    public class ClsDatoDuplicadoExcepcionDASC : Exception
    {
        /// <summary>Error 2627: violación de una restricción UNIQUE o PRIMARY KEY.</summary>
        private const int gIntErrorRestriccionUnicaSC = 2627;

        /// <summary>Error 2601: fila duplicada en un índice único.</summary>
        private const int gIntErrorIndiceUnicoSC = 2601;

        private static readonly Regex gRegNombreRestriccionSC = new Regex(@"'((?:UQ|UX|PK)_[A-Za-z0-9_]+)'");

        /// <summary>Obtiene el nombre de la restricción violada, por ejemplo UQ_Clientes_DPI.</summary>
        public string gStrRestriccionSC { get; }

        /// <summary>Crea la excepción a partir del error original de SQL Server.</summary>
        /// <param name="pObjExcepcionSql">Error devuelto por SQL Server.</param>
        public ClsDatoDuplicadoExcepcionDASC(SqlException pObjExcepcionSqlSC)
            : base("El registro ya existe en la base de datos.", pObjExcepcionSqlSC)
        {
            Match lObjCoincidenciaSC = gRegNombreRestriccionSC.Match(pObjExcepcionSqlSC.Message);
            gStrRestriccionSC = lObjCoincidenciaSC.Success ? lObjCoincidenciaSC.Groups[1].Value : string.Empty;
        }

        /// <summary>Indica si un error de SQL Server corresponde a un dato duplicado.</summary>
        /// <param name="pObjExcepcionSql">Error devuelto por SQL Server.</param>
        /// <returns><c>true</c> si el error es 2627 o 2601.</returns>
        public static bool FncEsDuplicadoSC(SqlException pObjExcepcionSqlSC)
        {
            return pObjExcepcionSqlSC.Number == gIntErrorRestriccionUnicaSC
                || pObjExcepcionSqlSC.Number == gIntErrorIndiceUnicoSC;
        }
    }
}
