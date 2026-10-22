//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Página de inicio
//==============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion
{
    /// <summary>
    /// Página de inicio: bienvenida, cantidades reales y accesos a los módulos disponibles.
    /// </summary>
    public partial class webInicioSC : Page
    {
        private const int gIntCantidadRecientesSC = 5;

        private readonly ClsClienteLNSC gObjClienteLNSC = new ClsClienteLNSC();
        private readonly ClsVehiculoLNSC gObjVehiculoLNSC = new ClsVehiculoLNSC();

        private webSiteMasterSC gObjSitioSC
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            litFechaHoySC.Text = ClsFormatoSC.FncFormatearFechaLargaSC(ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC());

            // Accesos a los módulos disponibles, excepto Inicio (la página actual).
            rptAccesosSC.DataSource = webSiteMasterSC.FncObtenerModulosDisponiblesSC(Context)
                .Where(pObjModuloSC => !pObjModuloSC.gBlnActivoSC)
                .ToList();
            rptAccesosSC.DataBind();

            SubCargarResumenSC();
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Consulta en AutoManagerDMS las cantidades y los clientes recientes.</summary>
        private void SubCargarResumenSC()
        {
            try
            {
                litTotalClientesSC.Text = gObjClienteLNSC.FncContarClientesSC().ToString();
                litClientesMesSC.Text = gObjClienteLNSC.FncContarClientesDelMesSC().ToString();
                litTotalVehiculosSC.Text = gObjVehiculoLNSC.FncContarVehiculosSC().ToString();

                List<ClsClienteENSC> lLstRecientesSC = gObjClienteLNSC.FncObtenerClientesRecientesSC(gIntCantidadRecientesSC);
                rptRecientesSC.DataSource = lLstRecientesSC;
                rptRecientesSC.DataBind();
                rptRecientesSC.Visible = lLstRecientesSC.Count > 0;
                pnlSinClientesSC.Visible = lLstRecientesSC.Count == 0;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "Default.aspx - cargar resumen");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible cargar el resumen del taller en este momento. Intenta de nuevo en unos minutos.");
                pnlResumenSC.Visible = false;
            }
        }
    }
}
