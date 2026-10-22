//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Detalle del vehículo
//==============================================================
using System;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Vehiculos
{
    /// <summary>Muestra los datos de un vehículo y de su propietario.</summary>
    public partial class webDetalleVehiculoSC : Page
    {
        private readonly ClsVehiculoLNSC gObjVehiculoLNSC = new ClsVehiculoLNSC();

        private webSiteMasterSC gObjSitioSC
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int lIntIdVehiculoSC;
            if (!int.TryParse(Request.QueryString["id"], out lIntIdVehiculoSC) || lIntIdVehiculoSC <= 0)
            {
                SubMostrarNoEncontradoSC();
                return;
            }

            try
            {
                ClsVehiculoENSC lObjVehiculoSC = gObjVehiculoLNSC.FncObtenerVehiculoSC(lIntIdVehiculoSC);
                if (lObjVehiculoSC == null)
                {
                    SubMostrarNoEncontradoSC();
                    return;
                }

                SubMostrarVehiculoSC(lObjVehiculoSC);

                if (Request.QueryString["registrado"] == "1")
                {
                    gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Exito, "Vehículo registrado correctamente.");
                }
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "DetalleVehiculo.aspx - consultar vehículo " + lIntIdVehiculoSC);
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible consultar el vehículo en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Copia los datos del vehículo y de su propietario a la página.</summary>
        private void SubMostrarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC)
        {
            string lStrPlacaSC = ClsFormatoSC.FncFormatearPlacaSC(pObjVehiculoSC.gStrPlacaSC);
            string lStrMarcaModeloSC = pObjVehiculoSC.gStrMarcaSC + " " + pObjVehiculoSC.gStrModeloSC;

            Page.Title = lStrMarcaModeloSC + " " + pObjVehiculoSC.gIntAnioSC;
            litPlacaGrandeSC.Text = lStrPlacaSC;
            litMarcaModeloSC.Text = lStrMarcaModeloSC;
            litIdVehiculoSC.Text = pObjVehiculoSC.gIntIdVehiculoSC.ToString();
            litFechaRegistroSC.Text = ClsFormatoSC.FncFormatearFechaSC(pObjVehiculoSC.gDatFechaRegistroUtcSC);

            litPlacaSC.Text = lStrPlacaSC;
            litVinSC.Text = pObjVehiculoSC.gStrVinSC;
            litMarcaSC.Text = pObjVehiculoSC.gStrMarcaSC;
            litModeloSC.Text = pObjVehiculoSC.gStrModeloSC;
            litAnioSC.Text = pObjVehiculoSC.gIntAnioSC.ToString();

            ClsClienteENSC lObjPropietarioSC = pObjVehiculoSC.gObjPropietarioSC;
            string lStrUrlClienteSC = "~/Clientes/DetalleCliente.aspx?id=" + lObjPropietarioSC.gIntIdClienteSC;
            lnkVerClienteSC.NavigateUrl = lStrUrlClienteSC;
            lnkNombrePropietarioSC.NavigateUrl = lStrUrlClienteSC;
            // HyperLink.Text no se codifica automáticamente: se codifica aquí.
            lnkNombrePropietarioSC.Text = Server.HtmlEncode(lObjPropietarioSC.gStrNombresSC + " " + lObjPropietarioSC.gStrApellidosSC);
            litInicialesPropietarioSC.Text = ClsFormatoSC.FncObtenerInicialesSC(lObjPropietarioSC.gStrNombresSC, lObjPropietarioSC.gStrApellidosSC);
            litDpiPropietarioSC.Text = ClsFormatoSC.FncFormatearDpiSC(lObjPropietarioSC.gStrDpiSC);
            litTelefonoPropietarioSC.Text = ClsFormatoSC.FncFormatearTelefonoSC(lObjPropietarioSC.gStrTelefonoSC);
            litCorreoPropietarioSC.Text = lObjPropietarioSC.gStrCorreoSC ?? string.Empty;
            lblCorreoVacioSC.Visible = string.IsNullOrWhiteSpace(lObjPropietarioSC.gStrCorreoSC);

            pnlVehiculoSC.Visible = true;
        }

        /// <summary>Indica al navegador que el recurso no existe (404) y muestra un mensaje claro.</summary>
        private void SubMostrarNoEncontradoSC()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            Page.Title = "Vehículo no encontrado";
            pnlNoEncontradoSC.Visible = true;
        }
    }
}
