//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Capa de Presentación
// Parte de Neider: todo el código de este archivo es de su módulo.
// Ajuste de integración (Sharon): nombres según la convención del docente (prefijo g en campos y propiedades, Sub en procedimientos).
//==============================================================
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;

namespace CapaPresentacion.OrdenesReparacion
{
    /// <summary>Muestra los datos de una orden y permite cerrarla mientras sigue abierta.</summary>
    public partial class webDetalleOrdenNT : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SubCargarDetalleNT();
            }
        }

        /// <summary>Carga la orden indicada en ?id=; si no es válida o no existe, muestra un mensaje.</summary>
        private void SubCargarDetalleNT()
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

                lblIdOrdenNT.Text = lObjOrdenNT.gIntIdOrdenNT.ToString();
                lblVinNT.Text = Server.HtmlEncode(lObjOrdenNT.gStrVinNT);
                lblPlacaNT.Text = Server.HtmlEncode(lObjOrdenNT.gStrPlacaNT);

                lblMotivoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.gStrMotivoIngresoNT);

                lblDiagnosticoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.gStrDiagnosticoNT);

                lblEstadoNT.Text =
                    Server.HtmlEncode(lObjOrdenNT.gStrEstadoNT);

                pnlDetalleNT.Visible = true;

                pnlCerrarNT.Visible =
                    string.Equals(
                        lObjOrdenNT.gStrEstadoNT,
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

        /// <summary>Cierra la orden con las observaciones escritas y vuelve a mostrar su detalle.</summary>
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

                    SubCargarDetalleNT();
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

