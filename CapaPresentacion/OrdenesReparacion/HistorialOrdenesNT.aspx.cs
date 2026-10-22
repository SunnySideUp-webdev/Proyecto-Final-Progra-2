//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Capa de Presentación
// Parte de Neider: todo el código de este archivo es de su módulo.
//==============================================================
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using System.Collections.Generic;

namespace CapaPresentacion.OrdenesReparacion
{
    /// <summary>Consulta el historial de reparaciones de un vehículo por su VIN.</summary>
    public partial class webHistorialOrdenesNT : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Aquí prepararemos la búsqueda por VIN.
            }
        }


        /// <summary>Busca las órdenes del VIN escrito y las muestra en la tabla.</summary>
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
