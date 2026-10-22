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
        private readonly ClsClienteLNSC lObjClienteLNSC = new ClsClienteLNSC();

        private SiteMaster lObjSitioSC
        {
            get { return (SiteMaster)Master; }
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
            fncLimpiarErroresSC();

            ClsClienteENSC lObjClienteSC = new ClsClienteENSC
            {
                lStrDpiSC = txtDpiSC.Text,
                lStrNombresSC = txtNombresSC.Text,
                lStrApellidosSC = txtApellidosSC.Text,
                lStrTelefonoSC = txtTelefonoSC.Text,
                lStrCorreoSC = txtCorreoSC.Text,
                lStrDireccionSC = txtDireccionSC.Text
            };

            try
            {
                int lIntIdClienteSC = lObjClienteLNSC.fncRegistrarClienteSC(lObjClienteSC);

                // Redirigir después de guardar evita registros duplicados si el usuario recarga la página.
                Response.Redirect("~/Clientes/DetalleCliente.aspx?id=" + lIntIdClienteSC + "&registrado=1", false);
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
                        ? "No se guardó el cliente. Revisa el campo marcado."
                        : "No se guardó el cliente. Revisa los " + lObjExcepcionSC.lDicErroresCampos.Count + " campos marcados.");
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "RegistrarCliente.aspx - guardar cliente");
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible guardar el cliente en este momento. Tus datos siguen en el formulario; intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Relaciona cada campo de la Capa Lógica con su control y su mensaje.</summary>
        private Dictionary<string, Tuple<TextBox, Label>> fncObtenerCamposSC()
        {
            return new Dictionary<string, Tuple<TextBox, Label>>
            {
                { ClsClienteLNSC.lStrCampoDpiSC, Tuple.Create(txtDpiSC, errDpiSC) },
                { ClsClienteLNSC.lStrCampoNombresSC, Tuple.Create(txtNombresSC, errNombresSC) },
                { ClsClienteLNSC.lStrCampoApellidosSC, Tuple.Create(txtApellidosSC, errApellidosSC) },
                { ClsClienteLNSC.lStrCampoTelefonoSC, Tuple.Create(txtTelefonoSC, errTelefonoSC) },
                { ClsClienteLNSC.lStrCampoCorreoSC, Tuple.Create(txtCorreoSC, errCorreoSC) },
                { ClsClienteLNSC.lStrCampoDireccionSC, Tuple.Create(txtDireccionSC, errDireccionSC) }
            };
        }

        /// <summary>Marca un campo en rojo y muestra su mensaje debajo.</summary>
        private void fncMarcarErrorSC(string pStrCampoSC, string pStrMensajeSC)
        {
            Tuple<TextBox, Label> lObjCampoSC;
            if (!fncObtenerCamposSC().TryGetValue(pStrCampoSC, out lObjCampoSC))
            {
                return;
            }

            lObjCampoSC.Item1.CssClass = "form-control is-invalid";
            lObjCampoSC.Item1.Attributes["aria-invalid"] = "true";
            lObjCampoSC.Item2.Text = HttpUtility.HtmlEncode(pStrMensajeSC);
            lObjCampoSC.Item2.Visible = true;
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void fncLimpiarErroresSC()
        {
            foreach (Tuple<TextBox, Label> lObjCampoSC in fncObtenerCamposSC().Values)
            {
                lObjCampoSC.Item1.CssClass = "form-control";
                lObjCampoSC.Item1.Attributes.Remove("aria-invalid");
                lObjCampoSC.Item2.Text = string.Empty;
                lObjCampoSC.Item2.Visible = false;
            }
        }
    }
}
