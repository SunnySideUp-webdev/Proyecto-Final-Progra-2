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
    public enum TipoAlerta
    {
        Exito,
        Error,
        Advertencia,
        Informacion
    }

    /// <summary>
    /// Página maestra compartida: barra lateral, encabezado, mensajes y estructura común.
    /// </summary>
    public partial class SiteMaster : MasterPage
    {
        /// <summary>Elemento del menú principal.</summary>
        public class ClsElementoMenu
        {
            public string lStrTexto { get; set; }
            public string lStrDescripcion { get; set; }
            public string lStrUrl { get; set; }
            public string lStrIcono { get; set; }
            public string lStrCarpeta { get; set; }
            public bool lBlnActivo { get; set; }
        }

        /// <summary>
        /// Módulos del sistema. Un módulo solo aparece en el menú cuando su página existe
        /// en el proyecto, así nunca se muestran enlaces a secciones sin desarrollar.
        /// Cada integrante puede ajustar aquí la ruta de su página principal.
        /// </summary>
        private static readonly ClsElementoMenu[] lArrModulos =
        {
            new ClsElementoMenu { lStrTexto = "Inicio", lStrDescripcion = "Resumen del taller",
                lStrUrl = "~/Default.aspx", lStrIcono = "bi-grid-1x2", lStrCarpeta = "~/Default.aspx" },
            new ClsElementoMenu { lStrTexto = "Clientes", lStrDescripcion = "Registro y consulta de clientes",
                lStrUrl = "~/Clientes/Clientes.aspx", lStrIcono = "bi-people", lStrCarpeta = "~/Clientes/" },
            new ClsElementoMenu { lStrTexto = "Vehículos", lStrDescripcion = "Vehículos de cada cliente",
                lStrUrl = "~/Vehiculos/Vehiculos.aspx", lStrIcono = "bi-car-front", lStrCarpeta = "~/Vehiculos/" },
            new ClsElementoMenu { lStrTexto = "Órdenes de reparación", lStrDescripcion = "Seguimiento de reparaciones",
                lStrUrl = "~/OrdenesReparacion/OrdenesNT.aspx", lStrIcono = "bi-clipboard-check", lStrCarpeta = "~/OrdenesReparacion/" },
            new ClsElementoMenu { lStrTexto = "Inventario", lStrDescripcion = "Control de repuestos",
                lStrUrl = "~/Inventario/Inventario.aspx", lStrIcono = "bi-box-seam", lStrCarpeta = "~/Inventario/" },
            new ClsElementoMenu { lStrTexto = "Reportes", lStrDescripcion = "Reportes del taller",
                lStrUrl = "~/Reportes/Reportes.aspx", lStrIcono = "bi-bar-chart-line", lStrCarpeta = "~/Reportes/" }
        };

        /// <summary>Devuelve los módulos cuya página existe, marcando el de la página actual.</summary>
        /// <param name="pObjContexto">Contexto de la solicitud actual.</param>
        /// <returns>Módulos disponibles en el orden del menú.</returns>
        public static List<ClsElementoMenu> fncObtenerModulosDisponibles(HttpContext pObjContexto)
        {
            string lStrRutaActual = pObjContexto.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;

            return lArrModulos
                .Where(m => File.Exists(pObjContexto.Server.MapPath(m.lStrUrl)))
                .Select(m => new ClsElementoMenu
                {
                    lStrTexto = m.lStrTexto,
                    lStrDescripcion = m.lStrDescripcion,
                    lStrUrl = m.lStrUrl,
                    lStrIcono = m.lStrIcono,
                    lStrCarpeta = m.lStrCarpeta,
                    lBlnActivo = lStrRutaActual.StartsWith(m.lStrCarpeta, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkEstilosSitio.Href = fncUrlConVersion("~/Content/autodms.css");
            lnkMarca.NavigateUrl = "~/Default.aspx";
            rptModulos.DataSource = fncObtenerModulosDisponibles(Context);
            rptModulos.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // El título de cada página se usa en el encabezado y en la pestaña del navegador.
            // litTituloPagina lo codifica (Mode="Encode"); <title> no, por eso se codifica aquí:
            // puede contener datos guardados, como el nombre de un cliente.
            string lStrTitulo = string.IsNullOrWhiteSpace(Page.Title) ? "AutoManager DMS" : Page.Title;
            litTituloPagina.Text = lStrTitulo;
            Page.Title = HttpUtility.HtmlEncode(
                lStrTitulo == "AutoManager DMS" ? lStrTitulo : lStrTitulo + " · AutoManager DMS");
        }

        /// <summary>
        /// Agrega a la dirección de un archivo propio (CSS o JS) su fecha de modificación,
        /// para que el navegador descargue la versión nueva cuando el archivo cambia.
        /// </summary>
        /// <param name="pStrRuta">Ruta relativa a la aplicación, por ejemplo ~/Content/autodms.css.</param>
        /// <returns>Dirección con el parámetro ?v=.</returns>
        protected string fncUrlConVersion(string pStrRuta)
        {
            string lStrRutaFisica = Server.MapPath(pStrRuta);
            long lLngVersion = File.Exists(lStrRutaFisica) ? File.GetLastWriteTimeUtc(lStrRutaFisica).Ticks : 0;
            return ResolveUrl(pStrRuta) + "?v=" + lLngVersion;
        }

        /// <summary>Muestra un mensaje en la parte superior del contenido.</summary>
        /// <param name="pEnmTipo">Tipo de mensaje.</param>
        /// <param name="pStrMensaje">Texto para el usuario; se codifica antes de mostrarse.</param>
        public void fncMostrarAlerta(TipoAlerta pEnmTipo, string pStrMensaje)
        {
            string lStrClase;
            string lStrIcono;
            switch (pEnmTipo)
            {
                case TipoAlerta.Exito:
                    lStrClase = "success";
                    lStrIcono = "bi-check-circle-fill";
                    break;
                case TipoAlerta.Error:
                    lStrClase = "danger";
                    lStrIcono = "bi-exclamation-octagon-fill";
                    break;
                case TipoAlerta.Advertencia:
                    lStrClase = "warning";
                    lStrIcono = "bi-exclamation-triangle-fill";
                    break;
                default:
                    lStrClase = "info";
                    lStrIcono = "bi-info-circle-fill";
                    break;
            }

            pnlAlerta.CssClass = "alert alert-" + lStrClase + " alert-dismissible fade show adms-alerta";
            litAlertaIcono.Text = "<i class=\"bi " + lStrIcono + " flex-shrink-0\" aria-hidden=\"true\"></i>";
            litAlertaMensaje.Text = pStrMensaje;
            pnlAlerta.Visible = true;
        }
    }
}
