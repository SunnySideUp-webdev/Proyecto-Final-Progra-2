//==============================================================
// Nombre: Pavel Alexander Palma Carrillo
// Carné: 0900-24-4296
// Tema: AutoManager DMS - Módulo 3 Inventario - Capa de Datos
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>
    /// Ejecuta las operaciones de la tabla dbo.Repuesto: registrar un repuesto, consultarlo
    /// y sumar unidades a su stock. Todas las consultas usan parámetros.
    /// </summary>
    public class ClsRepuestoDAPP
    {
        /// <summary>Columnas que se leen de cada repuesto, en el orden que espera FncLeerRepuestoPP.</summary>
        private const string gStrColumnasPP =
            "idrepuesto, codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro";

        /// <summary>Comprueba si un código ya está registrado.</summary>
        /// <param name="pStrCodigoPP">Código normalizado (mayúsculas).</param>
        /// <returns><c>true</c> si el código existe.</returns>
        public bool FncExisteCodigoPP(string pStrCodigoPP)
        {
            const string lStrConsultaPP = "SELECT COUNT(1) FROM dbo.Repuesto WHERE codigo = @Codigo;";

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                lObjComandoPP.Parameters.Add("@Codigo", SqlDbType.VarChar, 15).Value = pStrCodigoPP;
                lObjConexionPP.Open();
                return Convert.ToInt32(lObjComandoPP.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Inserta un repuesto validado y devuelve el identificador generado. Los datos viajan
        /// como parámetros: el texto del usuario nunca forma parte de la instrucción SQL.
        /// </summary>
        /// <param name="pObjRepuestoPP">Repuesto validado y normalizado por la Capa Lógica.</param>
        /// <returns>Identificador asignado por SQL Server.</returns>
        /// <exception cref="ClsDatoDuplicadoExcepcionDASC">El código ya existe (UQ_Repuesto_Codigo).</exception>
        public int FncAgregarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP)
        {
            const string lStrConsultaPP = @"
                INSERT INTO dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
                OUTPUT INSERTED.idrepuesto
                VALUES (@Codigo, @Nombre, @Descripcion, @Stock, @Marca, @Modelo, @Anio);";

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                lObjComandoPP.Parameters.Add("@Codigo", SqlDbType.VarChar, 15).Value = pObjRepuestoPP.gStrCodigoPP;
                lObjComandoPP.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = pObjRepuestoPP.gStrNombrePP;
                lObjComandoPP.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value = pObjRepuestoPP.gStrDescripcionPP;
                lObjComandoPP.Parameters.Add("@Stock", SqlDbType.Int).Value = pObjRepuestoPP.gIntStockPP;
                lObjComandoPP.Parameters.Add("@Marca", SqlDbType.NVarChar, 50).Value = pObjRepuestoPP.gStrMarcaCarroPP;
                lObjComandoPP.Parameters.Add("@Modelo", SqlDbType.NVarChar, 50).Value = pObjRepuestoPP.gStrModeloCarroPP;
                lObjComandoPP.Parameters.Add("@Anio", SqlDbType.Int).Value = pObjRepuestoPP.gIntAnioCarroPP;

                try
                {
                    lObjConexionPP.Open();
                    return Convert.ToInt32(lObjComandoPP.ExecuteScalar());
                }
                catch (SqlException lObjExcepcionPP) when (ClsDatoDuplicadoExcepcionDASC.FncEsDuplicadoSC(lObjExcepcionPP))
                {
                    throw new ClsDatoDuplicadoExcepcionDASC(lObjExcepcionPP);
                }
            }
        }

        /// <summary>
        /// Busca repuestos cuyo nombre contenga el texto o cuyo código sea exactamente ese texto.
        /// Sin texto devuelve los registrados más recientemente.
        /// </summary>
        /// <param name="pStrCriterioPP">Nombre o código, o <c>null</c> para listar los más recientes.</param>
        /// <param name="pIntMaximoPP">Cantidad máxima de filas a devolver.</param>
        /// <returns>Repuestos encontrados.</returns>
        public List<ClsRepuestoENPP> FncConsultarPP(string pStrCriterioPP, int pIntMaximoPP)
        {
            // COLLATE ..._CI_AI permite encontrar "Batería" escribiendo "bateria".
            const string lStrConsultaPP = "SELECT TOP (@Maximo) " + gStrColumnasPP + @"
                FROM dbo.Repuesto
                WHERE @Patron IS NULL
                   OR nombre COLLATE Latin1_General_CI_AI LIKE @Patron ESCAPE '\'
                   OR codigo = @Codigo
                ORDER BY CASE WHEN @Patron IS NULL THEN idrepuesto END DESC,
                         CASE WHEN codigo = @Codigo THEN 0 ELSE 1 END,
                         nombre;";

            List<ClsRepuestoENPP> lLstRepuestosPP = new List<ClsRepuestoENPP>();

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                bool lBlnSinCriterioPP = string.IsNullOrEmpty(pStrCriterioPP);
                lObjComandoPP.Parameters.Add("@Maximo", SqlDbType.Int).Value = pIntMaximoPP;
                // Los comodines de LIKE (%, _ y [) se escapan para buscar el texto tal cual.
                lObjComandoPP.Parameters.Add("@Patron", SqlDbType.NVarChar, 120).Value = lBlnSinCriterioPP
                    ? (object)DBNull.Value
                    : "%" + ClsSqlDASC.FncEscaparPatronLikeSC(pStrCriterioPP) + "%";
                lObjComandoPP.Parameters.Add("@Codigo", SqlDbType.VarChar, 50).Value = lBlnSinCriterioPP
                    ? (object)DBNull.Value
                    : pStrCriterioPP.ToUpperInvariant();

                lObjConexionPP.Open();
                using (SqlDataReader lObjLectorPP = lObjComandoPP.ExecuteReader())
                {
                    while (lObjLectorPP.Read())
                    {
                        lLstRepuestosPP.Add(FncLeerRepuestoPP(lObjLectorPP));
                    }
                }
            }

            return lLstRepuestosPP;
        }

        /// <summary>Obtiene un repuesto por su identificador.</summary>
        /// <param name="pIntIdRepuestoPP">Identificador del repuesto.</param>
        /// <returns>El repuesto o <c>null</c> si no existe.</returns>
        public ClsRepuestoENPP FncObtenerRepuestoPP(int pIntIdRepuestoPP)
        {
            const string lStrConsultaPP = "SELECT " + gStrColumnasPP + " FROM dbo.Repuesto WHERE idrepuesto = @IdRepuesto;";

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                lObjComandoPP.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = pIntIdRepuestoPP;
                lObjConexionPP.Open();
                using (SqlDataReader lObjLectorPP = lObjComandoPP.ExecuteReader())
                {
                    return lObjLectorPP.Read() ? FncLeerRepuestoPP(lObjLectorPP) : null;
                }
            }
        }

        /// <summary>
        /// Suma unidades al stock de un repuesto (entrada a bodega). La suma la hace SQL Server
        /// sobre el valor actual, así dos entradas simultáneas no se pisan.
        /// </summary>
        /// <param name="pIntIdRepuestoPP">Identificador del repuesto.</param>
        /// <param name="pIntCantidadPP">Unidades que ingresan (mayor que cero).</param>
        /// <returns><c>true</c> si el repuesto existía y se actualizó.</returns>
        public bool FncActualizarStockPP(int pIntIdRepuestoPP, int pIntCantidadPP)
        {
            const string lStrConsultaPP = "UPDATE dbo.Repuesto SET stock = stock + @Cantidad WHERE idrepuesto = @IdRepuesto;";

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                lObjComandoPP.Parameters.Add("@Cantidad", SqlDbType.Int).Value = pIntCantidadPP;
                lObjComandoPP.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = pIntIdRepuestoPP;
                lObjConexionPP.Open();
                return lObjComandoPP.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>Cuenta todos los repuestos registrados.</summary>
        /// <returns>Cantidad total de repuestos.</returns>
        public int FncContarRepuestosPP()
        {
            const string lStrConsultaPP = "SELECT COUNT(1) FROM dbo.Repuesto;";

            using (SqlConnection lObjConexionPP = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoPP = new SqlCommand(lStrConsultaPP, lObjConexionPP))
            {
                lObjConexionPP.Open();
                return Convert.ToInt32(lObjComandoPP.ExecuteScalar());
            }
        }

        /// <summary>Convierte la fila actual en un repuesto (columnas de gStrColumnasPP).</summary>
        private static ClsRepuestoENPP FncLeerRepuestoPP(SqlDataReader pObjLectorPP)
        {
            return new ClsRepuestoENPP
            {
                gIntIdRepuestoPP = pObjLectorPP.GetInt32(0),
                gStrCodigoPP = pObjLectorPP.GetString(1),
                gStrNombrePP = pObjLectorPP.GetString(2),
                gStrDescripcionPP = pObjLectorPP.GetString(3),
                gIntStockPP = pObjLectorPP.GetInt32(4),
                gStrMarcaCarroPP = pObjLectorPP.GetString(5),
                gStrModeloCarroPP = pObjLectorPP.GetString(6),
                gIntAnioCarroPP = pObjLectorPP.GetInt32(7)
            };
        }
    }
}
