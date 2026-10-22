//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Capa de Datos
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>Ejecuta las operaciones parametrizadas de la tabla dbo.Clientes.</summary>
    public class ClsClienteDASC
    {
        private const string gStrColumnasSC =
            "IdCliente, DPI, Nombres, Apellidos, Telefono, Correo, Direccion, FechaRegistro";

        /// <summary>Comprueba si un DPI ya pertenece a otro cliente.</summary>
        /// <param name="pStrDpiSC">DPI normalizado de 13 dígitos.</param>
        /// <returns><c>true</c> si el DPI ya está registrado.</returns>
        public bool FncExisteDpiSC(string pStrDpiSC)
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Clientes WHERE DPI = @DPI;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@DPI", SqlDbType.Char, 13).Value = pStrDpiSC;
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar()) > 0;
            }
        }

        /// <summary>Inserta un cliente validado y devuelve el identificador generado.</summary>
        /// <param name="pObjClienteSC">Cliente validado y normalizado por la Capa Lógica.</param>
        /// <returns>Identificador asignado por SQL Server.</returns>
        /// <exception cref="ClsDatoDuplicadoExcepcionDASC">El DPI ya existe (restricción UQ_Clientes_DPI).</exception>
        public int FncInsertarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            const string lStrConsultaSC = @"
                INSERT INTO dbo.Clientes (DPI, Nombres, Apellidos, Telefono, Correo, Direccion)
                OUTPUT INSERTED.IdCliente
                VALUES (@DPI, @Nombres, @Apellidos, @Telefono, @Correo, @Direccion);";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@DPI", SqlDbType.Char, 13).Value = pObjClienteSC.gStrDpiSC;
                lObjComandoSC.Parameters.Add("@Nombres", SqlDbType.NVarChar, 80).Value = pObjClienteSC.gStrNombresSC;
                lObjComandoSC.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 80).Value = pObjClienteSC.gStrApellidosSC;
                lObjComandoSC.Parameters.Add("@Telefono", SqlDbType.VarChar, 15).Value = pObjClienteSC.gStrTelefonoSC;
                lObjComandoSC.Parameters.Add("@Correo", SqlDbType.NVarChar, 254).Value =
                    (object)pObjClienteSC.gStrCorreoSC ?? DBNull.Value;
                lObjComandoSC.Parameters.Add("@Direccion", SqlDbType.NVarChar, 200).Value =
                    (object)pObjClienteSC.gStrDireccionSC ?? DBNull.Value;

                try
                {
                    lObjConexionSC.Open();
                    return Convert.ToInt32(lObjComandoSC.ExecuteScalar());
                }
                catch (SqlException lObjExcepcionSC) when (ClsDatoDuplicadoExcepcionDASC.FncEsDuplicadoSC(lObjExcepcionSC))
                {
                    throw new ClsDatoDuplicadoExcepcionDASC(lObjExcepcionSC);
                }
            }
        }

        /// <summary>
        /// Busca clientes cuyo DPI, teléfono, nombres o apellidos contengan cada una de
        /// las palabras indicadas. Sin palabras devuelve los registrados más recientemente.
        /// </summary>
        /// <param name="pLstPalabrasSC">Palabras normalizadas por la Capa Lógica.</param>
        /// <param name="pIntMaximoSC">Cantidad máxima de filas a devolver.</param>
        /// <returns>Clientes que coinciden con todas las palabras.</returns>
        public List<ClsClienteENSC> FncBuscarClientesSC(IList<string> pLstPalabrasSC, int pIntMaximoSC)
        {
            List<ClsClienteENSC> lLstClientesSC = new List<ClsClienteENSC>();

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand())
            {
                // Solo los nombres de parámetro (@Palabra0, @Palabra1...) se generan en el código;
                // el texto del usuario viaja siempre como valor de un parámetro.
                // COLLATE ..._CI_AI permite encontrar "Pérez" escribiendo "perez".
                // ESCAPE '\' hace que %, _ y [ escritos por el usuario se busquen literalmente.
                StringBuilder lSbCondicionesSC = new StringBuilder();
                for (int lIntIndiceSC = 0; lIntIndiceSC < pLstPalabrasSC.Count; lIntIndiceSC++)
                {
                    string lStrParametroSC = "@Palabra" + lIntIndiceSC;
                    lSbCondicionesSC
                        .Append(lIntIndiceSC == 0 ? " WHERE " : " AND ")
                        .Append("(DPI LIKE ").Append(lStrParametroSC).Append(@" ESCAPE '\'")
                        .Append(" OR Telefono LIKE ").Append(lStrParametroSC).Append(@" ESCAPE '\'")
                        .Append(" OR (Nombres + N' ' + Apellidos) COLLATE Latin1_General_CI_AI LIKE ")
                        .Append(lStrParametroSC).Append(@" ESCAPE '\')");
                    lObjComandoSC.Parameters.Add(lStrParametroSC, SqlDbType.NVarChar, 200).Value =
                        "%" + ClsSqlDASC.FncEscaparPatronLikeSC(pLstPalabrasSC[lIntIndiceSC]) + "%";
                }

                lObjComandoSC.CommandText =
                    "SELECT TOP (@Maximo) " + gStrColumnasSC + " FROM dbo.Clientes" + lSbCondicionesSC +
                    (pLstPalabrasSC.Count == 0 ? " ORDER BY IdCliente DESC;" : " ORDER BY Apellidos, Nombres;");
                lObjComandoSC.Connection = lObjConexionSC;
                lObjComandoSC.Parameters.Add("@Maximo", SqlDbType.Int).Value = pIntMaximoSC;

                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    while (lObjLectorSC.Read())
                    {
                        lLstClientesSC.Add(FncLeerClienteSC(lObjLectorSC));
                    }
                }
            }

            return lLstClientesSC;
        }

        /// <summary>Obtiene un cliente por su identificador.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente.</param>
        /// <returns>El cliente encontrado o <c>null</c> si no existe.</returns>
        public ClsClienteENSC FncObtenerClientePorIdSC(int pIntIdClienteSC)
        {
            const string lStrConsultaSC =
                "SELECT " + gStrColumnasSC + " FROM dbo.Clientes WHERE IdCliente = @IdCliente;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@IdCliente", SqlDbType.Int).Value = pIntIdClienteSC;
                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    return lObjLectorSC.Read() ? FncLeerClienteSC(lObjLectorSC) : null;
                }
            }
        }

        /// <summary>Cuenta todos los clientes registrados.</summary>
        /// <returns>Cantidad total de clientes.</returns>
        public int FncContarClientesSC()
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Clientes;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar());
            }
        }

        /// <summary>Cuenta los clientes registrados a partir de una fecha.</summary>
        /// <param name="pDatDesdeSC">Fecha y hora inicial en UTC (incluida).</param>
        /// <returns>Cantidad de clientes registrados desde esa fecha.</returns>
        public int FncContarClientesDesdeSC(DateTime pDatDesdeSC)
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Clientes WHERE FechaRegistro >= @Desde;";

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = pDatDesdeSC;
                lObjConexionSC.Open();
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar());
            }
        }

        /// <summary>Convierte la fila actual del lector en una entidad.</summary>
        private static ClsClienteENSC FncLeerClienteSC(SqlDataReader pObjLectorSC)
        {
            return new ClsClienteENSC
            {
                gIntIdClienteSC = pObjLectorSC.GetInt32(0),
                gStrDpiSC = pObjLectorSC.GetString(1),
                gStrNombresSC = pObjLectorSC.GetString(2),
                gStrApellidosSC = pObjLectorSC.GetString(3),
                gStrTelefonoSC = pObjLectorSC.GetString(4),
                gStrCorreoSC = pObjLectorSC.IsDBNull(5) ? null : pObjLectorSC.GetString(5),
                gStrDireccionSC = pObjLectorSC.IsDBNull(6) ? null : pObjLectorSC.GetString(6),
                // SQL Server devuelve DATETIME2 sin zona; la columna se guarda en UTC.
                gDatFechaRegistroUtcSC = DateTime.SpecifyKind(pObjLectorSC.GetDateTime(7), DateTimeKind.Utc)
            };
        }
    }
}
