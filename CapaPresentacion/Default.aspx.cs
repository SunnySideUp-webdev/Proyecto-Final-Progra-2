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
        private const int lIntCantidadRecientesSC = 5;

        private readonly ClsClienteLNSC lObjClienteLNSC = new ClsClienteLNSC();
        private readonly ClsVehiculoLNSC lObjVehiculoLNSC = new ClsVehiculoLNSC();

        private SiteMaster lObjSitioSC
        {
            get { return (SiteMaster)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            litFechaHoySC.Text = ClsFormatoSC.fncFormatearFechaLargaSC(ClsZonaHorariaLN.fncObtenerHoyGuatemala());

            // Accesos a los módulos disponibles, excepto Inicio (la página actual).
            rptAccesosSC.DataSource = SiteMaster.fncObtenerModulosDisponibles(Context)
                .Where(m => !m.lBlnActivo)
                .ToList();
            rptAccesosSC.DataBind();

            fncCargarResumenSC();
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string fncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Consulta en AutoManagerDMS las cantidades y los clientes recientes.</summary>
        private void fncCargarResumenSC()
        {
            try
            {
                litTotalClientesSC.Text = lObjClienteLNSC.fncContarClientesSC().ToString();
                litClientesMesSC.Text = lObjClienteLNSC.fncContarClientesDelMesSC().ToString();
                litTotalVehiculosSC.Text = lObjVehiculoLNSC.fncContarVehiculosSC().ToString();

                List<ClsClienteENSC> lLstRecientesSC = lObjClienteLNSC.fncObtenerClientesRecientesSC(lIntCantidadRecientesSC);
                rptRecientesSC.DataSource = lLstRecientesSC;
                rptRecientesSC.DataBind();
                rptRecientesSC.Visible = lLstRecientesSC.Count > 0;
                pnlSinClientesSC.Visible = lLstRecientesSC.Count == 0;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "Default.aspx - cargar resumen");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible cargar el resumen del taller en este momento. Intenta de nuevo en unos minutos.");
                pnlResumenSC.Visible = false;
            }
        }
    }
}
