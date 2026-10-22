//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Repuestos utilizados en órdenes - Capa de Presentación
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.OrdenesReparacion
{
    /// <summary>
    /// Muestra y registra los repuestos utilizados en una orden (?idOrden=). Mientras la orden
    /// está abierta se pueden agregar repuestos (se descuentan del inventario) o quitarlos
    /// (sus unidades vuelven al inventario); cerrada, la lista es de solo lectura.
    /// </summary>
    public partial class webRepuestosOrdenSC : Page
    {
        private const string lStrClaseCampoSC = "form-control adms-numero";

        private readonly ClsRepuestoOrdenLNSC lObjRepuestoOrdenLNSC = new ClsRepuestoOrdenLNSC();
        private readonly ClsOrdenReparacionLNT lObjOrdenLNSC = new ClsOrdenReparacionLNT();
        private readonly ClsVehiculoLNSC lObjVehiculoLNSC = new ClsVehiculoLNSC();

        private int lIntIdOrdenSC;

        // Datos de la orden que se muestran con expresiones <%= %> en la página.
        protected string gStrPlacaSC = string.Empty;
        protected string gStrClaseEstadoSC = "adms-estado";
        protected bool gBlnOrdenAbiertaSC;

        private SiteMaster lObjSitioSC
        {
            get { return (SiteMaster)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["idOrden"], out lIntIdOrdenSC) || lIntIdOrdenSC <= 0)
            {
                fncMostrarNoEncontradaSC();
                return;
            }

            if (IsPostBack)
            {
                return;
            }

            if (fncCargarPaginaSC())
            {
                if (Request.QueryString["agregado"] == "1")
                {
                    lObjSitioSC.fncMostrarAlerta(TipoAlerta.Exito, "Se agregó el repuesto a la orden y se descontó del inventario.");
                }
                else if (Request.QueryString["quitado"] == "1")
                {
                    lObjSitioSC.fncMostrarAlerta(TipoAlerta.Exito, "Se quitó el repuesto de la orden y sus unidades volvieron al inventario.");
                }
            }
        }

        /// <summary>Registra el repuesto elegido y lo descuenta del inventario.</summary>
        protected void btnAgregarSC_Click(object sender, EventArgs e)
        {
            if (lIntIdOrdenSC <= 0)
            {
                return;
            }

            fncLimpiarErroresSC();
            string lStrRepuestoElegidoSC = ddlRepuestoSC.SelectedValue;

            try
            {
                lObjRepuestoOrdenLNSC.fncRegistrarRepuestoSC(lIntIdOrdenSC, lStrRepuestoElegidoSC, txtCantidadSC.Text);
                fncRedirigirSC("agregado");
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionSC)
            {
                fncCargarPaginaSC();
                fncSeleccionarRepuestoSC(lStrRepuestoElegidoSC);
                fncMostrarErroresSC(lObjExcepcionSC, "No se agregó el repuesto.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RepuestosOrdenSC.aspx - agregar repuesto");
                fncCargarPaginaSC();
                fncSeleccionarRepuestoSC(lStrRepuestoElegidoSC);
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible agregar el repuesto en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Quita un repuesto de la orden y devuelve sus unidades al inventario.</summary>
        protected void rptUsadosSC_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int lIntIdRepuestoOrdenSC;
            if (lIntIdOrdenSC <= 0 || e.CommandName != "Quitar"
                || !int.TryParse(Convert.ToString(e.CommandArgument), out lIntIdRepuestoOrdenSC))
            {
                return;
            }

            try
            {
                lObjRepuestoOrdenLNSC.fncQuitarRepuestoSC(lIntIdOrdenSC, lIntIdRepuestoOrdenSC);
                fncRedirigirSC("quitado");
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionSC)
            {
                fncCargarPaginaSC();
                fncMostrarErroresSC(lObjExcepcionSC, "No se quitó el repuesto.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RepuestosOrdenSC.aspx - quitar repuesto");
                fncCargarPaginaSC();
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible quitar el repuesto en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string fncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>
        /// Carga la orden, su vehículo, los repuestos utilizados y los disponibles.
        /// </summary>
        /// <returns><c>true</c> si la orden existe y se mostró.</returns>
        private bool fncCargarPaginaSC()
        {
            try
            {
                ClsOrdenReparacionENT lObjOrdenSC = lObjOrdenLNSC.FncObtenerOrdenPorIdNT(lIntIdOrdenSC);
                if (lObjOrdenSC == null)
                {
                    fncMostrarNoEncontradaSC();
                    return false;
                }

                gBlnOrdenAbiertaSC = lObjOrdenSC.lStrEstadoNT == "Abierta";
                Page.Title = "Repuestos de la orden #" + lIntIdOrdenSC;
                string lStrUrlOrdenSC = "~/OrdenesReparacion/DetalleOrdenNT.aspx?id=" + lIntIdOrdenSC;
                lnkVolverOrdenSC.NavigateUrl = lStrUrlOrdenSC;
                lnkRutaOrdenSC.NavigateUrl = lStrUrlOrdenSC;
                lnkRutaOrdenSC.Text = "Orden #" + lIntIdOrdenSC;
                phRutaOrdenSC.Visible = true;

                gStrPlacaSC = ClsFormatoSC.fncFormatearPlacaSC(lObjOrdenSC.lStrPlacaNT);
                gStrClaseEstadoSC = gBlnOrdenAbiertaSC ? "adms-estado adms-estado-abierta" : "adms-estado adms-estado-cerrada";
                litNumeroOrdenSC.Text = lIntIdOrdenSC.ToString();
                litEstadoSC.Text = lObjOrdenSC.lStrEstadoNT;
                litMotivoSC.Text = lObjOrdenSC.lStrMotivoIngresoNT;

                ClsVehiculoENSC lObjVehiculoSC = lObjVehiculoLNSC.fncObtenerVehiculoSC(lObjOrdenSC.lIntIdVehiculoNT);
                litVehiculoSC.Text = lObjVehiculoSC == null
                    ? "Vehículo VIN " + lObjOrdenSC.lStrVinNT
                    : lObjVehiculoSC.lStrMarcaSC + " " + lObjVehiculoSC.lStrModeloSC + " " + lObjVehiculoSC.lIntAnioSC;
                if (lObjVehiculoSC != null && lObjVehiculoSC.lObjPropietarioSC != null)
                {
                    lnkClienteSC.Text = HttpUtility.HtmlEncode(lObjVehiculoSC.lObjPropietarioSC.lStrNombresSC + " "
                        + lObjVehiculoSC.lObjPropietarioSC.lStrApellidosSC);
                    lnkClienteSC.NavigateUrl = "~/Clientes/DetalleCliente.aspx?id=" + lObjVehiculoSC.lIntIdClienteSC;
                    phClienteSC.Visible = true;
                }
                pnlOrdenSC.Visible = true;

                List<ClsRepuestoOrdenENSC> lLstUsosSC = lObjRepuestoOrdenLNSC.fncListarRepuestosDeOrdenSC(lIntIdOrdenSC);
                rptUsadosSC.DataSource = lLstUsosSC;
                rptUsadosSC.DataBind();
                rptUsadosSC.Visible = lLstUsosSC.Count > 0;
                pnlSinUsadosSC.Visible = lLstUsosSC.Count == 0;
                litSinUsadosSC.Text = gBlnOrdenAbiertaSC
                    ? "Usa el formulario de arriba para registrar el primero."
                    : "Esta orden se cerró sin repuestos registrados.";
                int lIntUnidadesSC = 0;
                foreach (ClsRepuestoOrdenENSC lObjUsoSC in lLstUsosSC)
                {
                    lIntUnidadesSC += lObjUsoSC.lIntCantidadSC;
                }
                litTotalSC.Text = lIntUnidadesSC == 1 ? "1 unidad" : lIntUnidadesSC + " unidades";
                pnlUsadosSC.Visible = true;

                pnlCerradaSC.Visible = !gBlnOrdenAbiertaSC;
                pnlAgregarSC.Visible = gBlnOrdenAbiertaSC;
                if (gBlnOrdenAbiertaSC)
                {
                    fncCargarDisponiblesSC();
                }
                return true;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RepuestosOrdenSC.aspx - cargar orden " + lIntIdOrdenSC);
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible cargar los repuestos de la orden en este momento. Intenta de nuevo en unos minutos.");
                return false;
            }
        }

        /// <summary>Llena la lista de repuestos con unidades disponibles.</summary>
        private void fncCargarDisponiblesSC()
        {
            List<ClsRepuestoENPP> lLstDisponiblesSC = lObjRepuestoOrdenLNSC.fncListarRepuestosDisponiblesSC();
            ddlRepuestoSC.Items.Clear();
            ddlRepuestoSC.Items.Add(new ListItem("Selecciona un repuesto", string.Empty));
            foreach (ClsRepuestoENPP lObjRepuestoSC in lLstDisponiblesSC)
            {
                // DropDownList codifica el texto de cada opción al dibujarla.
                ddlRepuestoSC.Items.Add(new ListItem(
                    lObjRepuestoSC.lStrCodigoPP + " · " + lObjRepuestoSC.lStrNombrePP
                        + " (" + lObjRepuestoSC.lStrMarcaCarroPP + " " + lObjRepuestoSC.lStrModeloCarroPP + " " + lObjRepuestoSC.lIntAnioCarroPP + ")"
                        + " · " + (lObjRepuestoSC.lIntStockPP == 1 ? "1 disponible" : lObjRepuestoSC.lIntStockPP + " disponibles"),
                    lObjRepuestoSC.lIntIdRepuestoPP.ToString()));
            }

            bool lBlnHayDisponiblesSC = lLstDisponiblesSC.Count > 0;
            pnlFormularioSC.Visible = lBlnHayDisponiblesSC;
            pnlSinDisponiblesSC.Visible = !lBlnHayDisponiblesSC;
        }

        /// <summary>Vuelve a elegir en la lista el repuesto que el usuario había seleccionado.</summary>
        private void fncSeleccionarRepuestoSC(string pStrIdRepuestoSC)
        {
            ListItem lObjOpcionSC = ddlRepuestoSC.Items.FindByValue(pStrIdRepuestoSC ?? string.Empty);
            if (lObjOpcionSC != null)
            {
                ddlRepuestoSC.ClearSelection();
                lObjOpcionSC.Selected = true;
            }
        }

        /// <summary>Muestra cada error en su campo, o como mensaje general si es de la orden.</summary>
        private void fncMostrarErroresSC(ClsValidacionExcepcionLN pObjExcepcionSC, string pStrResumenSC)
        {
            string lStrMensajeSC;
            if (pObjExcepcionSC.lDicErroresCampos.TryGetValue(ClsRepuestoOrdenLNSC.lStrCampoOrdenSC, out lStrMensajeSC))
            {
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Advertencia, lStrMensajeSC);
                return;
            }

            if (pObjExcepcionSC.lDicErroresCampos.TryGetValue(ClsRepuestoOrdenLNSC.lStrCampoRepuestoSC, out lStrMensajeSC))
            {
                ddlRepuestoSC.CssClass = "form-select is-invalid";
                ddlRepuestoSC.Attributes["aria-invalid"] = "true";
                errRepuestoSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC);
                errRepuestoSC.Visible = true;
            }

            if (pObjExcepcionSC.lDicErroresCampos.TryGetValue(ClsRepuestoOrdenLNSC.lStrCampoCantidadSC, out lStrMensajeSC))
            {
                txtCantidadSC.CssClass = lStrClaseCampoSC + " is-invalid";
                txtCantidadSC.Attributes["aria-invalid"] = "true";
                errCantidadSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC);
                errCantidadSC.Visible = true;
            }

            lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error, pStrResumenSC + " Revisa los datos marcados.");
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void fncLimpiarErroresSC()
        {
            ddlRepuestoSC.CssClass = "form-select";
            ddlRepuestoSC.Attributes.Remove("aria-invalid");
            errRepuestoSC.Visible = false;
            txtCantidadSC.CssClass = lStrClaseCampoSC;
            txtCantidadSC.Attributes.Remove("aria-invalid");
            errCantidadSC.Visible = false;
        }

        /// <summary>Vuelve a la misma página con un aviso (evita repetir la operación al recargar).</summary>
        private void fncRedirigirSC(string pStrAvisoSC)
        {
            Response.Redirect("~/OrdenesReparacion/RepuestosOrdenSC.aspx?idOrden=" + lIntIdOrdenSC + "&" + pStrAvisoSC + "=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Muestra el aviso de orden inexistente y responde 404 (el recurso pedido no existe).</summary>
        private void fncMostrarNoEncontradaSC()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            pnlOrdenSC.Visible = false;
            pnlAgregarSC.Visible = false;
            pnlCerradaSC.Visible = false;
            pnlUsadosSC.Visible = false;
            pnlNoEncontradaSC.Visible = true;
        }
    }
}
