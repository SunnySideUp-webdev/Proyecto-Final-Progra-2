//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Registro de vehículos
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Vehiculos
{
    /// <summary>
    /// Formulario para registrar un vehículo asociado a un cliente existente.
    /// El propietario se elige buscándolo o llega preseleccionado con ?idCliente=.
    /// </summary>
    public partial class webRegistrarVehiculoSC : Page
    {
        private const string gStrClavePropietarioSC = "IdPropietarioSC";

        private readonly ClsVehiculoLNSC gObjVehiculoLNSC = new ClsVehiculoLNSC();
        private readonly ClsClienteLNSC gObjClienteLNSC = new ClsClienteLNSC();

        private webSiteMasterSC gObjSitioSC
        {
            get { return (webSiteMasterSC)Master; }
        }

        /// <summary>
        /// Identificador del propietario elegido. Se guarda en el ViewState, que ASP.NET
        /// protege contra modificaciones; además, la Capa Lógica vuelve a comprobar que exista.
        /// </summary>
        private int gIntIdPropietarioSC
        {
            get { return ViewState[gStrClavePropietarioSC] is int lIntIdSC ? lIntIdSC : 0; }
            set { ViewState[gStrClavePropietarioSC] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litAyudaAnioSC.Text = "Entre " + ClsVehiculoLNSC.gIntAnioMinimoSC + " y " + ClsVehiculoLNSC.FncObtenerAnioMaximoSC() + ".";

            if (IsPostBack)
            {
                return;
            }

            SubMostrarPropietarioSC(null);

            string lStrIdClienteSC = Request.QueryString["idCliente"];
            if (string.IsNullOrEmpty(lStrIdClienteSC))
            {
                txtBuscarPropietarioSC.Focus();
                return;
            }

            try
            {
                int lIntIdClienteSC;
                ClsClienteENSC lObjClienteSC = int.TryParse(lStrIdClienteSC, out lIntIdClienteSC)
                    ? gObjClienteLNSC.FncObtenerClienteSC(lIntIdClienteSC)
                    : null;

                if (lObjClienteSC == null)
                {
                    gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Advertencia,
                        "El cliente indicado no existe. Busca al propietario en el formulario.");
                    return;
                }

                SubMostrarPropietarioSC(lObjClienteSC);
                txtPlacaSC.Focus();
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RegistrarVehiculo.aspx - cargar propietario");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible cargar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Busca clientes para elegir al propietario.</summary>
        protected void btnBuscarPropietarioSC_Click(object sender, EventArgs e)
        {
            SubLimpiarErroresSC();
            try
            {
                List<ClsClienteENSC> lLstClientesSC = gObjClienteLNSC.FncBuscarClientesSC(txtBuscarPropietarioSC.Text);
                rptPropietariosSC.DataSource = lLstClientesSC;
                rptPropietariosSC.DataBind();
                rptPropietariosSC.Visible = lLstClientesSC.Count > 0;
                pnlSinPropietariosSC.Visible = lLstClientesSC.Count == 0;
                pnlResultadosPropietarioSC.Visible = true;
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                string lStrMensajeSC;
                lObjExcepcionSC.gDicErroresCamposSC.TryGetValue(ClsClienteLNSC.gStrCampoBusquedaSC, out lStrMensajeSC);
                SubMarcarErrorPropietarioSC(lStrMensajeSC ?? "Revisa el texto de búsqueda.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RegistrarVehiculo.aspx - buscar propietario");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible buscar clientes en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Selecciona al cliente elegido en los resultados de búsqueda.</summary>
        protected void rptPropietariosSC_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int lIntIdClienteSC;
            if (e.CommandName != "Seleccionar" || !int.TryParse(Convert.ToString(e.CommandArgument), out lIntIdClienteSC))
            {
                return;
            }

            SubLimpiarErroresSC();
            try
            {
                ClsClienteENSC lObjClienteSC = gObjClienteLNSC.FncObtenerClienteSC(lIntIdClienteSC);
                if (lObjClienteSC == null)
                {
                    SubMarcarErrorPropietarioSC("El cliente seleccionado ya no existe. Búscalo de nuevo.");
                    return;
                }

                SubMostrarPropietarioSC(lObjClienteSC);
                txtPlacaSC.Focus();
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RegistrarVehiculo.aspx - seleccionar propietario");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible seleccionar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Quita el propietario elegido para buscar otro.</summary>
        protected void lnkCambiarPropietarioSC_Click(object sender, EventArgs e)
        {
            SubMostrarPropietarioSC(null);
            txtBuscarPropietarioSC.Focus();
        }

        /// <summary>Envía los datos a la Capa Lógica y muestra el resultado.</summary>
        protected void btnGuardarSC_Click(object sender, EventArgs e)
        {
            SubLimpiarErroresSC();

            ClsVehiculoENSC lObjVehiculoSC = new ClsVehiculoENSC
            {
                gIntIdClienteSC = gIntIdPropietarioSC,
                gStrPlacaSC = txtPlacaSC.Text,
                gStrVinSC = txtVinSC.Text,
                gStrMarcaSC = txtMarcaSC.Text,
                gStrModeloSC = txtModeloSC.Text
            };

            try
            {
                int lIntIdVehiculoSC = gObjVehiculoLNSC.FncRegistrarVehiculoSC(lObjVehiculoSC, txtAnioSC.Text);

                // Redirigir después de guardar evita registros duplicados si el usuario recarga la página.
                Response.Redirect("~/Vehiculos/DetalleVehiculo.aspx?id=" + lIntIdVehiculoSC + "&registrado=1", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionSC)
            {
                foreach (KeyValuePair<string, string> lObjErrorSC in lObjExcepcionSC.gDicErroresCamposSC)
                {
                    SubMarcarErrorSC(lObjErrorSC.Key, lObjErrorSC.Value);
                }

                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    lObjExcepcionSC.gDicErroresCamposSC.Count == 1
                        ? "No se guardó el vehículo. Revisa el campo marcado."
                        : "No se guardó el vehículo. Revisa los " + lObjExcepcionSC.gDicErroresCamposSC.Count + " campos marcados.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RegistrarVehiculo.aspx - guardar vehículo");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible guardar el vehículo en este momento. Tus datos siguen en el formulario; intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Muestra la tarjeta del propietario elegido o, si es nulo, el buscador.</summary>
        private void SubMostrarPropietarioSC(ClsClienteENSC pObjClienteSC)
        {
            bool lBlnSeleccionadoSC = pObjClienteSC != null;
            gIntIdPropietarioSC = lBlnSeleccionadoSC ? pObjClienteSC.gIntIdClienteSC : 0;
            pnlPropietarioSeleccionadoSC.Visible = lBlnSeleccionadoSC;
            pnlBuscarPropietarioSC.Visible = !lBlnSeleccionadoSC;
            pnlResultadosPropietarioSC.Visible = false;

            if (lBlnSeleccionadoSC)
            {
                litInicialesPropietarioSC.Text = ClsFormatoSC.FncObtenerInicialesSC(pObjClienteSC.gStrNombresSC, pObjClienteSC.gStrApellidosSC);
                litNombrePropietarioSC.Text = pObjClienteSC.gStrNombresSC + " " + pObjClienteSC.gStrApellidosSC;
                litDpiPropietarioSC.Text = ClsFormatoSC.FncFormatearDpiSC(pObjClienteSC.gStrDpiSC);
                litTelefonoPropietarioSC.Text = ClsFormatoSC.FncFormatearTelefonoSC(pObjClienteSC.gStrTelefonoSC);
            }
        }

        /// <summary>Relaciona cada campo de la Capa Lógica con su control y su mensaje.</summary>
        private Dictionary<string, Tuple<TextBox, Label>> FncObtenerCamposSC()
        {
            return new Dictionary<string, Tuple<TextBox, Label>>
            {
                { ClsVehiculoLNSC.gStrCampoPlacaSC, Tuple.Create(txtPlacaSC, errPlacaSC) },
                { ClsVehiculoLNSC.gStrCampoVinSC, Tuple.Create(txtVinSC, errVinSC) },
                { ClsVehiculoLNSC.gStrCampoMarcaSC, Tuple.Create(txtMarcaSC, errMarcaSC) },
                { ClsVehiculoLNSC.gStrCampoModeloSC, Tuple.Create(txtModeloSC, errModeloSC) },
                { ClsVehiculoLNSC.gStrCampoAnioSC, Tuple.Create(txtAnioSC, errAnioSC) }
            };
        }

        /// <summary>Marca un campo en rojo y muestra su mensaje debajo.</summary>
        private void SubMarcarErrorSC(string pStrCampoSC, string pStrMensajeSC)
        {
            if (pStrCampoSC == ClsVehiculoLNSC.gStrCampoPropietarioSC)
            {
                SubMarcarErrorPropietarioSC(pStrMensajeSC);
                return;
            }

            Tuple<TextBox, Label> lObjCampoSC;
            if (!FncObtenerCamposSC().TryGetValue(pStrCampoSC, out lObjCampoSC))
            {
                return;
            }

            lObjCampoSC.Item1.CssClass = lObjCampoSC.Item1.CssClass.Replace(" is-invalid", string.Empty) + " is-invalid";
            lObjCampoSC.Item1.Attributes["aria-invalid"] = "true";
            lObjCampoSC.Item2.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            lObjCampoSC.Item2.Visible = true;
        }

        /// <summary>Muestra un error en la sección del propietario.</summary>
        private void SubMarcarErrorPropietarioSC(string pStrMensajeSC)
        {
            txtBuscarPropietarioSC.CssClass = "form-control is-invalid";
            errPropietarioSC.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            errPropietarioSC.Visible = true;
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void SubLimpiarErroresSC()
        {
            foreach (Tuple<TextBox, Label> lObjCampoSC in FncObtenerCamposSC().Values)
            {
                lObjCampoSC.Item1.CssClass = lObjCampoSC.Item1.CssClass.Replace(" is-invalid", string.Empty);
                lObjCampoSC.Item1.Attributes.Remove("aria-invalid");
                lObjCampoSC.Item2.Text = string.Empty;
                lObjCampoSC.Item2.Visible = false;
            }

            txtBuscarPropietarioSC.CssClass = "form-control";
            errPropietarioSC.Text = string.Empty;
            errPropietarioSC.Visible = false;
        }
    }
}
