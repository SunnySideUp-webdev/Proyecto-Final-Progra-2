
using System;
using System.Collections.Generic;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;

namespace CapaPresentacion.OrdenesReparacion
{
    public partial class webOrdenesNT : Page
    {
        private readonly ClsOrdenReparacionLNT gObjLogicaNT =
            new ClsOrdenReparacionLNT();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FncCargarOrdenesNT();
            }
        }

        private void FncCargarOrdenesNT()
        {
            try
            {
                List<ClsOrdenReparacionENT> lObjOrdenesNT =
                    gObjLogicaNT.FncConsultarOrdenesAbiertasNT();

                grvOrdenesNT.DataSource = lObjOrdenesNT;
                grvOrdenesNT.DataBind();
            }
            catch (Exception lObjErrorNT)
            {
                lblMensajeNT.Text =
                    "No fue posible cargar las órdenes de reparación.";

                grvOrdenesNT.Visible = false;
                System.Diagnostics.Trace.TraceError(
                    "Error al consultar órdenes: " + lObjErrorNT);
            }
        }
    }
}
