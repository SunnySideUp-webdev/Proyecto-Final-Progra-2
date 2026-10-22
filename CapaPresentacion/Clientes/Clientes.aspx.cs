//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Listado y búsqueda de clientes
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Clientes
{
    /// <summary>Lista los clientes y permite buscarlos por DPI, nombre o teléfono.</summary>
    public partial class webClientesSC : Page
    {
        private readonly ClsClienteLNSC gObjClienteLNSC = new ClsClienteLNSC();

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

            string lStrTerminoSC = Request.QueryString["q"] ?? string.Empty;
            txtBusquedaSC.Text = lStrTerminoSC;
            SubCargarClientesSC(lStrTerminoSC);
        }

        /// <summary>Lleva la búsqueda a la dirección de la página (?q=) para poder recargarla o compartirla.</summary>
        protected void btnBuscarSC_Click(object sender, EventArgs e)
        {
            string lStrTerminoSC = txtBusquedaSC.Text.Trim();
            string lStrDestinoSC = lStrTerminoSC.Length == 0
                ? "~/Clientes/Clientes.aspx"
                : "~/Clientes/Clientes.aspx?q=" + HttpUtility.UrlEncode(lStrTerminoSC);

            Response.Redirect(lStrDestinoSC, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Consulta los clientes en AutoManagerDMS y actualiza la tabla.</summary>
        private void SubCargarClientesSC(string pStrTerminoSC)
        {
            bool lBlnBuscandoSC = !string.IsNullOrWhiteSpace(pStrTerminoSC);
            lnkLimpiarSC.Visible = lBlnBuscandoSC;

            try
            {
                int lIntTotalSC = gObjClienteLNSC.FncContarClientesSC();
                litTotalSC.Text = lIntTotalSC == 1 ? "1 cliente registrado" : lIntTotalSC + " clientes registrados";

                List<ClsClienteENSC> lLstClientesSC = gObjClienteLNSC.FncBuscarClientesSC(pStrTerminoSC);
                rptClientesSC.DataSource = lLstClientesSC;
                rptClientesSC.DataBind();
                rptClientesSC.Visible = lLstClientesSC.Count > 0;
                pnlVacioSC.Visible = lLstClientesSC.Count == 0;

                bool lBlnLimiteSC = lLstClientesSC.Count == ClsClienteLNSC.gIntMaximoResultadosSC;
                if (lBlnBuscandoSC)
                {
                    litResumenSC.Text = (lLstClientesSC.Count == 1 ? "1 resultado" : lLstClientesSC.Count + " resultados")
                        + " para «" + pStrTerminoSC.Trim() + "»";
                    litLimiteSC.Text = lBlnLimiteSC
                        ? "Se muestran los primeros " + ClsClienteLNSC.gIntMaximoResultadosSC + "; escribe más datos para afinar la búsqueda."
                        : "Coincidencias en DPI, nombres, apellidos o teléfono.";
                    litVacioTituloSC.Text = "No se encontraron clientes";
                    litVacioDetalleSC.Text = "Revisa el texto o busca por otro dato, como el DPI o el teléfono.";
                }
                else
                {
                    litResumenSC.Text = "Registrados recientemente";
                    litLimiteSC.Text = lIntTotalSC > ClsClienteLNSC.gIntMaximoResultadosSC
                        ? "Se muestran los " + ClsClienteLNSC.gIntMaximoResultadosSC + " más recientes."
                        : "Ordenados del más reciente al más antiguo.";
                    litVacioTituloSC.Text = "Todavía no hay clientes registrados";
                    litVacioDetalleSC.Text = "Usa el botón «Registrar cliente» para agregar el primero.";
                }
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                string lStrMensajeSC;
                lObjExcepcionSC.gDicErroresCamposSC.TryGetValue(ClsClienteLNSC.gStrCampoBusquedaSC, out lStrMensajeSC);
                lblErrorBusquedaSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC ?? "Revisa el texto de búsqueda.");
                lblErrorBusquedaSC.Visible = true;
                txtBusquedaSC.CssClass = "form-control is-invalid";
                pnlResultadosSC.Visible = false;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "Clientes.aspx - cargar clientes");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible consultar los clientes en este momento. Intenta de nuevo en unos minutos.");
                litTotalSC.Text = string.Empty;
                pnlResultadosSC.Visible = false;
            }
        }
    }
}
