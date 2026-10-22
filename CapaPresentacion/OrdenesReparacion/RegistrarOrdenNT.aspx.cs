
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using System.Collections.Generic;

namespace CapaPresentacion.OrdenesReparacion
{
    public partial class webRegistrarOrdenNT : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FncCargarVehiculosNT();
            }
        }


        private void FncCargarVehiculosNT()
        {
            ClsVehiculoLNSC lObjVehiculoLogicaNT =
                new ClsVehiculoLNSC();


            List<ClsVehiculoENSC> lObjVehiculosNT =
                lObjVehiculoLogicaNT.fncBuscarVehiculosSC(null);


            ddlVehiculoNT.DataSource = lObjVehiculosNT;
            ddlVehiculoNT.DataValueField = "lIntIdVehiculoSC";
            ddlVehiculoNT.DataTextField = "lStrPlacaSC";
            ddlVehiculoNT.DataBind();

            ddlVehiculoNT.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                    "-- Seleccione un vehículo --", ""));
        }


        protected void btnGuardarOrdenNT_Click(
            object sender, EventArgs e)
        {
            try
            {
                int lIntIdVehiculoNT;

                if (!int.TryParse(
                    ddlVehiculoNT.SelectedValue,
                    out lIntIdVehiculoNT) || lIntIdVehiculoNT <= 0)
                {
                    lblMensajeNT.Text =
                        "Debe seleccionar un vehículo.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMotivoIngresoNT.Text))
                {
                    lblMensajeNT.Text =
                        "Debe ingresar el motivo de la reparación.";
                    return;
                }

                ClsVehiculoLNSC lObjVehiculoLogicaNT =
                    new ClsVehiculoLNSC();


                ClsVehiculoENSC lObjVehiculoNT =
                    lObjVehiculoLogicaNT.fncObtenerVehiculoSC(
                        lIntIdVehiculoNT);


                if (lObjVehiculoNT == null)
                {
                    lblMensajeNT.Text =
                        "El vehículo seleccionado no existe.";
                    return;
                }

                ClsOrdenReparacionENT lObjOrdenNT =
                    new ClsOrdenReparacionENT
                    {
                        lIntIdVehiculoNT = lIntIdVehiculoNT,
                        lStrVinNT = lObjVehiculoNT.lStrVinSC,
                        lStrMotivoIngresoNT = txtMotivoIngresoNT.Text,
                        lStrDiagnosticoNT = txtDiagnosticoNT.Text
                    };

                ClsOrdenReparacionLNT lObjLogicaNT =
                    new ClsOrdenReparacionLNT();

                int lIntIdOrdenNT =
                    lObjLogicaNT.FncRegistrarOrdenNT(lObjOrdenNT);

                Response.Redirect(
                    "OrdenesNT.aspx", false);

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
                    "Error al registrar orden: " + lObjErrorNT);

                lblMensajeNT.Text =
                    "No fue posible registrar la orden.";
            }

        }
    }
}
