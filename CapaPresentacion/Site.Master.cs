//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Página maestra
//==============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CapaPresentacion
{
    /// <summary>Tipos de mensaje que la página maestra puede mostrar.</summary>
    public enum TipoAlertaSC
    {
        Exito,
        Error,
        Advertencia,
        Informacion
    }

    /// <summary>
    /// Página maestra compartida: barra lateral, encabezado, mensajes y estructura común.
    /// </summary>
    public partial class webSiteMasterSC : MasterPage
    {
        /// <summary>Elemento del menú principal.</summary>
        public class ClsElementoMenuSC
        {
            public string gStrTextoSC { get; set; }
            public string gStrDescripcionSC { get; set; }
            public string gStrUrlSC { get; set; }
            public string gStrIconoSC { get; set; }
            public string gStrCarpetaSC { get; set; }
            public bool gBlnActivoSC { get; set; }
        }

        /// <summary>
        /// Módulos del sistema. Un módulo solo aparece en el menú cuando su página existe
        /// en el proyecto, así nunca se muestran enlaces a secciones sin desarrollar.
        /// Cada integrante puede ajustar aquí la ruta de su página principal.
        /// </summary>
        private static readonly ClsElementoMenuSC[] gArrModulosSC =
        {
            new ClsElementoMenuSC { gStrTextoSC = "Inicio", gStrDescripcionSC = "Resumen del taller",
                gStrUrlSC = "~/Default.aspx", gStrIconoSC = "bi-grid-1x2", gStrCarpetaSC = "~/Default.aspx" },
            new ClsElementoMenuSC { gStrTextoSC = "Clientes", gStrDescripcionSC = "Registro y consulta de clientes",
                gStrUrlSC = "~/Clientes/Clientes.aspx", gStrIconoSC = "bi-people", gStrCarpetaSC = "~/Clientes/" },
            new ClsElementoMenuSC { gStrTextoSC = "Vehículos", gStrDescripcionSC = "Vehículos de cada cliente",
                gStrUrlSC = "~/Vehiculos/Vehiculos.aspx", gStrIconoSC = "bi-car-front", gStrCarpetaSC = "~/Vehiculos/" },
            // Módulo 2 (parte de Neider): ruta de su página principal.
            new ClsElementoMenuSC { gStrTextoSC = "Órdenes de reparación", gStrDescripcionSC = "Seguimiento de reparaciones",
                gStrUrlSC = "~/OrdenesReparacion/OrdenesNT.aspx", gStrIconoSC = "bi-clipboard-check", gStrCarpetaSC = "~/OrdenesReparacion/" },
            new ClsElementoMenuSC { gStrTextoSC = "Inventario", gStrDescripcionSC = "Control de repuestos",
                gStrUrlSC = "~/Inventario/Inventario.aspx", gStrIconoSC = "bi-box-seam", gStrCarpetaSC = "~/Inventario/" },
            new ClsElementoMenuSC { gStrTextoSC = "Reportes", gStrDescripcionSC = "Reportes del taller",
                gStrUrlSC = "~/Reportes/Reportes.aspx", gStrIconoSC = "bi-bar-chart-line", gStrCarpetaSC = "~/Reportes/" }
        };

        /// <summary>Devuelve los módulos cuya página existe, marcando el de la página actual.</summary>
        /// <param name="pObjContexto">Contexto de la solicitud actual.</param>
        /// <returns>Módulos disponibles en el orden del menú.</returns>
        public static List<ClsElementoMenuSC> FncObtenerModulosDisponiblesSC(HttpContext pObjContextoSC)
        {
            string lStrRutaActualSC = pObjContextoSC.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;

            return gArrModulosSC
                .Where(pObjModuloSC => File.Exists(pObjContextoSC.Server.MapPath(pObjModuloSC.gStrUrlSC)))
                .Select(pObjModuloSC => new ClsElementoMenuSC
                {
                    gStrTextoSC = pObjModuloSC.gStrTextoSC,
                    gStrDescripcionSC = pObjModuloSC.gStrDescripcionSC,
                    gStrUrlSC = pObjModuloSC.gStrUrlSC,
                    gStrIconoSC = pObjModuloSC.gStrIconoSC,
                    gStrCarpetaSC = pObjModuloSC.gStrCarpetaSC,
                    gBlnActivoSC = lStrRutaActualSC.StartsWith(pObjModuloSC.gStrCarpetaSC, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkEstilosSitio.Href = FncUrlConVersionSC("~/Content/autodms.css");
            lnkMarca.NavigateUrl = "~/Default.aspx";
            rptModulos.DataSource = FncObtenerModulosDisponiblesSC(Context);
            rptModulos.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // El título de cada página se usa en el encabezado y en la pestaña del navegador.
            // litTituloPagina lo codifica (Mode="Encode"); <title> no, por eso se codifica aquí:
            // puede contener datos guardados, como el nombre de un cliente.
            string lStrTituloSC = string.IsNullOrWhiteSpace(Page.Title) ? "AutoManager DMS" : Page.Title;
            litTituloPagina.Text = lStrTituloSC;
            Page.Title = HttpUtility.HtmlEncode(
                lStrTituloSC == "AutoManager DMS" ? lStrTituloSC : lStrTituloSC + " · AutoManager DMS");
        }

        /// <summary>
        /// Agrega a la dirección de un archivo propio (CSS o JS) su fecha de modificación,
        /// para que el navegador descargue la versión nueva cuando el archivo cambia.
        /// </summary>
        /// <param name="pStrRuta">Ruta relativa a la aplicación, por ejemplo ~/Content/autodms.css.</param>
        /// <returns>Dirección con el parámetro ?v=.</returns>
        protected string FncUrlConVersionSC(string pStrRutaSC)
        {
            string lStrRutaFisicaSC = Server.MapPath(pStrRutaSC);
            long lLngVersionSC = File.Exists(lStrRutaFisicaSC) ? File.GetLastWriteTimeUtc(lStrRutaFisicaSC).Ticks : 0;
            return ResolveUrl(pStrRutaSC) + "?v=" + lLngVersionSC;
        }

        /// <summary>Muestra un mensaje en la parte superior del contenido.</summary>
        /// <param name="pEnmTipo">Tipo de mensaje.</param>
        /// <param name="pStrMensaje">Texto para el usuario; se codifica antes de mostrarse.</param>
        public void SubMostrarAlertaSC(TipoAlertaSC pEnmTipoSC, string pStrMensajeSC)
        {
            string lStrClaseSC;
            string lStrIconoSC;
            switch (pEnmTipoSC)
            {
                case TipoAlertaSC.Exito:
                    lStrClaseSC = "success";
                    lStrIconoSC = "bi-check-circle-fill";
                    break;
                case TipoAlertaSC.Error:
                    lStrClaseSC = "danger";
                    lStrIconoSC = "bi-exclamation-octagon-fill";
                    break;
                case TipoAlertaSC.Advertencia:
                    lStrClaseSC = "warning";
                    lStrIconoSC = "bi-exclamation-triangle-fill";
                    break;
                default:
                    lStrClaseSC = "info";
                    lStrIconoSC = "bi-info-circle-fill";
                    break;
            }

            pnlAlerta.CssClass = "alert alert-" + lStrClaseSC + " alert-dismissible fade show adms-alerta";
            litAlertaIcono.Text = "<i class=\"bi " + lStrIconoSC + " flex-shrink-0\" aria-hidden=\"true\"></i>";
            litAlertaMensaje.Text = pStrMensajeSC;
            pnlAlerta.Visible = true;
        }
    }
}
