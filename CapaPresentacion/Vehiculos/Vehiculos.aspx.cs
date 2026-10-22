//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Listado y búsqueda de vehículos
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Vehiculos
{
    /// <summary>Lista los vehículos y permite buscarlos por placa o VIN.</summary>
    public partial class webVehiculosSC : Page
    {
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

            string lStrTerminoSC = Request.QueryString["q"] ?? string.Empty;
            txtBusquedaSC.Text = lStrTerminoSC;
            fncCargarVehiculosSC(lStrTerminoSC);
        }

        /// <summary>Lleva la búsqueda a la dirección de la página (?q=) para poder recargarla o compartirla.</summary>
        protected void btnBuscarSC_Click(object sender, EventArgs e)
        {
            string lStrTerminoSC = txtBusquedaSC.Text.Trim();
            string lStrDestinoSC = lStrTerminoSC.Length == 0
                ? "~/Vehiculos/Vehiculos.aspx"
                : "~/Vehiculos/Vehiculos.aspx?q=" + HttpUtility.UrlEncode(lStrTerminoSC);

            Response.Redirect(lStrDestinoSC, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string fncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Consulta los vehículos en AutoManagerDMS y actualiza la tabla.</summary>
        private void fncCargarVehiculosSC(string pStrTerminoSC)
        {
            bool lBlnBuscandoSC = !string.IsNullOrWhiteSpace(pStrTerminoSC);
            lnkLimpiarSC.Visible = lBlnBuscandoSC;

            try
            {
                int lIntTotalSC = lObjVehiculoLNSC.fncContarVehiculosSC();
                litTotalSC.Text = lIntTotalSC == 1 ? "1 vehículo registrado" : lIntTotalSC + " vehículos registrados";

                List<ClsVehiculoENSC> lLstVehiculosSC = lObjVehiculoLNSC.fncBuscarVehiculosSC(pStrTerminoSC);
                rptVehiculosSC.DataSource = lLstVehiculosSC;
                rptVehiculosSC.DataBind();
                rptVehiculosSC.Visible = lLstVehiculosSC.Count > 0;
                pnlVacioSC.Visible = lLstVehiculosSC.Count == 0;

                bool lBlnLimiteSC = lLstVehiculosSC.Count == ClsVehiculoLNSC.lIntMaximoResultadosSC;
                if (lBlnBuscandoSC)
                {
                    litResumenSC.Text = (lLstVehiculosSC.Count == 1 ? "1 resultado" : lLstVehiculosSC.Count + " resultados")
                        + " para «" + pStrTerminoSC.Trim() + "»";
                    litLimiteSC.Text = lBlnLimiteSC
                        ? "Se muestran los primeros " + ClsVehiculoLNSC.lIntMaximoResultadosSC + "; escribe más caracteres para afinar la búsqueda."
                        : "Coincidencias en la placa o el VIN; las exactas aparecen primero.";
                    litVacioTituloSC.Text = "No se encontraron vehículos";
                    litVacioDetalleSC.Text = "Revisa la placa o el VIN escritos.";
                }
                else
                {
                    litResumenSC.Text = "Registrados recientemente";
                    litLimiteSC.Text = lIntTotalSC > ClsVehiculoLNSC.lIntMaximoResultadosSC
                        ? "Se muestran los " + ClsVehiculoLNSC.lIntMaximoResultadosSC + " más recientes."
                        : "Ordenados del más reciente al más antiguo.";
                    litVacioTituloSC.Text = "Todavía no hay vehículos registrados";
                    litVacioDetalleSC.Text = "Usa el botón «Registrar vehículo» para agregar el primero.";
                }
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionSC)
            {
                string lStrMensajeSC;
                lObjExcepcionSC.lDicErroresCampos.TryGetValue(ClsVehiculoLNSC.lStrCampoBusquedaSC, out lStrMensajeSC);
                lblErrorBusquedaSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC ?? "Revisa el texto de búsqueda.");
                lblErrorBusquedaSC.Visible = true;
                txtBusquedaSC.CssClass = "form-control is-invalid";
                pnlResultadosSC.Visible = false;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "Vehiculos.aspx - cargar vehículos");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible consultar los vehículos en este momento. Intenta de nuevo en unos minutos.");
                litTotalSC.Text = string.Empty;
                pnlResultadosSC.Visible = false;
            }
        }
    }
}
