//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Prueba de conexión a SQL Server
//==============================================================
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace PruebaConexionSQL
{
    /// <summary>
    /// Página mínima que comprueba la conexión a TallerMecanicoDB: muestra la versión del
    /// servidor, la base y el usuario, y ejecuta una consulta parametrizada de prueba.
    /// </summary>
    public partial class webPruebaConexionSC : Page
    {
        private const string gStrNombreConexionSC = "TallerMecanicoDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectionStringSettings lObjConfiguracionSC = ConfigurationManager.ConnectionStrings[gStrNombreConexionSC];
            if (lObjConfiguracionSC == null)
            {
                SubMostrarErrorSC($"No existe la cadena de conexión '{gStrNombreConexionSC}' en Web.config.");
                return;
            }

            // Solo se muestran datos no sensibles de la cadena de conexión.
            SqlConnectionStringBuilder lObjDatosConexionSC = new SqlConnectionStringBuilder(lObjConfiguracionSC.ConnectionString);
            lblServidor.Text = Server.HtmlEncode(lObjDatosConexionSC.DataSource);

            try
            {
                using (SqlConnection lObjConexionSC = new SqlConnection(lObjConfiguracionSC.ConnectionString))
                {
                    lObjConexionSC.Open();
                    SubLeerDatosServidorSC(lObjConexionSC);
                    lblExisteBase.Text = FncExisteBaseDatosSC(lObjConexionSC, "TallerMecanicoDB") ? "Sí" : "No";
                    lblConsulta.Text = Convert.ToString(FncSumarEnServidorSC(lObjConexionSC, 2, 3));
                }

                lblEstado.Text = "CONEXION_OK";
            }
            catch (SqlException lObjExcepcionSC)
            {
                SubMostrarErrorSC($"SQL Server respondió con el error {lObjExcepcionSC.Number}: {lObjExcepcionSC.Message}");
            }
            catch (Exception lObjExcepcionSC)
            {
                SubMostrarErrorSC($"{lObjExcepcionSC.GetType().Name}: {lObjExcepcionSC.Message}");
            }
        }

        /// <summary>Muestra la versión, la edición, la base de datos y el usuario de la conexión.</summary>
        private void SubLeerDatosServidorSC(SqlConnection pObjConexionSC)
        {
            const string lStrConsultaSC =
                "SELECT @@VERSION, CAST(SERVERPROPERTY('Edition') AS nvarchar(128)), DB_NAME(), SUSER_SNAME();";

            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, pObjConexionSC))
            using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
            {
                if (lObjLectorSC.Read())
                {
                    lblVersion.Text = Server.HtmlEncode(lObjLectorSC.GetString(0).Split('\n')[0].Trim());
                    lblEdicion.Text = Server.HtmlEncode(lObjLectorSC.GetString(1));
                    lblBaseDatos.Text = Server.HtmlEncode(lObjLectorSC.GetString(2));
                    lblUsuario.Text = Server.HtmlEncode(lObjLectorSC.GetString(3));
                }
            }
        }

        /// <summary>Indica si existe una base de datos con el nombre indicado.</summary>
        private static bool FncExisteBaseDatosSC(SqlConnection pObjConexionSC, string pStrNombreSC)
        {
            using (SqlCommand lObjComandoSC = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @nombre;", pObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@nombre", SqlDbType.NVarChar, 128).Value = pStrNombreSC;
                return (int)lObjComandoSC.ExecuteScalar() == 1;
            }
        }

        /// <summary>Suma dos números en SQL Server con una consulta parametrizada.</summary>
        private static int FncSumarEnServidorSC(SqlConnection pObjConexionSC, int pIntPrimeroSC, int pIntSegundoSC)
        {
            using (SqlCommand lObjComandoSC = new SqlCommand("SELECT @a + @b;", pObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@a", SqlDbType.Int).Value = pIntPrimeroSC;
                lObjComandoSC.Parameters.Add("@b", SqlDbType.Int).Value = pIntSegundoSC;
                return (int)lObjComandoSC.ExecuteScalar();
            }
        }

        /// <summary>Marca la prueba como fallida y muestra el mensaje codificado.</summary>
        private void SubMostrarErrorSC(string pStrMensajeSC)
        {
            lblEstado.Text = "CONEXION_FALLIDA";
            lblError.Text = Server.HtmlEncode(pStrMensajeSC);
        }
    }
}
