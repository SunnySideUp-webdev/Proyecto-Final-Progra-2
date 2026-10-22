//==============================================================
// Nombre: Fátima Palacios Ordoñez
// Carné: 0900-21-1810
// Tema: AutoManager DMS - Módulo 4 Reportes - Capa de Presentación
//==============================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Reportes
{
    /// <summary>
    /// Página de reportes del taller. Tiene seis secciones que se eligen con ?seccion=:
    /// resumen (por defecto), abiertas, cerradas, historial, existencias y repuestos.
    /// Solo consulta información; no registra ni modifica datos.
    /// </summary>
    public partial class webReportesFP : Page
    {
        private const string gStrSeccionResumenFP = "resumen";
        private const string gStrSeccionAbiertasFP = "abiertas";
        private const string gStrSeccionCerradasFP = "cerradas";
        private const string gStrSeccionHistorialFP = "historial";
        private const string gStrSeccionExistenciasFP = "existencias";
        private const string gStrSeccionRepuestosFP = "repuestos";

        /// <summary>A partir de cuántos días una orden abierta se resalta como atrasada.</summary>
        private const int gIntDiasAlertaFP = 7;

        private const string gStrClaseCampoFP = "form-control";
        private const string gStrClaseCampoErrorFP = "form-control is-invalid";

        private readonly ClsReporteLNFP gObjReporteLNFP = new ClsReporteLNFP();

        // Valor y estilo de los dos campos de fecha (inputs HTML del filtro que comparten
        // «Órdenes cerradas» y «Repuestos utilizados»), la etiqueta del primero y la sección
        // a la que vuelve el enlace «Mes actual».
        protected string gStrDesdeFP = string.Empty;
        protected string gStrHastaFP = string.Empty;
        protected string gStrClaseDesdeFP = gStrClaseCampoFP;
        protected string gStrClaseHastaFP = gStrClaseCampoFP;
        protected string gStrEtiquetaDesdeFP = "Cerradas desde";
        protected string gStrSeccionFP = gStrSeccionCerradasFP;

        // La búsqueda de clientes se reutiliza del módulo 1 (Clientes y vehículos).
        private readonly ClsClienteLNSC gObjClienteLNFP = new ClsClienteLNSC();

        private webSiteMasterSC gObjSitioFP
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            string lStrSeccionFP = FncObtenerSeccionFP();
            SubMarcarPestanaFP(lStrSeccionFP);

            switch (lStrSeccionFP)
            {
                case gStrSeccionAbiertasFP:
                    SubCargarAbiertasFP();
                    break;
                case gStrSeccionCerradasFP:
                    SubCargarCerradasFP();
                    break;
                case gStrSeccionHistorialFP:
                    SubCargarHistorialFP();
                    break;
                case gStrSeccionExistenciasFP:
                    SubCargarExistenciasFP();
                    break;
                case gStrSeccionRepuestosFP:
                    SubCargarRepuestosFP();
                    break;
                default:
                    SubCargarResumenFP();
                    break;
            }
        }

        /// <summary>
        /// Lleva las fechas del filtro a la dirección de la página (?desde=&amp;hasta=), para
        /// que el reporte se pueda recargar, compartir o imprimir con el mismo rango.
        /// </summary>
        protected void btnFiltrarFP_Click(object sender, EventArgs e)
        {
            // El filtro vuelve a la sección en la que está el usuario (cerradas o repuestos).
            string lStrSeccionFP = FncObtenerSeccionFP() == gStrSeccionRepuestosFP ? gStrSeccionRepuestosFP : gStrSeccionCerradasFP;

            // Los campos de fecha son inputs HTML: su valor llega en el formulario con su atributo name.
            string lStrDestinoFP = "~/Reportes/Reportes.aspx?seccion=" + lStrSeccionFP
                + "&desde=" + HttpUtility.UrlEncode((Request.Form["desde"] ?? string.Empty).Trim())
                + "&hasta=" + HttpUtility.UrlEncode((Request.Form["hasta"] ?? string.Empty).Trim());
            Response.Redirect(lStrDestinoFP, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Lleva el texto de búsqueda de clientes a la dirección de la página (?q=).</summary>
        protected void btnBuscarClienteFP_Click(object sender, EventArgs e)
        {
            string lStrDestinoFP = "~/Reportes/Reportes.aspx?seccion=" + gStrSeccionHistorialFP
                + "&q=" + HttpUtility.UrlEncode(txtClienteFP.Text.Trim());
            Response.Redirect(lStrDestinoFP, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ===================== Secciones =====================

        /// <summary>Muestra los indicadores generales del taller.</summary>
        private void SubCargarResumenFP()
        {
            pnlResumenFP.Visible = true;
            try
            {
                ClsResumenReportesENFP lObjResumenFP = gObjReporteLNFP.FncObtenerResumenFP();

                litAbiertasFP.Text = lObjResumenFP.gIntOrdenesAbiertasFP.ToString();
                litCerradasMesFP.Text = lObjResumenFP.gIntOrdenesCerradasMesFP.ToString();
                litVehiculosFP.Text = lObjResumenFP.gIntVehiculosAtendidosFP.ToString();
                litPromedioFP.Text = lObjResumenFP.gIntMinutosPromedioReparacionFP.HasValue
                    ? FncFormatearDuracionFP(lObjResumenFP.gIntMinutosPromedioReparacionFP.Value)
                    : "Sin datos";

                litMasAntiguaFP.Text = lObjResumenFP.gIntOrdenesAbiertasFP == 0
                    ? "No hay órdenes abiertas en este momento."
                    : "La orden abierta más antigua se abrió el "
                        + ClsFormatoSC.FncFormatearFechaSC(lObjResumenFP.gDatAperturaMasAntiguaUtcFP.Value)
                        + (lObjResumenFP.gIntDiasOrdenMasAntiguaFP <= 0
                            ? " (hoy)."
                            : " (hace " + FncTextoDiasFP(lObjResumenFP.gIntDiasOrdenMasAntiguaFP) + ").");
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar el resumen", "No fue posible cargar el resumen en este momento.");
                pnlResumenFP.Visible = false;
            }
        }

        /// <summary>Muestra las órdenes abiertas, de la más antigua a la más reciente.</summary>
        private void SubCargarAbiertasFP()
        {
            pnlAbiertasFP.Visible = true;
            try
            {
                List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteLNFP.FncListarOrdenesAbiertasFP();
                rptAbiertasFP.DataSource = lLstOrdenesFP;
                rptAbiertasFP.DataBind();
                rptAbiertasFP.Visible = lLstOrdenesFP.Count > 0;
                pnlSinAbiertasFP.Visible = lLstOrdenesFP.Count == 0;
                litTotalAbiertasFP.Text = FncContarFP(lLstOrdenesFP.Count, "orden", "órdenes");
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar las órdenes abiertas", "No fue posible cargar las órdenes abiertas en este momento.");
                pnlAbiertasFP.Visible = false;
            }
        }

        /// <summary>
        /// Muestra las órdenes cerradas en el rango de ?desde= y ?hasta=; sin fechas, el mes actual.
        /// </summary>
        private void SubCargarCerradasFP()
        {
            pnlFiltroFP.Visible = true;
            pnlCerradasFP.Visible = true;
            string lStrDesdeFP = Request.QueryString["desde"];
            string lStrHastaFP = Request.QueryString["hasta"];

            try
            {
                DateTime lDatDesdeFP;
                DateTime lDatHastaFP;
                List<ClsReporteOrdenENFP> lLstOrdenesFP =
                    gObjReporteLNFP.FncListarOrdenesCerradasFP(lStrDesdeFP, lStrHastaFP, out lDatDesdeFP, out lDatHastaFP);

                // Los campos muestran el rango que realmente se usó (incluido el mes actual por defecto).
                gStrDesdeFP = lDatDesdeFP.ToString(ClsReporteLNFP.gStrFormatoFechaFP, CultureInfo.InvariantCulture);
                gStrHastaFP = lDatHastaFP.ToString(ClsReporteLNFP.gStrFormatoFechaFP, CultureInfo.InvariantCulture);
                litRangoFP.Text = "Cerradas del " + ClsFormatoSC.FncFormatearFechaLargaSC(lDatDesdeFP)
                    + " al " + ClsFormatoSC.FncFormatearFechaLargaSC(lDatHastaFP) + ".";

                rptCerradasFP.DataSource = lLstOrdenesFP;
                rptCerradasFP.DataBind();
                rptCerradasFP.Visible = lLstOrdenesFP.Count > 0;
                pnlSinCerradasFP.Visible = lLstOrdenesFP.Count == 0;
                litTotalCerradasFP.Text = FncContarFP(lLstOrdenesFP.Count, "orden", "órdenes");
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionFP)
            {
                SubMarcarErroresFechasFP(lObjExcepcionFP, lStrDesdeFP, lStrHastaFP);
                pnlResultadoCerradasFP.Visible = false;
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar las órdenes cerradas", "No fue posible cargar las órdenes cerradas en este momento.");
                pnlResultadoCerradasFP.Visible = false;
            }
        }

        /// <summary>
        /// Muestra los repuestos utilizados en órdenes en el rango de ?desde= y ?hasta=; sin
        /// fechas, el mes actual. Usa el mismo filtro que «Órdenes cerradas».
        /// </summary>
        private void SubCargarRepuestosFP()
        {
            pnlFiltroFP.Visible = true;
            pnlRepuestosFP.Visible = true;
            gStrEtiquetaDesdeFP = "Utilizados desde";
            gStrSeccionFP = gStrSeccionRepuestosFP;
            string lStrDesdeFP = Request.QueryString["desde"];
            string lStrHastaFP = Request.QueryString["hasta"];

            try
            {
                DateTime lDatDesdeFP;
                DateTime lDatHastaFP;
                List<ClsRepuestoUtilizadoENFP> lLstRepuestosFP =
                    gObjReporteLNFP.FncListarRepuestosUtilizadosFP(lStrDesdeFP, lStrHastaFP, out lDatDesdeFP, out lDatHastaFP);

                gStrDesdeFP = lDatDesdeFP.ToString(ClsReporteLNFP.gStrFormatoFechaFP, CultureInfo.InvariantCulture);
                gStrHastaFP = lDatHastaFP.ToString(ClsReporteLNFP.gStrFormatoFechaFP, CultureInfo.InvariantCulture);

                int lIntUnidadesFP = 0;
                foreach (ClsRepuestoUtilizadoENFP lObjRepuestoFP in lLstRepuestosFP)
                {
                    lIntUnidadesFP += lObjRepuestoFP.gIntUnidadesFP;
                }
                litRangoRepuestosFP.Text = "Del " + ClsFormatoSC.FncFormatearFechaLargaSC(lDatDesdeFP)
                    + " al " + ClsFormatoSC.FncFormatearFechaLargaSC(lDatHastaFP) + ": "
                    + FncContarFP(lIntUnidadesFP, "unidad", "unidades") + " en total. Primero los más utilizados.";

                rptRepuestosFP.DataSource = lLstRepuestosFP;
                rptRepuestosFP.DataBind();
                rptRepuestosFP.Visible = lLstRepuestosFP.Count > 0;
                pnlSinRepuestosFP.Visible = lLstRepuestosFP.Count == 0;
                litTotalRepuestosFP.Text = FncContarFP(lLstRepuestosFP.Count, "repuesto", "repuestos");
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionFP)
            {
                SubMarcarErroresFechasFP(lObjExcepcionFP, lStrDesdeFP, lStrHastaFP);
                pnlRepuestosFP.Visible = false;
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar los repuestos utilizados", "No fue posible cargar los repuestos utilizados en este momento.");
                pnlRepuestosFP.Visible = false;
            }
        }

        /// <summary>
        /// Muestra las existencias del inventario: primero los repuestos con menos unidades,
        /// con su nivel de stock (agotado, bajo o disponible).
        /// </summary>
        private void SubCargarExistenciasFP()
        {
            pnlExistenciasFP.Visible = true;
            try
            {
                List<ClsExistenciaRepuestoENFP> lLstExistenciasFP = gObjReporteLNFP.FncListarExistenciasFP();

                int lIntAgotadosFP = 0;
                int lIntBajosFP = 0;
                foreach (ClsExistenciaRepuestoENFP lObjExistenciaFP in lLstExistenciasFP)
                {
                    if (lObjExistenciaFP.gStrNivelStockFP == ClsReporteLNFP.gStrNivelAgotadoFP)
                    {
                        lIntAgotadosFP++;
                    }
                    else if (lObjExistenciaFP.gStrNivelStockFP == ClsReporteLNFP.gStrNivelBajoFP)
                    {
                        lIntBajosFP++;
                    }
                }
                litResumenExistenciasFP.Text = FncContarFP(lIntAgotadosFP, "agotado", "agotados") + " y "
                    + FncContarFP(lIntBajosFP, "con stock bajo", "con stock bajo") + " ("
                    + ClsReporteLNFP.gIntStockBajoFP + " unidades o menos). Primero los que tienen menos unidades.";

                rptExistenciasFP.DataSource = lLstExistenciasFP;
                rptExistenciasFP.DataBind();
                rptExistenciasFP.Visible = lLstExistenciasFP.Count > 0;
                pnlSinExistenciasFP.Visible = lLstExistenciasFP.Count == 0;
                litTotalExistenciasFP.Text = FncContarFP(lLstExistenciasFP.Count, "repuesto", "repuestos");
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar las existencias", "No fue posible cargar las existencias en este momento.");
                pnlExistenciasFP.Visible = false;
            }
        }

        /// <summary>
        /// Sección de historial: con ?idCliente= muestra las reparaciones de ese cliente; con
        /// ?q= muestra los clientes que coinciden con la búsqueda; sin nada, solo el buscador.
        /// </summary>
        private void SubCargarHistorialFP()
        {
            pnlHistorialFP.Visible = true;
            string lStrBusquedaFP = Request.QueryString["q"] ?? string.Empty;
            txtClienteFP.Text = lStrBusquedaFP;

            string lStrIdClienteFP = Request.QueryString["idCliente"];
            if (!string.IsNullOrEmpty(lStrIdClienteFP))
            {
                SubMostrarHistorialClienteFP(lStrIdClienteFP);
            }
            else if (lStrBusquedaFP.Trim().Length > 0)
            {
                SubBuscarClientesFP(lStrBusquedaFP);
            }
        }

        /// <summary>Busca clientes con la misma búsqueda del módulo de clientes (DPI, nombre o teléfono).</summary>
        private void SubBuscarClientesFP(string pStrBusquedaFP)
        {
            pnlClientesEncontradosFP.Visible = true;
            try
            {
                List<ClsClienteENSC> lLstClientesFP = gObjClienteLNFP.FncBuscarClientesSC(pStrBusquedaFP);
                rptClientesFP.DataSource = lLstClientesFP;
                rptClientesFP.DataBind();
                rptClientesFP.Visible = lLstClientesFP.Count > 0;
                pnlSinClientesFP.Visible = lLstClientesFP.Count == 0;
                litTotalClientesFP.Text = FncContarFP(lLstClientesFP.Count, "cliente", "clientes");
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionFP)
            {
                string lStrMensajeFP;
                lObjExcepcionFP.gDicErroresCamposSC.TryGetValue(ClsClienteLNSC.gStrCampoBusquedaSC, out lStrMensajeFP);
                lblErrorClienteFP.Text = HttpUtility.HtmlEncode(lStrMensajeFP ?? "Revisa el texto de búsqueda.");
                lblErrorClienteFP.Visible = true;
                txtClienteFP.CssClass = "form-control is-invalid";
                pnlClientesEncontradosFP.Visible = false;
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "buscar clientes", "No fue posible buscar clientes en este momento.");
                pnlClientesEncontradosFP.Visible = false;
            }
        }

        /// <summary>Muestra todas las reparaciones de los vehículos del cliente indicado en ?idCliente=.</summary>
        private void SubMostrarHistorialClienteFP(string pStrIdClienteFP)
        {
            int lIntIdClienteFP;
            if (!int.TryParse(pStrIdClienteFP, out lIntIdClienteFP) || lIntIdClienteFP <= 0)
            {
                SubMostrarClienteNoEncontradoFP();
                return;
            }

            try
            {
                ClsClienteENSC lObjClienteFP = gObjClienteLNFP.FncObtenerClienteSC(lIntIdClienteFP);
                if (lObjClienteFP == null)
                {
                    SubMostrarClienteNoEncontradoFP();
                    return;
                }

                List<ClsReporteOrdenENFP> lLstOrdenesFP = gObjReporteLNFP.FncListarOrdenesPorClienteFP(lIntIdClienteFP);
                litClienteFP.Text = lObjClienteFP.gStrNombresSC + " " + lObjClienteFP.gStrApellidosSC;
                lnkVerClienteFP.NavigateUrl = "~/Clientes/DetalleCliente.aspx?id=" + lIntIdClienteFP;
                rptHistorialFP.DataSource = lLstOrdenesFP;
                rptHistorialFP.DataBind();
                rptHistorialFP.Visible = lLstOrdenesFP.Count > 0;
                pnlSinHistorialFP.Visible = lLstOrdenesFP.Count == 0;
                litTotalHistorialFP.Text = FncContarFP(lLstOrdenesFP.Count, "orden", "órdenes");
                pnlHistorialClienteFP.Visible = true;
            }
            catch (Exception lObjExcepcionFP)
            {
                SubMostrarErrorFP(lObjExcepcionFP, "cargar el historial del cliente " + lIntIdClienteFP,
                    "No fue posible cargar el historial del cliente en este momento.");
            }
        }

        // ===================== Apoyo =====================

        /// <summary>Lee ?seccion= y devuelve una sección conocida; cualquier otro valor muestra el resumen.</summary>
        private string FncObtenerSeccionFP()
        {
            string lStrSeccionFP = (Request.QueryString["seccion"] ?? string.Empty).Trim().ToLowerInvariant();
            switch (lStrSeccionFP)
            {
                case gStrSeccionAbiertasFP:
                case gStrSeccionCerradasFP:
                case gStrSeccionHistorialFP:
                case gStrSeccionExistenciasFP:
                case gStrSeccionRepuestosFP:
                    return lStrSeccionFP;
                default:
                    return gStrSeccionResumenFP;
            }
        }

        /// <summary>Resalta la pestaña de la sección actual (también para lectores de pantalla).</summary>
        private void SubMarcarPestanaFP(string pStrSeccionFP)
        {
            Dictionary<string, HyperLink> lDicPestanasFP = new Dictionary<string, HyperLink>
            {
                { gStrSeccionResumenFP, lnkResumenFP },
                { gStrSeccionAbiertasFP, lnkAbiertasFP },
                { gStrSeccionCerradasFP, lnkCerradasFP },
                { gStrSeccionHistorialFP, lnkHistorialFP },
                { gStrSeccionExistenciasFP, lnkExistenciasFP },
                { gStrSeccionRepuestosFP, lnkRepuestosFP }
            };

            foreach (KeyValuePair<string, HyperLink> lObjPestanaFP in lDicPestanasFP)
            {
                bool lBlnActivaFP = lObjPestanaFP.Key == pStrSeccionFP;
                lObjPestanaFP.Value.CssClass = lBlnActivaFP ? "adms-pestana activa" : "adms-pestana";
                if (lBlnActivaFP)
                {
                    lObjPestanaFP.Value.Attributes["aria-current"] = "page";
                }
            }
        }

        /// <summary>
        /// Muestra debajo del campo el mensaje que envió la Capa Lógica para ese campo.
        /// </summary>
        /// <returns><c>true</c> si el campo tenía un error (para pintarlo en rojo).</returns>
        private static bool FncMarcarErrorCampoFP(ClsValidacionExcepcionLNSC pObjExcepcionFP, string pStrCampoFP, Label pLblErrorFP)
        {
            string lStrMensajeFP;
            if (!pObjExcepcionFP.gDicErroresCamposSC.TryGetValue(pStrCampoFP, out lStrMensajeFP))
            {
                return false;
            }
            pLblErrorFP.Text = HttpUtility.HtmlEncode(lStrMensajeFP);
            pLblErrorFP.Visible = true;
            return true;
        }

        /// <summary>
        /// Conserva en el filtro las fechas que escribió el usuario y marca en rojo el campo
        /// con error, con el mensaje que envió la Capa Lógica.
        /// </summary>
        private void SubMarcarErroresFechasFP(ClsValidacionExcepcionLNSC pObjExcepcionFP, string pStrDesdeFP, string pStrHastaFP)
        {
            gStrDesdeFP = pStrDesdeFP ?? string.Empty;
            gStrHastaFP = pStrHastaFP ?? string.Empty;
            if (FncMarcarErrorCampoFP(pObjExcepcionFP, ClsReporteLNFP.gStrCampoDesdeFP, errDesdeFP))
            {
                gStrClaseDesdeFP = gStrClaseCampoErrorFP;
            }
            if (FncMarcarErrorCampoFP(pObjExcepcionFP, ClsReporteLNFP.gStrCampoHastaFP, errHastaFP))
            {
                gStrClaseHastaFP = gStrClaseCampoErrorFP;
            }
        }

        /// <summary>Muestra el aviso de cliente inexistente y responde 404 (el recurso pedido no existe).</summary>
        private void SubMostrarClienteNoEncontradoFP()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            pnlClienteNoEncontradoFP.Visible = true;
        }

        /// <summary>Registra el error para el equipo y muestra al usuario solo un mensaje general.</summary>
        private void SubMostrarErrorFP(Exception pObjExcepcionFP, string pStrOperacionFP, string pStrMensajeFP)
        {
            ClsRegistroErroresSC.SubRegistrarSC(pObjExcepcionFP, "Reportes.aspx - " + pStrOperacionFP);
            gObjSitioFP.SubMostrarAlertaSC(TipoAlertaSC.Error, pStrMensajeFP + " Intenta de nuevo en unos minutos.");
        }

        // ===================== Formato (se usan desde Reportes.aspx) =====================

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarFP(object pObjValorFP)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorFP));
        }

        /// <summary>Texto de los días que lleva abierta una orden: «Hoy», «1 día» o «N días».</summary>
        protected static string FncTextoDiasFP(int pIntDiasFP)
        {
            if (pIntDiasFP <= 0)
            {
                return "Hoy";
            }
            return pIntDiasFP == 1 ? "1 día" : pIntDiasFP + " días";
        }

        /// <summary>Clase CSS para los días abierta: resaltada si la orden lleva una semana o más.</summary>
        protected static string FncClaseDiasFP(int pIntDiasFP)
        {
            return pIntDiasFP >= gIntDiasAlertaFP ? "adms-dias adms-dias-alerta" : "adms-dias";
        }

        /// <summary>Clase CSS de la etiqueta de estado (Abierta o Cerrada).</summary>
        protected static string FncClaseEstadoFP(string pStrEstadoFP)
        {
            return pStrEstadoFP == "Abierta" ? "adms-estado adms-estado-abierta" : "adms-estado adms-estado-cerrada";
        }

        /// <summary>Clase CSS de la etiqueta del nivel de stock (agotado, bajo o disponible).</summary>
        protected static string FncClaseNivelStockFP(string pStrNivelFP)
        {
            if (pStrNivelFP == ClsReporteLNFP.gStrNivelAgotadoFP)
            {
                return "adms-estado adms-nivel-agotado";
            }
            return pStrNivelFP == ClsReporteLNFP.gStrNivelBajoFP ? "adms-estado adms-nivel-bajo" : "adms-estado adms-nivel-disponible";
        }

        /// <summary>
        /// Convierte minutos en un texto corto: «45 min», «3 h 20 min» o «2 días 5 h».
        /// </summary>
        protected static string FncFormatearDuracionFP(int pIntMinutosFP)
        {
            if (pIntMinutosFP < 1)
            {
                return "Menos de 1 min";
            }
            if (pIntMinutosFP < 60)
            {
                return pIntMinutosFP + " min";
            }

            int lIntHorasFP = pIntMinutosFP / 60;
            if (lIntHorasFP < 24)
            {
                int lIntRestoMinutosFP = pIntMinutosFP % 60;
                return lIntHorasFP + " h" + (lIntRestoMinutosFP > 0 ? " " + lIntRestoMinutosFP + " min" : string.Empty);
            }

            int lIntDiasFP = lIntHorasFP / 24;
            int lIntRestoHorasFP = lIntHorasFP % 24;
            return (lIntDiasFP == 1 ? "1 día" : lIntDiasFP + " días")
                + (lIntRestoHorasFP > 0 ? " " + lIntRestoHorasFP + " h" : string.Empty);
        }

        /// <summary>Fecha de cierre en hora de Guatemala, o una raya si la orden sigue abierta.</summary>
        protected static string FncFechaOpcionalFP(object pObjFechaUtcFP)
        {
            return pObjFechaUtcFP is DateTime lDatFechaFP ? ClsFormatoSC.FncFormatearFechaSC(lDatFechaFP) : "—";
        }

        /// <summary>Para el historial: cuánto duró una orden cerrada o cuánto lleva una abierta.</summary>
        protected static string FncTiempoOrdenFP(object pObjOrdenFP)
        {
            ClsReporteOrdenENFP lObjOrdenFP = (ClsReporteOrdenENFP)pObjOrdenFP;
            if (lObjOrdenFP.gDatFechaCierreUtcFP.HasValue)
            {
                return "Duró " + FncFormatearDuracionFP(lObjOrdenFP.gIntMinutosReparacionFP);
            }
            return lObjOrdenFP.gIntDiasAbiertaFP <= 0
                ? "Abierta desde hoy"
                : "Abierta hace " + FncTextoDiasFP(lObjOrdenFP.gIntDiasAbiertaFP);
        }

        /// <summary>Texto del contador con singular o plural: «1 orden», «3 órdenes», «0 órdenes».</summary>
        private static string FncContarFP(int pIntCantidadFP, string pStrSingularFP, string pStrPluralFP)
        {
            return pIntCantidadFP == 1 ? "1 " + pStrSingularFP : pIntCantidadFP + " " + pStrPluralFP;
        }
    }
}
