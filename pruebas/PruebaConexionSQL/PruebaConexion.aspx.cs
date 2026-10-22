using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace PruebaConexionSQL
{
    public partial class PruebaConexion : Page
    {
        private const string NombreConexion = "TallerMecanicoDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            var configuracion = ConfigurationManager.ConnectionStrings[NombreConexion];
            if (configuracion == null)
            {
                MostrarError($"No existe la cadena de conexión '{NombreConexion}' en Web.config.");
                return;
            }

            // Solo se muestran datos no sensibles de la cadena de conexión.
            var datosConexion = new SqlConnectionStringBuilder(configuracion.ConnectionString);
            lblServidor.Text = Server.HtmlEncode(datosConexion.DataSource);

            try
            {
                using (var conexion = new SqlConnection(configuracion.ConnectionString))
                {
                    conexion.Open();
                    LeerDatosServidor(conexion);
                    lblExisteBase.Text = ExisteBaseDatos(conexion, "TallerMecanicoDB") ? "Sí" : "No";
                    lblConsulta.Text = Convert.ToString(SumarEnServidor(conexion, 2, 3));
                }

                lblEstado.Text = "CONEXION_OK";
            }
            catch (SqlException ex)
            {
                MostrarError($"SQL Server respondió con el error {ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarError($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        private void LeerDatosServidor(SqlConnection conexion)
        {
            const string consulta =
                "SELECT @@VERSION, CAST(SERVERPROPERTY('Edition') AS nvarchar(128)), DB_NAME(), SUSER_SNAME();";

            using (var comando = new SqlCommand(consulta, conexion))
            using (var lector = comando.ExecuteReader())
            {
                if (lector.Read())
                {
                    lblVersion.Text = Server.HtmlEncode(lector.GetString(0).Split('\n')[0].Trim());
                    lblEdicion.Text = Server.HtmlEncode(lector.GetString(1));
                    lblBaseDatos.Text = Server.HtmlEncode(lector.GetString(2));
                    lblUsuario.Text = Server.HtmlEncode(lector.GetString(3));
                }
            }
        }

        private static bool ExisteBaseDatos(SqlConnection conexion, string nombre)
        {
            using (var comando = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @nombre;", conexion))
            {
                comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 128).Value = nombre;
                return (int)comando.ExecuteScalar() == 1;
            }
        }

        private static int SumarEnServidor(SqlConnection conexion, int a, int b)
        {
            using (var comando = new SqlCommand("SELECT @a + @b;", conexion))
            {
                comando.Parameters.Add("@a", SqlDbType.Int).Value = a;
                comando.Parameters.Add("@b", SqlDbType.Int).Value = b;
                return (int)comando.ExecuteScalar();
            }
        }

        private void MostrarError(string mensaje)
        {
            lblEstado.Text = "CONEXION_FALLIDA";
            lblError.Text = Server.HtmlEncode(mensaje);
        }
    }
}
