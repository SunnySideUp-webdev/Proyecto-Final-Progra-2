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
using System.Collections.Generic;

namespace CapaPresentacion.OrdenesReparacion
{
    /// <summary>Formulario para abrir una orden de reparación de un vehículo registrado.</summary>
    public partial class webRegistrarOrdenNT : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SubCargarVehiculosNT();
            }
        }


        /// <summary>
        /// Llena la lista de vehículos (por placa). Usa la búsqueda del módulo de vehículos,
        /// que devuelve como máximo los 50 registrados más recientemente.
        /// </summary>
        private void SubCargarVehiculosNT()
        {
            ClsVehiculoLNSC lObjVehiculoLogicaNT =
                new ClsVehiculoLNSC();


            List<ClsVehiculoENSC> lObjVehiculosNT =
                lObjVehiculoLogicaNT.FncBuscarVehiculosSC(null);


            ddlVehiculoNT.DataSource = lObjVehiculosNT;
            ddlVehiculoNT.DataValueField = "gIntIdVehiculoSC";
            ddlVehiculoNT.DataTextField = "gStrPlacaSC";
            ddlVehiculoNT.DataBind();

            ddlVehiculoNT.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                    "-- Seleccione un vehículo --", ""));
        }


        /// <summary>Valida y registra la orden; al terminar vuelve al listado de órdenes.</summary>
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
                    lObjVehiculoLogicaNT.FncObtenerVehiculoSC(
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
                        gIntIdVehiculoNT = lIntIdVehiculoNT,
                        gStrVinNT = lObjVehiculoNT.gStrVinSC,
                        gStrMotivoIngresoNT = txtMotivoIngresoNT.Text,
                        gStrDiagnosticoNT = txtDiagnosticoNT.Text
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
