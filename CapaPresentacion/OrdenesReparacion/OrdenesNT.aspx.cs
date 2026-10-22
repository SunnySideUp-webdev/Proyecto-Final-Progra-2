//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Capa de Presentación
// Parte de Neider: todo el código de este archivo es de su módulo.
//==============================================================
using System;
using System.Collections.Generic;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;

namespace CapaPresentacion.OrdenesReparacion
{
    /// <summary>Lista las órdenes de reparación que siguen abiertas.</summary>
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

        /// <summary>Consulta las órdenes abiertas y las muestra en la tabla; si falla, muestra un mensaje.</summary>
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
