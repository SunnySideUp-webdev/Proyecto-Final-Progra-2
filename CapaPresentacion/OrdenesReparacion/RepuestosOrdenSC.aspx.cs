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
        private const string gStrClaseCampoSC = "form-control adms-numero";

        private readonly ClsRepuestoOrdenLNSC gObjRepuestoOrdenLNSC = new ClsRepuestoOrdenLNSC();
        private readonly ClsOrdenReparacionLNT gObjOrdenLNSC = new ClsOrdenReparacionLNT();
        private readonly ClsVehiculoLNSC gObjVehiculoLNSC = new ClsVehiculoLNSC();

        private int gIntIdOrdenSC;

        // Datos de la orden que se muestran con expresiones <%= %> en la página.
        protected string gStrPlacaSC = string.Empty;
        protected string gStrClaseEstadoSC = "adms-estado";
        protected bool gBlnOrdenAbiertaSC;

        private webSiteMasterSC gObjSitioSC
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["idOrden"], out gIntIdOrdenSC) || gIntIdOrdenSC <= 0)
            {
                SubMostrarNoEncontradaSC();
                return;
            }

            if (IsPostBack)
            {
                return;
            }

            if (FncCargarPaginaSC())
            {
                if (Request.QueryString["agregado"] == "1")
                {
                    gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Exito, "Se agregó el repuesto a la orden y se descontó del inventario.");
                }
                else if (Request.QueryString["quitado"] == "1")
                {
                    gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Exito, "Se quitó el repuesto de la orden y sus unidades volvieron al inventario.");
                }
            }
        }

        /// <summary>Registra el repuesto elegido y lo descuenta del inventario.</summary>
        protected void btnAgregarSC_Click(object sender, EventArgs e)
        {
            if (gIntIdOrdenSC <= 0)
            {
                return;
            }

            SubLimpiarErroresSC();
            string lStrRepuestoElegidoSC = ddlRepuestoSC.SelectedValue;

            try
            {
                gObjRepuestoOrdenLNSC.SubRegistrarRepuestoSC(gIntIdOrdenSC, lStrRepuestoElegidoSC, txtCantidadSC.Text);
                SubRedirigirSC("agregado");
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                FncCargarPaginaSC();
                SubSeleccionarRepuestoSC(lStrRepuestoElegidoSC);
                SubMostrarErroresSC(lObjExcepcionSC, "No se agregó el repuesto.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RepuestosOrdenSC.aspx - agregar repuesto");
                FncCargarPaginaSC();
                SubSeleccionarRepuestoSC(lStrRepuestoElegidoSC);
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible agregar el repuesto en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Quita un repuesto de la orden y devuelve sus unidades al inventario.</summary>
        protected void rptUsadosSC_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int lIntIdRepuestoOrdenSC;
            if (gIntIdOrdenSC <= 0 || e.CommandName != "Quitar"
                || !int.TryParse(Convert.ToString(e.CommandArgument), out lIntIdRepuestoOrdenSC))
            {
                return;
            }

            try
            {
                gObjRepuestoOrdenLNSC.SubQuitarRepuestoSC(gIntIdOrdenSC, lIntIdRepuestoOrdenSC);
                SubRedirigirSC("quitado");
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                FncCargarPaginaSC();
                SubMostrarErroresSC(lObjExcepcionSC, "No se quitó el repuesto.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RepuestosOrdenSC.aspx - quitar repuesto");
                FncCargarPaginaSC();
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible quitar el repuesto en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>
        /// Carga la orden, su vehículo, los repuestos utilizados y los disponibles.
        /// </summary>
        /// <returns><c>true</c> si la orden existe y se mostró.</returns>
        private bool FncCargarPaginaSC()
        {
            try
            {
                ClsOrdenReparacionENT lObjOrdenSC = gObjOrdenLNSC.FncObtenerOrdenPorIdNT(gIntIdOrdenSC);
                if (lObjOrdenSC == null)
                {
                    SubMostrarNoEncontradaSC();
                    return false;
                }

                gBlnOrdenAbiertaSC = lObjOrdenSC.gStrEstadoNT == "Abierta";
                Page.Title = "Repuestos de la orden #" + gIntIdOrdenSC;
                string lStrUrlOrdenSC = "~/OrdenesReparacion/DetalleOrdenNT.aspx?id=" + gIntIdOrdenSC;
                lnkVolverOrdenSC.NavigateUrl = lStrUrlOrdenSC;
                lnkRutaOrdenSC.NavigateUrl = lStrUrlOrdenSC;
                lnkRutaOrdenSC.Text = "Orden #" + gIntIdOrdenSC;
                phRutaOrdenSC.Visible = true;

                gStrPlacaSC = ClsFormatoSC.FncFormatearPlacaSC(lObjOrdenSC.gStrPlacaNT);
                gStrClaseEstadoSC = gBlnOrdenAbiertaSC ? "adms-estado adms-estado-abierta" : "adms-estado adms-estado-cerrada";
                litNumeroOrdenSC.Text = gIntIdOrdenSC.ToString();
                litEstadoSC.Text = lObjOrdenSC.gStrEstadoNT;
                litMotivoSC.Text = lObjOrdenSC.gStrMotivoIngresoNT;

                ClsVehiculoENSC lObjVehiculoSC = gObjVehiculoLNSC.FncObtenerVehiculoSC(lObjOrdenSC.gIntIdVehiculoNT);
                litVehiculoSC.Text = lObjVehiculoSC == null
                    ? "Vehículo VIN " + lObjOrdenSC.gStrVinNT
                    : lObjVehiculoSC.gStrMarcaSC + " " + lObjVehiculoSC.gStrModeloSC + " " + lObjVehiculoSC.gIntAnioSC;
                if (lObjVehiculoSC != null && lObjVehiculoSC.gObjPropietarioSC != null)
                {
                    lnkClienteSC.Text = HttpUtility.HtmlEncode(lObjVehiculoSC.gObjPropietarioSC.gStrNombresSC + " "
                        + lObjVehiculoSC.gObjPropietarioSC.gStrApellidosSC);
                    lnkClienteSC.NavigateUrl = "~/Clientes/DetalleCliente.aspx?id=" + lObjVehiculoSC.gIntIdClienteSC;
                    phClienteSC.Visible = true;
                }
                pnlOrdenSC.Visible = true;

                List<ClsRepuestoOrdenENSC> lLstUsosSC = gObjRepuestoOrdenLNSC.FncListarRepuestosDeOrdenSC(gIntIdOrdenSC);
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
                    lIntUnidadesSC += lObjUsoSC.gIntCantidadSC;
                }
                litTotalSC.Text = lIntUnidadesSC == 1 ? "1 unidad" : lIntUnidadesSC + " unidades";
                pnlUsadosSC.Visible = true;

                pnlCerradaSC.Visible = !gBlnOrdenAbiertaSC;
                pnlAgregarSC.Visible = gBlnOrdenAbiertaSC;
                if (gBlnOrdenAbiertaSC)
                {
                    SubCargarDisponiblesSC();
                }
                return true;
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RepuestosOrdenSC.aspx - cargar orden " + gIntIdOrdenSC);
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible cargar los repuestos de la orden en este momento. Intenta de nuevo en unos minutos.");
                return false;
            }
        }

        /// <summary>Llena la lista de repuestos con unidades disponibles.</summary>
        private void SubCargarDisponiblesSC()
        {
            List<ClsRepuestoENPP> lLstDisponiblesSC = gObjRepuestoOrdenLNSC.FncListarRepuestosDisponiblesSC();
            ddlRepuestoSC.Items.Clear();
            ddlRepuestoSC.Items.Add(new ListItem("Selecciona un repuesto", string.Empty));
            foreach (ClsRepuestoENPP lObjRepuestoSC in lLstDisponiblesSC)
            {
                // DropDownList codifica el texto de cada opción al dibujarla.
                ddlRepuestoSC.Items.Add(new ListItem(
                    lObjRepuestoSC.gStrCodigoPP + " · " + lObjRepuestoSC.gStrNombrePP
                        + " (" + lObjRepuestoSC.gStrMarcaCarroPP + " " + lObjRepuestoSC.gStrModeloCarroPP + " " + lObjRepuestoSC.gIntAnioCarroPP + ")"
                        + " · " + (lObjRepuestoSC.gIntStockPP == 1 ? "1 disponible" : lObjRepuestoSC.gIntStockPP + " disponibles"),
                    lObjRepuestoSC.gIntIdRepuestoPP.ToString()));
            }

            bool lBlnHayDisponiblesSC = lLstDisponiblesSC.Count > 0;
            pnlFormularioSC.Visible = lBlnHayDisponiblesSC;
            pnlSinDisponiblesSC.Visible = !lBlnHayDisponiblesSC;
        }

        /// <summary>Vuelve a elegir en la lista el repuesto que el usuario había seleccionado.</summary>
        private void SubSeleccionarRepuestoSC(string pStrIdRepuestoSC)
        {
            ListItem lObjOpcionSC = ddlRepuestoSC.Items.FindByValue(pStrIdRepuestoSC ?? string.Empty);
            if (lObjOpcionSC != null)
            {
                ddlRepuestoSC.ClearSelection();
                lObjOpcionSC.Selected = true;
            }
        }

        /// <summary>Muestra cada error en su campo, o como mensaje general si es de la orden.</summary>
        private void SubMostrarErroresSC(ClsValidacionExcepcionLNSC pObjExcepcionSC, string pStrResumenSC)
        {
            string lStrMensajeSC;
            if (pObjExcepcionSC.gDicErroresCamposSC.TryGetValue(ClsRepuestoOrdenLNSC.gStrCampoOrdenSC, out lStrMensajeSC))
            {
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Advertencia, lStrMensajeSC);
                return;
            }

            if (pObjExcepcionSC.gDicErroresCamposSC.TryGetValue(ClsRepuestoOrdenLNSC.gStrCampoRepuestoSC, out lStrMensajeSC))
            {
                ddlRepuestoSC.CssClass = "form-select is-invalid";
                ddlRepuestoSC.Attributes["aria-invalid"] = "true";
                errRepuestoSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC);
                errRepuestoSC.Visible = true;
            }

            if (pObjExcepcionSC.gDicErroresCamposSC.TryGetValue(ClsRepuestoOrdenLNSC.gStrCampoCantidadSC, out lStrMensajeSC))
            {
                txtCantidadSC.CssClass = gStrClaseCampoSC + " is-invalid";
                txtCantidadSC.Attributes["aria-invalid"] = "true";
                errCantidadSC.Text = HttpUtility.HtmlEncode(lStrMensajeSC);
                errCantidadSC.Visible = true;
            }

            gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error, pStrResumenSC + " Revisa los datos marcados.");
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void SubLimpiarErroresSC()
        {
            ddlRepuestoSC.CssClass = "form-select";
            ddlRepuestoSC.Attributes.Remove("aria-invalid");
            errRepuestoSC.Visible = false;
            txtCantidadSC.CssClass = gStrClaseCampoSC;
            txtCantidadSC.Attributes.Remove("aria-invalid");
            errCantidadSC.Visible = false;
        }

        /// <summary>Vuelve a la misma página con un aviso (evita repetir la operación al recargar).</summary>
        private void SubRedirigirSC(string pStrAvisoSC)
        {
            Response.Redirect("~/OrdenesReparacion/RepuestosOrdenSC.aspx?idOrden=" + gIntIdOrdenSC + "&" + pStrAvisoSC + "=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Muestra el aviso de orden inexistente y responde 404 (el recurso pedido no existe).</summary>
        private void SubMostrarNoEncontradaSC()
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
