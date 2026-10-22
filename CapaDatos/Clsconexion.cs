using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidades;

namespace CapaDatos
{
        public class Clsconexion
        {
            /// <summary>
            /// funcion en donde mandamos a llamar a la conexion con la base de datos
            /// </summary>
            public SqlConnection fncobtenerconexionPP()
            {
                SqlConnection gobjconexionPP = new SqlConnection("Data Source=DESKTOP-FEI37DB\\SQLEXPRESS;Initial Catalog=AutoManagerDMS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=true;");
                gobjconexionPP.Open();
                return gobjconexionPP;
            }

            /// <summary>
            /// funcion en donde mandamos a insertar el registro del repuesto
            /// </summary>
            public int fncagregarepuestoPP(ClsCapaEntidades pobjrepuestoPP)
            {
                int lintresultadoPP = 0;
                using(SqlConnection lobjconexionPP = fncobtenerconexionPP())
                {
                    SqlCommand lobjcomandoPP = new SqlCommand(string.Format("insert into dbo.Repuesto ([codigo], [nombre], [descripcion], [stock], [marcacarro], [modelocarro], [aniocarro]) VALUES ('{0}', '{1}', '{2}', {3},'{4}','{5}',{6})",
                                                                            pobjrepuestoPP.gstrcodrepuestoPP, pobjrepuestoPP.gstrnomrepuestoPP, pobjrepuestoPP.gstrdescrepuestoPP, pobjrepuestoPP.gintstockrepuestoPP, pobjrepuestoPP.gstrmarcacarroPP,
                                                                            pobjrepuestoPP.gstrmodelocarroPP, pobjrepuestoPP.gintaniocarroPP), lobjconexionPP);
                    lintresultadoPP = lobjcomandoPP.ExecuteNonQuery();
                }

                return lintresultadoPP;
            }

            /// <summary>
            /// funcion en donde mandamos a consultamos todos los repuestos en la tabla
            /// </summary>
            public DataTable fncconsultarPP(string pstrcriterioPP)
            {
                DataTable ldttinformacionPP = new DataTable();
                using (SqlConnection lobjconnecPP = fncobtenerconexionPP())
                {

                    string query = "SELECT * FROM dbo.Repuesto WHERE nombre LIKE @criterio OR codigo = @codigoExacto";

                    SqlCommand lobjcommandPP = new SqlCommand(query, lobjconnecPP);
                    lobjcommandPP.Parameters.AddWithValue("@criterio", "%" + pstrcriterioPP + "%");
                    lobjcommandPP.Parameters.AddWithValue("@codigoExacto", pstrcriterioPP);

                    SqlDataAdapter lobjadapterPP = new SqlDataAdapter();
                    lobjadapterPP.SelectCommand = lobjcommandPP;
                    lobjadapterPP.Fill(ldttinformacionPP);
                }
                return ldttinformacionPP;
            }

            public int fncactualizarstockPP(string pstrcodigoPP, int pintnuevostockPP)
            {
                int lintresultadoPP = 0;
                using(SqlConnection lobjconexionPP = fncobtenerconexionPP())
                {
                    string pobjprocesoPP = "UPDATE dbo.Repuesto SET [stock] = [stock] + @stock WHERE [codigo] = @codigo";
                    using(SqlCommand lobjcommandPP = new SqlCommand(pobjprocesoPP, lobjconexionPP))
                    {
                        lobjcommandPP.Parameters.AddWithValue("@stock", pintnuevostockPP);
                        lobjcommandPP.Parameters.AddWithValue("@codigo", pstrcodigoPP);
                        lintresultadoPP = lobjcommandPP.ExecuteNonQuery();
                    }
                }
                return lintresultadoPP;
            }
        }
}
