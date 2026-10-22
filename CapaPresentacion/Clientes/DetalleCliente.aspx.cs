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
        private readonly ClsClienteLNSC lObjClienteLNSC = new ClsClienteLNSC();
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

            int lIntIdClienteSC;
            if (!int.TryParse(Request.QueryString["id"], out lIntIdClienteSC) || lIntIdClienteSC <= 0)
            {
                fncMostrarNoEncontradoSC();
                return;
            }

            try
            {
                ClsClienteENSC lObjClienteSC = lObjClienteLNSC.fncObtenerClienteSC(lIntIdClienteSC);
                if (lObjClienteSC == null)
                {
                    fncMostrarNoEncontradoSC();
                    return;
                }

                fncMostrarClienteSC(lObjClienteSC);
                fncCargarVehiculosSC(lObjClienteSC.lIntIdClienteSC);

                if (Request.QueryString["registrado"] == "1")
                {
                    lObjSitioSC.fncMostrarAlerta(TipoAlerta.Exito, "Cliente registrado correctamente.");
                }
            }
            catch (Exception lObjExcepcionSC)
            {
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "DetalleCliente.aspx - consultar cliente " + lIntIdClienteSC);
                lObjSitioSC.fncMostrarAlerta(TipoAlerta.Error,
                    "No fue posible consultar el cliente en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Copia los datos del cliente a la página.</summary>
        private void fncMostrarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            string lStrNombreCompletoSC = pObjClienteSC.lStrNombresSC + " " + pObjClienteSC.lStrApellidosSC;

            Page.Title = lStrNombreCompletoSC;
            litInicialesSC.Text = ClsFormatoSC.fncObtenerInicialesSC(pObjClienteSC.lStrNombresSC, pObjClienteSC.lStrApellidosSC);
            litNombreCompletoSC.Text = lStrNombreCompletoSC;
            litIdClienteSC.Text = pObjClienteSC.lIntIdClienteSC.ToString();
            litDpiSC.Text = ClsFormatoSC.fncFormatearDpiSC(pObjClienteSC.lStrDpiSC);
            litNombresSC.Text = pObjClienteSC.lStrNombresSC;
            litApellidosSC.Text = pObjClienteSC.lStrApellidosSC;
            litTelefonoSC.Text = ClsFormatoSC.fncFormatearTelefonoSC(pObjClienteSC.lStrTelefonoSC);
            litCorreoSC.Text = pObjClienteSC.lStrCorreoSC ?? string.Empty;
            lblCorreoVacioSC.Visible = string.IsNullOrWhiteSpace(pObjClienteSC.lStrCorreoSC);
            litDireccionSC.Text = pObjClienteSC.lStrDireccionSC ?? string.Empty;
            lblDireccionVaciaSC.Visible = string.IsNullOrWhiteSpace(pObjClienteSC.lStrDireccionSC);
            litFechaRegistroSC.Text = ClsFormatoSC.fncFormatearFechaSC(pObjClienteSC.lDatFechaRegistroUtcSC);
            pnlClienteSC.Visible = true;
        }

        /// <summary>
        /// Muestra los vehículos del cliente y prepara los enlaces para registrar uno nuevo
        /// con este cliente ya seleccionado. Si solo falla esta consulta, la ficha del
        /// cliente se sigue mostrando.
        /// </summary>
        private void fncCargarVehiculosSC(int pIntIdClienteSC)
        {
            string lStrUrlRegistroSC = "~/Vehiculos/RegistrarVehiculo.aspx?idCliente=" + pIntIdClienteSC;
            lnkRegistrarVehiculoSC.NavigateUrl = lStrUrlRegistroSC;
            lnkRegistrarPrimerVehiculoSC.NavigateUrl = lStrUrlRegistroSC;

            try
            {
                List<ClsVehiculoENSC> lLstVehiculosSC = lObjVehiculoLNSC.fncListarVehiculosPorClienteSC(pIntIdClienteSC);
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
                ClsRegistroErrores.fncRegistrar(lObjExcepcionSC, "DetalleCliente.aspx - vehículos del cliente " + pIntIdClienteSC);
                rptVehiculosSC.Visible = false;
                pnlSinVehiculosSC.Visible = false;
                pnlErrorVehiculosSC.Visible = true;
                litResumenVehiculosSC.Text = string.Empty;
            }
        }

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string fncCodificarSC(object pObjValorSC)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorSC));
        }

        /// <summary>Indica al navegador que el recurso no existe (404) y muestra un mensaje claro.</summary>
        private void fncMostrarNoEncontradoSC()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            Page.Title = "Cliente no encontrado";
            pnlNoEncontradoSC.Visible = true;
        }
    }
}
