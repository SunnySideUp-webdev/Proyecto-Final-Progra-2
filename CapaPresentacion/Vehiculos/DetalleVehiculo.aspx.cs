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
        private readonly ClsVehiculoLNSC lObjVehiculoLNSC = new ClsVehiculoLNSC();

        private SiteMaster lObjSitioSC
        {
            get { return (SiteMaster)Master; }
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
                fncMostrarNoEncontradoSC();
                return;
            }

            try
            {
                ClsVehiculoENSC lObjVehiculoSC = lObjVehiculoLNSC.fncObtenerVehiculoSC(lIntIdVehiculoSC);
                if (lObjVehiculoSC == null)
                {
                    fncMostrarNoEncontradoSC();
                    return;
                }

                fncMostrarVehiculoSC(lObjVehiculoSC);

                if (Request.QueryString["registrado"] == "1")
                {
                    lObjSitioSC.fncMostrarAlerta(TipoAlerta.Exito, "Vehículo registrado correctamente.");
                }
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "DetalleVehiculo.aspx - consultar vehículo " + lIntIdVehiculoSC);
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible consultar el vehículo en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Copia los datos del vehículo y de su propietario a la página.</summary>
        private void fncMostrarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC)
        {
            string lStrPlacaSC = ClsFormatoSC.fncFormatearPlacaSC(pObjVehiculoSC.lStrPlacaSC);
            string lStrMarcaModeloSC = pObjVehiculoSC.lStrMarcaSC + " " + pObjVehiculoSC.lStrModeloSC;

            Page.Title = lStrMarcaModeloSC + " " + pObjVehiculoSC.lIntAnioSC;
            litPlacaGrandeSC.Text = lStrPlacaSC;
            litMarcaModeloSC.Text = lStrMarcaModeloSC;
            litIdVehiculoSC.Text = pObjVehiculoSC.lIntIdVehiculoSC.ToString();
            litFechaRegistroSC.Text = ClsFormatoSC.fncFormatearFechaSC(pObjVehiculoSC.lDatFechaRegistroUtcSC);

            litPlacaSC.Text = lStrPlacaSC;
            litVinSC.Text = pObjVehiculoSC.lStrVinSC;
            litMarcaSC.Text = pObjVehiculoSC.lStrMarcaSC;
            litModeloSC.Text = pObjVehiculoSC.lStrModeloSC;
            litAnioSC.Text = pObjVehiculoSC.lIntAnioSC.ToString();

            ClsClienteENSC lObjPropietarioSC = pObjVehiculoSC.lObjPropietarioSC;
            string lStrUrlClienteSC = "~/Clientes/DetalleCliente.aspx?id=" + lObjPropietarioSC.lIntIdClienteSC;
            lnkVerClienteSC.NavigateUrl = lStrUrlClienteSC;
            lnkNombrePropietarioSC.NavigateUrl = lStrUrlClienteSC;
            // HyperLink.Text no se codifica automáticamente: se codifica aquí.
            lnkNombrePropietarioSC.Text = Server.HtmlEncode(lObjPropietarioSC.lStrNombresSC + " " + lObjPropietarioSC.lStrApellidosSC);
            litInicialesPropietarioSC.Text = ClsFormatoSC.fncObtenerInicialesSC(lObjPropietarioSC.lStrNombresSC, lObjPropietarioSC.lStrApellidosSC);
            litDpiPropietarioSC.Text = ClsFormatoSC.fncFormatearDpiSC(lObjPropietarioSC.lStrDpiSC);
            litTelefonoPropietarioSC.Text = ClsFormatoSC.fncFormatearTelefonoSC(lObjPropietarioSC.lStrTelefonoSC);
            litCorreoPropietarioSC.Text = lObjPropietarioSC.lStrCorreoSC ?? string.Empty;
            lblCorreoVacioSC.Visible = string.IsNullOrWhiteSpace(lObjPropietarioSC.lStrCorreoSC);

            pnlVehiculoSC.Visible = true;
        }

        /// <summary>Indica al navegador que el recurso no existe (404) y muestra un mensaje claro.</summary>
        private void fncMostrarNoEncontradoSC()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            Page.Title = "Vehículo no encontrado";
            pnlNoEncontradoSC.Visible = true;
        }
    }
}
