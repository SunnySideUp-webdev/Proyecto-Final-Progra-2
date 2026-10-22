//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Registro de clientes
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Clientes
{
    /// <summary>Formulario para registrar un cliente nuevo.</summary>
    public partial class webRegistrarClienteSC : Page
    {
        private readonly ClsClienteLNSC gObjClienteLNSC = new ClsClienteLNSC();

        private webSiteMasterSC gObjSitioSC
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDpiSC.Focus();
            }
        }

        /// <summary>Envía los datos a la Capa Lógica y muestra el resultado.</summary>
        protected void btnGuardarSC_Click(object sender, EventArgs e)
        {
            SubLimpiarErroresSC();

            ClsClienteENSC lObjClienteSC = new ClsClienteENSC
            {
                gStrDpiSC = txtDpiSC.Text,
                gStrNombresSC = txtNombresSC.Text,
                gStrApellidosSC = txtApellidosSC.Text,
                gStrTelefonoSC = txtTelefonoSC.Text,
                gStrCorreoSC = txtCorreoSC.Text,
                gStrDireccionSC = txtDireccionSC.Text
            };

            try
            {
                int lIntIdClienteSC = gObjClienteLNSC.FncRegistrarClienteSC(lObjClienteSC);

                // Redirigir después de guardar evita registros duplicados si el usuario recarga la página.
                Response.Redirect("~/Clientes/DetalleCliente.aspx?id=" + lIntIdClienteSC + "&registrado=1", false);
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
                        ? "No se guardó el cliente. Revisa el campo marcado."
                        : "No se guardó el cliente. Revisa los " + lObjExcepcionSC.gDicErroresCamposSC.Count + " campos marcados.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "RegistrarCliente.aspx - guardar cliente");
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible guardar el cliente en este momento. Tus datos siguen en el formulario; intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Relaciona cada campo de la Capa Lógica con su control y su mensaje.</summary>
        private Dictionary<string, Tuple<TextBox, Label>> FncObtenerCamposSC()
        {
            return new Dictionary<string, Tuple<TextBox, Label>>
            {
                { ClsClienteLNSC.gStrCampoDpiSC, Tuple.Create(txtDpiSC, errDpiSC) },
                { ClsClienteLNSC.gStrCampoNombresSC, Tuple.Create(txtNombresSC, errNombresSC) },
                { ClsClienteLNSC.gStrCampoApellidosSC, Tuple.Create(txtApellidosSC, errApellidosSC) },
                { ClsClienteLNSC.gStrCampoTelefonoSC, Tuple.Create(txtTelefonoSC, errTelefonoSC) },
                { ClsClienteLNSC.gStrCampoCorreoSC, Tuple.Create(txtCorreoSC, errCorreoSC) },
                { ClsClienteLNSC.gStrCampoDireccionSC, Tuple.Create(txtDireccionSC, errDireccionSC) }
            };
        }

        /// <summary>Marca un campo en rojo y muestra su mensaje debajo.</summary>
        private void SubMarcarErrorSC(string pStrCampoSC, string pStrMensajeSC)
        {
            Tuple<TextBox, Label> lObjCampoSC;
            if (!FncObtenerCamposSC().TryGetValue(pStrCampoSC, out lObjCampoSC))
            {
                return;
            }

            lObjCampoSC.Item1.CssClass = "form-control is-invalid";
            lObjCampoSC.Item1.Attributes["aria-invalid"] = "true";
            lObjCampoSC.Item2.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            lObjCampoSC.Item2.Visible = true;
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void SubLimpiarErroresSC()
        {
            foreach (Tuple<TextBox, Label> lObjCampoSC in FncObtenerCamposSC().Values)
            {
                lObjCampoSC.Item1.CssClass = "form-control";
                lObjCampoSC.Item1.Attributes.Remove("aria-invalid");
                lObjCampoSC.Item2.Text = string.Empty;
                lObjCampoSC.Item2.Visible = false;
            }
        }
    }
}
