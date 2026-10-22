
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using System.Collections.Generic;

namespace CapaPresentacion.OrdenesReparacion
{
    public partial class webHistorialOrdenesNT : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Aquí prepararemos la búsqueda por VIN.
            }
        }


        protected void btnBuscarNT_Click(object sender, EventArgs e)
        {
            lblMensajeNT.Text = string.Empty;
            grvHistorialNT.DataSource = null;
            grvHistorialNT.DataBind();

            try
            {
                string lStrVinNT = txtVinNT.Text.Trim();

                ClsOrdenReparacionLNT lObjLogicaNT =
                    new ClsOrdenReparacionLNT();

                List<ClsOrdenReparacionENT> lObjHistorialNT =
                    lObjLogicaNT.FncConsultarHistorialPorVinNT(
                        lStrVinNT);

                grvHistorialNT.DataSource = lObjHistorialNT;
                grvHistorialNT.DataBind();

                if (lObjHistorialNT.Count == 0)
                {
                    lblMensajeNT.Text =
                        "No se encontraron reparaciones para este VIN.";
                }
            }
            catch (ArgumentException lObjErrorNT)
            {
                lblMensajeNT.Text =
                    Server.HtmlEncode(lObjErrorNT.Message);
            }
            catch (Exception lObjErrorNT)
            {
                System.Diagnostics.Trace.TraceError(
                    "Error al consultar historial: " + lObjErrorNT);

                lblMensajeNT.Text =
                    "No fue posible consultar el historial de reparaciones.";
            }
        }

    }
}
