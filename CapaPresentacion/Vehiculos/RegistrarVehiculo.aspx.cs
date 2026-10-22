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
        private const string lStrClavePropietarioSC = "IdPropietarioSC";

        private readonly ClsVehiculoLNSC lObjVehiculoLNSC = new ClsVehiculoLNSC();
        private readonly ClsClienteLNSC lObjClienteLNSC = new ClsClienteLNSC();

        private SiteMaster lObjSitioSC
        {
            get { return (SiteMaster)Master; }
        }

        /// <summary>
        /// Identificador del propietario elegido. Se guarda en el ViewState, que ASP.NET
        /// protege contra modificaciones; además, la Capa Lógica vuelve a comprobar que exista.
        /// </summary>
        private int lIntIdPropietarioSC
        {
            get { return ViewState[lStrClavePropietarioSC] is int lIntIdSC ? lIntIdSC : 0; }
            set { ViewState[lStrClavePropietarioSC] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litAyudaAnioSC.Text = "Entre " + ClsVehiculoLNSC.lIntAnioMinimoSC + " y " + ClsVehiculoLNSC.fncObtenerAnioMaximoSC() + ".";

            if (IsPostBack)
            {
                return;
            }

            fncMostrarPropietarioSC(null);

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
                    ? lObjClienteLNSC.fncObtenerClienteSC(lIntIdClienteSC)
                    : null;

                if (lObjClienteSC == null)
                {
                    lObjSitioSC.fncMostrarAlerta(TipoAlerta.Advertencia,
                        "El cliente indicado no existe. Busca al propietario en el formulario.");
                    return;
                }

                fncMostrarPropietarioSC(lObjClienteSC);
                txtPlacaSC.Focus();
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RegistrarVehiculo.aspx - cargar propietario");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible cargar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Busca clientes para elegir al propietario.</summary>
        protected void btnBuscarPropietarioSC_Click(object sender, EventArgs e)
        {
            fncLimpiarErroresSC();
            try
            {
                List<ClsClienteENSC> lLstClientesSC = lObjClienteLNSC.fncBuscarClientesSC(txtBuscarPropietarioSC.Text);
                rptPropietariosSC.DataSource = lLstClientesSC;
                rptPropietariosSC.DataBind();
                rptPropietariosSC.Visible = lLstClientesSC.Count > 0;
                pnlSinPropietariosSC.Visible = lLstClientesSC.Count == 0;
                pnlResultadosPropietarioSC.Visible = true;
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionSC)
            {
                string lStrMensajeSC;
                lObjExcepcionSC.lDicErroresCampos.TryGetValue(ClsClienteLNSC.lStrCampoBusquedaSC, out lStrMensajeSC);
                fncMarcarErrorPropietarioSC(lStrMensajeSC ?? "Revisa el texto de búsqueda.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RegistrarVehiculo.aspx - buscar propietario");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
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

            fncLimpiarErroresSC();
            try
            {
                ClsClienteENSC lObjClienteSC = lObjClienteLNSC.fncObtenerClienteSC(lIntIdClienteSC);
                if (lObjClienteSC == null)
                {
                    fncMarcarErrorPropietarioSC("El cliente seleccionado ya no existe. Búscalo de nuevo.");
                    return;
                }

                fncMostrarPropietarioSC(lObjClienteSC);
                txtPlacaSC.Focus();
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RegistrarVehiculo.aspx - seleccionar propietario");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible seleccionar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Quita el propietario elegido para buscar otro.</summary>
        protected void lnkCambiarPropietarioSC_Click(object sender, EventArgs e)
        {
            fncMostrarPropietarioSC(null);
            txtBuscarPropietarioSC.Focus();
        }

        /// <summary>Envía los datos a la Capa Lógica y muestra el resultado.</summary>
        protected void btnGuardarSC_Click(object sender, EventArgs e)
        {
            fncLimpiarErroresSC();

            ClsVehiculoENSC lObjVehiculoSC = new ClsVehiculoENSC
            {
                lIntIdClienteSC = lIntIdPropietarioSC,
                lStrPlacaSC = txtPlacaSC.Text,
                lStrVinSC = txtVinSC.Text,
                lStrMarcaSC = txtMarcaSC.Text,
                lStrModeloSC = txtModeloSC.Text
            };

            try
            {
                int lIntIdVehiculoSC = lObjVehiculoLNSC.fncRegistrarVehiculoSC(lObjVehiculoSC, txtAnioSC.Text);

                // Redirigir después de guardar evita registros duplicados si el usuario recarga la página.
                Response.Redirect("~/Vehiculos/DetalleVehiculo.aspx?id=" + lIntIdVehiculoSC + "&registrado=1", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionSC)
            {
                foreach (KeyValuePair<string, string> lObjErrorSC in lObjExcepcionSC.lDicErroresCampos)
                {
                    fncMarcarErrorSC(lObjErrorSC.Key, lObjErrorSC.Value);
                }

                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    lObjExcepcionSC.lDicErroresCampos.Count == 1
                        ? "No se guardó el vehículo. Revisa el campo marcado."
                        : "No se guardó el vehículo. Revisa los " + lObjExcepcionSC.lDicErroresCampos.Count + " campos marcados.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RegistrarVehiculo.aspx - guardar vehículo");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible guardar el vehículo en este momento. Tus datos siguen en el formulario; intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string fncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Muestra la tarjeta del propietario elegido o, si es nulo, el buscador.</summary>
        private void fncMostrarPropietarioSC(ClsClienteENSC pObjClienteSC)
        {
            bool lBlnSeleccionadoSC = pObjClienteSC != null;
            lIntIdPropietarioSC = lBlnSeleccionadoSC ? pObjClienteSC.lIntIdClienteSC : 0;
            pnlPropietarioSeleccionadoSC.Visible = lBlnSeleccionadoSC;
            pnlBuscarPropietarioSC.Visible = !lBlnSeleccionadoSC;
            pnlResultadosPropietarioSC.Visible = false;

            if (lBlnSeleccionadoSC)
            {
                litInicialesPropietarioSC.Text = ClsFormatoSC.fncObtenerInicialesSC(pObjClienteSC.lStrNombresSC, pObjClienteSC.lStrApellidosSC);
                litNombrePropietarioSC.Text = pObjClienteSC.lStrNombresSC + " " + pObjClienteSC.lStrApellidosSC;
                litDpiPropietarioSC.Text = ClsFormatoSC.fncFormatearDpiSC(pObjClienteSC.lStrDpiSC);
                litTelefonoPropietarioSC.Text = ClsFormatoSC.fncFormatearTelefonoSC(pObjClienteSC.lStrTelefonoSC);
            }
        }

        /// <summary>Relaciona cada campo de la Capa Lógica con su control y su mensaje.</summary>
        private Dictionary<string, Tuple<TextBox, Label>> fncObtenerCamposSC()
        {
            return new Dictionary<string, Tuple<TextBox, Label>>
            {
                { ClsVehiculoLNSC.lStrCampoPlacaSC, Tuple.Create(txtPlacaSC, errPlacaSC) },
                { ClsVehiculoLNSC.lStrCampoVinSC, Tuple.Create(txtVinSC, errVinSC) },
                { ClsVehiculoLNSC.lStrCampoMarcaSC, Tuple.Create(txtMarcaSC, errMarcaSC) },
                { ClsVehiculoLNSC.lStrCampoModeloSC, Tuple.Create(txtModeloSC, errModeloSC) },
                { ClsVehiculoLNSC.lStrCampoAnioSC, Tuple.Create(txtAnioSC, errAnioSC) }
            };
        }

        /// <summary>Marca un campo en rojo y muestra su mensaje debajo.</summary>
        private void fncMarcarErrorSC(string pStrCampoSC, string pStrMensajeSC)
        {
            if (pStrCampoSC == ClsVehiculoLNSC.lStrCampoPropietarioSC)
            {
                fncMarcarErrorPropietarioSC(pStrMensajeSC);
                return;
            }

            Tuple<TextBox, Label> lObjCampoSC;
            if (!fncObtenerCamposSC().TryGetValue(pStrCampoSC, out lObjCampoSC))
            {
                return;
            }

            lObjCampoSC.Item1.CssClass = lObjCampoSC.Item1.CssClass.Replace(" is-invalid", string.Empty) + " is-invalid";
            lObjCampoSC.Item1.Attributes["aria-invalid"] = "true";
            lObjCampoSC.Item2.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            lObjCampoSC.Item2.Visible = true;
        }

        /// <summary>Muestra un error en la sección del propietario.</summary>
        private void fncMarcarErrorPropietarioSC(string pStrMensajeSC)
        {
            txtBuscarPropietarioSC.CssClass = "form-control is-invalid";
            errPropietarioSC.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            errPropietarioSC.Visible = true;
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void fncLimpiarErroresSC()
        {
            foreach (Tuple<TextBox, Label> lObjCampoSC in fncObtenerCamposSC().Values)
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
