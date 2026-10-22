//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Detalle del cliente
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Clientes
{
    /// <summary>Muestra los datos de un cliente registrado.</summary>
    public partial class webDetalleClienteSC : Page
    {
        private readonly ClsClienteLNSC gObjClienteLNSC = new ClsClienteLNSC();
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

            int lIntIdClienteSC;
            if (!int.TryParse(Request.QueryString["id"], out lIntIdClienteSC) || lIntIdClienteSC <= 0)
            {
                SubMostrarNoEncontradoSC();
                return;
            }

            try
            {
                ClsClienteENSC lObjClienteSC = gObjClienteLNSC.FncObtenerClienteSC(lIntIdClienteSC);
                if (lObjClienteSC == null)
                {
                    SubMostrarNoEncontradoSC();
                    return;
                }

                SubMostrarClienteSC(lObjClienteSC);
                SubCargarVehiculosSC(lObjClienteSC.gIntIdClienteSC);

                if (Request.QueryString["registrado"] == "1")
                {
                    gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Exito, "Cliente registrado correctamente.");
                }
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "DetalleCliente.aspx - consultar cliente " + lIntIdClienteSC);
                gObjSitioSC.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible consultar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Copia los datos del cliente a la página.</summary>
        private void SubMostrarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            string lStrNombreCompletoSC = pObjClienteSC.gStrNombresSC + " " + pObjClienteSC.gStrApellidosSC;

            Page.Title = lStrNombreCompletoSC;
            litInicialesSC.Text = ClsFormatoSC.FncObtenerInicialesSC(pObjClienteSC.gStrNombresSC, pObjClienteSC.gStrApellidosSC);
            litNombreCompletoSC.Text = lStrNombreCompletoSC;
            litIdClienteSC.Text = pObjClienteSC.gIntIdClienteSC.ToString();
            litDpiSC.Text = ClsFormatoSC.FncFormatearDpiSC(pObjClienteSC.gStrDpiSC);
            litNombresSC.Text = pObjClienteSC.gStrNombresSC;
            litApellidosSC.Text = pObjClienteSC.gStrApellidosSC;
            litTelefonoSC.Text = ClsFormatoSC.FncFormatearTelefonoSC(pObjClienteSC.gStrTelefonoSC);
            litCorreoSC.Text = pObjClienteSC.gStrCorreoSC ?? string.Empty;
            lblCorreoVacioSC.Visible = string.IsNullOrWhiteSpace(pObjClienteSC.gStrCorreoSC);
            litDireccionSC.Text = pObjClienteSC.gStrDireccionSC ?? string.Empty;
            lblDireccionVaciaSC.Visible = string.IsNullOrWhiteSpace(pObjClienteSC.gStrDireccionSC);
            litFechaRegistroSC.Text = ClsFormatoSC.FncFormatearFechaSC(pObjClienteSC.gDatFechaRegistroUtcSC);
            pnlClienteSC.Visible = true;
        }

        /// <summary>
        /// Muestra los vehículos del cliente y prepara los enlaces para registrar uno nuevo
        /// con este cliente ya seleccionado. Si solo falla esta consulta, la ficha del
        /// cliente se sigue mostrando.
        /// </summary>
        private void SubCargarVehiculosSC(int pIntIdClienteSC)
        {
            string lStrUrlRegistroSC = "~/Vehiculos/RegistrarVehiculo.aspx?idCliente=" + pIntIdClienteSC;
            lnkRegistrarVehiculoSC.NavigateUrl = lStrUrlRegistroSC;
            lnkRegistrarPrimerVehiculoSC.NavigateUrl = lStrUrlRegistroSC;

            try
            {
                List<ClsVehiculoENSC> lLstVehiculosSC = gObjVehiculoLNSC.FncListarVehiculosPorClienteSC(pIntIdClienteSC);
                rptVehiculosSC.DataSource = lLstVehiculosSC;
                rptVehiculosSC.DataBind();
                rptVehiculosSC.Visible = lLstVehiculosSC.Count > 0;
                pnlSinVehiculosSC.Visible = lLstVehiculosSC.Count == 0;
                litResumenVehiculosSC.Text = lLstVehiculosSC.Count == 0
                    ? "Sin vehículos registrados"
                    : lLstVehiculosSC.Count == 1 ? "1 vehículo registrado" : lLstVehiculosSC.Count + " vehículos registrados";
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionSC, "DetalleCliente.aspx - vehículos del cliente " + pIntIdClienteSC);
                rptVehiculosSC.Visible = false;
                pnlSinVehiculosSC.Visible = false;
                pnlErrorVehiculosSC.Visible = true;
                litResumenVehiculosSC.Text = string.Empty;
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Indica al navegador que el recurso no existe (404) y muestra un mensaje claro.</summary>
        private void SubMostrarNoEncontradoSC()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            Page.Title = "Cliente no encontrado";
            pnlNoEncontradoSC.Visible = true;
        }
    }
}
