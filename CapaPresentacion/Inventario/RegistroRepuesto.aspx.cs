//==============================================================
// Nombre: Pavel Alexander Palma Carrillo
// Carné: 0900-24-4296
// Tema: AutoManager DMS - Módulo 3 Inventario - Registro de repuestos
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Inventario
{
    /// <summary>Formulario para registrar un repuesto nuevo en el inventario.</summary>
    public partial class webRegistroRepuestoPP : Page
    {
        private readonly ClsRepuestoLNPP gObjRepuestoLNPP = new ClsRepuestoLNPP();

        private SiteMaster gObjSitioPP
        {
            get { return (SiteMaster)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litAyudaAnioPP.Text = "Entre " + ClsVehiculoLNSC.lIntAnioMinimoSC + " y " + ClsVehiculoLNSC.fncObtenerAnioMaximoSC() + ".";

            if (!IsPostBack)
            {
                txtCodigoPP.Focus();
            }
        }

        /// <summary>Envía los datos a la Capa Lógica y muestra el resultado.</summary>
        protected void btnGuardarPP_Click(object sender, EventArgs e)
        {
            SubLimpiarErroresPP();

            ClsRepuestoENPP lObjRepuestoPP = new ClsRepuestoENPP
            {
                lStrCodigoPP = txtCodigoPP.Text,
                lStrNombrePP = txtNombrePP.Text,
                lStrDescripcionPP = txtDescripcionPP.Text,
                lStrMarcaCarroPP = txtMarcaPP.Text,
                lStrModeloCarroPP = txtModeloPP.Text
            };

            try
            {
                int lIntIdRepuestoPP = gObjRepuestoLNPP.FncRegistrarRepuestoPP(lObjRepuestoPP, txtStockPP.Text, txtAnioPP.Text);

                // Redirigir después de guardar evita registros duplicados si el usuario recarga la página.
                Response.Redirect("~/Inventario/Inventario.aspx?registrado=" + lIntIdRepuestoPP, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ClsValidacionExcepcionLN lObjExcepcionPP)
            {
                foreach (KeyValuePair<string, string> lObjErrorPP in lObjExcepcionPP.lDicErroresCampos)
                {
                    SubMarcarErrorPP(lObjErrorPP.Key, lObjErrorPP.Value);
                }

                gObjSitioPP.fncMostrarAlerta(TipoAlerta.Error,
                    lObjExcepcionPP.lDicErroresCampos.Count == 1
                        ? "No se guardó el repuesto. Revisa el campo marcado."
                        : "No se guardó el repuesto. Revisa los " + lObjExcepcionPP.lDicErroresCampos.Count + " campos marcados.");
            }
            catch (Exception lObjExcepcionPP)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionPP, "RegistroRepuesto.aspx - guardar repuesto");
                gObjSitioPP.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible guardar el repuesto en este momento. Tus datos siguen en el formulario; intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Relaciona cada campo de la Capa Lógica con su control y su mensaje.</summary>
        private Dictionary<string, Tuple<TextBox, Label>> FncObtenerCamposPP()
        {
            return new Dictionary<string, Tuple<TextBox, Label>>
            {
                { ClsRepuestoLNPP.lStrCampoCodigoPP, Tuple.Create(txtCodigoPP, errCodigoPP) },
                { ClsRepuestoLNPP.lStrCampoNombrePP, Tuple.Create(txtNombrePP, errNombrePP) },
                { ClsRepuestoLNPP.lStrCampoDescripcionPP, Tuple.Create(txtDescripcionPP, errDescripcionPP) },
                { ClsRepuestoLNPP.lStrCampoStockPP, Tuple.Create(txtStockPP, errStockPP) },
                { ClsRepuestoLNPP.lStrCampoMarcaPP, Tuple.Create(txtMarcaPP, errMarcaPP) },
                { ClsRepuestoLNPP.lStrCampoModeloPP, Tuple.Create(txtModeloPP, errModeloPP) },
                { ClsRepuestoLNPP.lStrCampoAnioPP, Tuple.Create(txtAnioPP, errAnioPP) }
            };
        }

        /// <summary>Marca un campo en rojo y muestra su mensaje debajo.</summary>
        private void SubMarcarErrorPP(string pStrCampoPP, string pStrMensajePP)
        {
            Tuple<TextBox, Label> lObjCampoPP;
            if (!FncObtenerCamposPP().TryGetValue(pStrCampoPP, out lObjCampoPP))
            {
                return;
            }

            lObjCampoPP.Item1.CssClass = lObjCampoPP.Item1.CssClass.Replace(" is-invalid", string.Empty) + " is-invalid";
            lObjCampoPP.Item1.Attributes["aria-invalid"] = "true";
            lObjCampoPP.Item2.Text = HttpUtility.HtmlEncode(pStrMensajePP);
            lObjCampoPP.Item2.Visible = true;
        }

        /// <summary>Quita las marcas de error de un intento anterior.</summary>
        private void SubLimpiarErroresPP()
        {
            foreach (Tuple<TextBox, Label> lObjCampoPP in FncObtenerCamposPP().Values)
            {
                lObjCampoPP.Item1.CssClass = lObjCampoPP.Item1.CssClass.Replace(" is-invalid", string.Empty);
                lObjCampoPP.Item1.Attributes.Remove("aria-invalid");
                lObjCampoPP.Item2.Text = string.Empty;
                lObjCampoPP.Item2.Visible = false;
            }
        }
    }
}
