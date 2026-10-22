
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;

namespace CapaPresentacion.OrdenesReparacion
{
    public partial class webDetalleOrdenNT : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FncCargarDetalleNT();
            }
        }

        private void FncCargarDetalleNT()
        {
            int lIntIdOrdenNT;


            if (!int.TryParse(Request.QueryString["id"], out lIntIdOrdenNT)
                || lIntIdOrdenNT <= 0)
            {
                lblMensajeNT.Text = "El número de orden no es válido.";
                pnlDetalleNT.Visible = false;
                pnlCerrarNT.Visible = false;
                return;
            }

            try
            {
                ClsOrdenReparacionLNT lObjLogicaNT =
                    new ClsOrdenReparacionLNT();

                ClsOrdenReparacionENT lObjOrdenNT =
                    lObjLogicaNT.FncObtenerOrdenPorIdNT(lIntIdOrdenNT);

                if (lObjOrdenNT == null)
                {
                    lblMensajeNT.Text = "No se encontró la orden.";
                    pnlDetalleNT.Visible = false;
                    pnlCerrarNT.Visible = false;
                    return;
                }

                lblIdOrdenNT.Text = lObjOrdenNT.lIntIdOrdenNT.ToString();
                lblVinNT.Text = Server.HtmlEncode(lObjOrdenNT.lStrVinNT);
                lblPlacaNT.Text = Server.HtmlEncode(lObjOrdenNT.lStrPlacaNT);

                lblMotivoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.lStrMotivoIngresoNT);

                lblDiagnosticoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.lStrDiagnosticoNT);

                lblEstadoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.lStrEstadoNT);

                pnlDetalleNT.Visible = true;

                pnlCerrarNT.Visible =
                    string.Equals(
                        lObjOrdenNT.lStrEstadoNT,
                        "Abierta",
                        StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception lObjErrorNT)
            {
                System.Diagnostics.Trace.TraceError(
                    "Error al consultar la orden: " + lObjErrorNT);

                lblMensajeNT.Text =
                    "No fue posible consultar la información de la orden.";

                pnlDetalleNT.Visible = false;
                pnlCerrarNT.Visible = false;
            }
        }

        protected void btnCerrarOrdenNT_Click(
            object sender, EventArgs e)
        {
            int lIntIdOrdenNT;

            if (!int.TryParse(Request.QueryString["id"], out lIntIdOrdenNT)
                || lIntIdOrdenNT <= 0)
            {
                lblMensajeNT.Text = "El número de orden no es válido.";
                return;
            }

            try
            {
                ClsOrdenReparacionLNT lObjLogicaNT =
                    new ClsOrdenReparacionLNT();

                bool lBlnCerradaNT = lObjLogicaNT.FncCerrarOrdenNT(
                    lIntIdOrdenNT,
                    txtObservacionesNT.Text);

                if (!lBlnCerradaNT)
                {
                    lblMensajeNT.Text =
                        "La orden no existe o ya se encuentra cerrada.";

                    FncCargarDetalleNT();
                    return;
                }

                Response.Redirect(
                    "DetalleOrdenNT.aspx?id=" + lIntIdOrdenNT,
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ArgumentException lObjErrorNT)
            {
                lblMensajeNT.Text =
                    Server.HtmlEncode(lObjErrorNT.Message);
            }
            catch (Exception lObjErrorNT)
            {
                System.Diagnostics.Trace.TraceError(
                    "Error al cerrar la orden: " + lObjErrorNT);

                lblMensajeNT.Text =
                    "No fue posible cerrar la orden de reparación.";
            }
        }
    }
}

