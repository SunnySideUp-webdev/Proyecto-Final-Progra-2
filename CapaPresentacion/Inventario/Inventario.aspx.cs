//==============================================================
// Nombre: Pavel Alexander Palma Carrillo
// Carné: 0900-24-4296
// Tema: AutoManager DMS - Módulo 3 Inventario - Capa de Presentación
//==============================================================
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using CapaEntidades;
using CapaLogica;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Inventario
{
    /// <summary>
    /// Lista los repuestos (los más recientes primero), permite buscarlos por nombre o código
    /// (?q=) y registrar entradas a bodega en una ventana que se abre con ?entrada=.
    /// </summary>
    public partial class webInventarioPP : Page
    {
        private readonly ClsRepuestoLNPP gObjRepuestoLNPP = new ClsRepuestoLNPP();

        /// <summary>Dirección del listado con la búsqueda actual; la usan «Cancelar» y «Cerrar».</summary>
        protected string gStrUrlVolverPP = "Inventario.aspx";

        private string gStrBusquedaPP = string.Empty;

        private webSiteMasterSC gObjSitioPP
        {
            get { return (webSiteMasterSC)Master; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            gStrBusquedaPP = (Request.QueryString["q"] ?? string.Empty).Trim();
            gStrUrlVolverPP = gStrBusquedaPP.Length == 0
                ? "Inventario.aspx"
                : "Inventario.aspx?q=" + HttpUtility.UrlEncode(gStrBusquedaPP);

            if (IsPostBack)
            {
                return;
            }

            txtBuscarPP.Text = gStrBusquedaPP;
            SubMostrarAvisoPP();
            SubCargarRepuestosPP();
            SubAbrirEntradaPP();
        }

        /// <summary>Lleva la búsqueda a la dirección de la página (?q=) para poder recargarla o compartirla.</summary>
        protected void btnBuscarPP_Click(object sender, EventArgs e)
        {
            string lStrCriterioPP = txtBuscarPP.Text.Trim();
            string lStrDestinoPP = lStrCriterioPP.Length == 0
                ? "~/Inventario/Inventario.aspx"
                : "~/Inventario/Inventario.aspx?q=" + HttpUtility.UrlEncode(lStrCriterioPP);

            Response.Redirect(lStrDestinoPP, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>Suma al stock las unidades escritas en la ventana de entrada.</summary>
        protected void btnGuardarEntradaPP_Click(object sender, EventArgs e)
        {
            int lIntIdRepuestoPP = FncLeerIdEntradaPP();
            errCantidadPP.Visible = false;

            try
            {
                gObjRepuestoLNPP.FncRegistrarEntradaPP(lIntIdRepuestoPP, txtCantidadPP.Text);

                // Redirigir después de guardar evita registrar la entrada dos veces si se recarga la página.
                Response.Redirect(gStrUrlVolverPP + (gStrBusquedaPP.Length == 0 ? "?" : "&")
                    + "actualizado=" + lIntIdRepuestoPP, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionPP)
            {
                SubCargarRepuestosPP();

                string lStrMensajePP;
                if (lObjExcepcionPP.gDicErroresCamposSC.TryGetValue(ClsRepuestoLNPP.gStrCampoCantidadPP, out lStrMensajePP))
                {
                    txtCantidadPP.CssClass = "form-control adms-numero is-invalid";
                    txtCantidadPP.Attributes["aria-invalid"] = "true";
                    errCantidadPP.Text = HttpUtility.HtmlEncode(lStrMensajePP);
                    errCantidadPP.Visible = true;
                    SubAbrirEntradaPP();
                }
                else
                {
                    lObjExcepcionPP.gDicErroresCamposSC.TryGetValue(ClsRepuestoLNPP.gStrCampoRepuestoPP, out lStrMensajePP);
                    gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Advertencia, lStrMensajePP ?? "No se pudo registrar la entrada.");
                }
            }
            catch (Exception lObjExcepcionPP)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionPP, "Inventario.aspx - registrar entrada");
                gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible registrar la entrada en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        // ===================== Carga de datos =====================

        /// <summary>Consulta los repuestos según la búsqueda de ?q= y llena la tabla.</summary>
        private void SubCargarRepuestosPP()
        {
            bool lBlnBuscandoPP = gStrBusquedaPP.Length > 0;
            lnkLimpiarPP.Visible = lBlnBuscandoPP;

            try
            {
                int lIntTotalPP = gObjRepuestoLNPP.FncContarRepuestosPP();
                litTotalPP.Text = lIntTotalPP == 1 ? "1 repuesto registrado" : lIntTotalPP + " repuestos registrados";

                List<ClsRepuestoENPP> lLstRepuestosPP = gObjRepuestoLNPP.FncConsultarRepuestosPP(gStrBusquedaPP);
                rptRepuestosPP.DataSource = lLstRepuestosPP;
                rptRepuestosPP.DataBind();
                rptRepuestosPP.Visible = lLstRepuestosPP.Count > 0;
                pnlVacioPP.Visible = lLstRepuestosPP.Count == 0;

                if (lBlnBuscandoPP)
                {
                    litResumenPP.Text = (lLstRepuestosPP.Count == 1 ? "1 resultado" : lLstRepuestosPP.Count + " resultados")
                        + " para «" + gStrBusquedaPP + "»";
                    litDetallePP.Text = "Coincidencias en el nombre o código exacto; el código exacto aparece primero.";
                    litVacioTituloPP.Text = "No se encontraron repuestos";
                    litVacioDetallePP.Text = "Revisa el nombre o el código escritos.";
                }
                else
                {
                    litResumenPP.Text = "Repuestos registrados recientemente";
                    litDetallePP.Text = lIntTotalPP > ClsRepuestoLNPP.gIntMaximoResultadosPP
                        ? "Se muestran los " + ClsRepuestoLNPP.gIntMaximoResultadosPP + " más recientes."
                        : "Ordenados del más reciente al más antiguo.";
                    litVacioTituloPP.Text = "Todavía no hay repuestos registrados";
                    litVacioDetallePP.Text = "Usa el botón «Registrar repuesto» para agregar el primero.";
                }
            }
            catch (ClsValidacionExcepcionLNSC lObjExcepcionPP)
            {
                string lStrMensajePP;
                lObjExcepcionPP.gDicErroresCamposSC.TryGetValue(ClsRepuestoLNPP.gStrCampoBusquedaPP, out lStrMensajePP);
                lblErrorBusquedaPP.Text = HttpUtility.HtmlEncode(lStrMensajePP ?? "Revisa el texto de búsqueda.");
                lblErrorBusquedaPP.Visible = true;
                txtBuscarPP.CssClass = "form-control is-invalid";
                pnlResultadosPP.Visible = false;
            }
            catch (Exception lObjExcepcionPP)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionPP, "Inventario.aspx - cargar repuestos");
                gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible consultar el inventario en este momento. Intenta de nuevo en unos minutos.");
                litTotalPP.Text = string.Empty;
                pnlResultadosPP.Visible = false;
            }
        }

        /// <summary>
        /// Si la dirección trae ?entrada=, abre la ventana de entrada con los datos del repuesto.
        /// Si el repuesto no existe, avisa y deja solo el listado.
        /// </summary>
        private void SubAbrirEntradaPP()
        {
            if (Request.QueryString["entrada"] == null)
            {
                return;
            }

            try
            {
                ClsRepuestoENPP lObjRepuestoPP = gObjRepuestoLNPP.FncObtenerRepuestoPP(FncLeerIdEntradaPP());
                if (lObjRepuestoPP == null)
                {
                    gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Advertencia, "El repuesto indicado no existe. Búscalo en el listado.");
                    return;
                }

                litInicialesEntradaPP.Text = FncInicialesPP(lObjRepuestoPP.gStrNombrePP);
                litNombreEntradaPP.Text = lObjRepuestoPP.gStrNombrePP;
                litCodigoEntradaPP.Text = lObjRepuestoPP.gStrCodigoPP;
                litStockEntradaPP.Text = FncTextoStockPP(lObjRepuestoPP.gIntStockPP);
                pnlEntradaPP.Visible = true;
            }
            catch (Exception lObjExcepcionPP)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionPP, "Inventario.aspx - abrir entrada");
                gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Error,
                    "No fue posible cargar el repuesto en este momento. Intenta de nuevo en unos minutos.");
            }
        }

        /// <summary>Muestra el mensaje de éxito después de registrar un repuesto o una entrada.</summary>
        private void SubMostrarAvisoPP()
        {
            int lIntIdRepuestoPP;
            bool lBlnRegistradoPP = int.TryParse(Request.QueryString["registrado"], out lIntIdRepuestoPP);
            if (!lBlnRegistradoPP && !int.TryParse(Request.QueryString["actualizado"], out lIntIdRepuestoPP))
            {
                return;
            }

            try
            {
                // El mensaje se arma con los datos guardados, no con texto de la dirección.
                ClsRepuestoENPP lObjRepuestoPP = gObjRepuestoLNPP.FncObtenerRepuestoPP(lIntIdRepuestoPP);
                if (lObjRepuestoPP == null)
                {
                    return;
                }

                gObjSitioPP.SubMostrarAlertaSC(TipoAlertaSC.Exito, lBlnRegistradoPP
                    ? "Se registró el repuesto «" + lObjRepuestoPP.gStrNombrePP + "» con el código " + lObjRepuestoPP.gStrCodigoPP + "."
                    : "Se actualizó el stock de «" + lObjRepuestoPP.gStrNombrePP + "»: ahora hay " + FncTextoStockPP(lObjRepuestoPP.gIntStockPP) + ".");
            }
            catch (Exception lObjExcepcionPP)
            {
                ClsRegistroErroresSC.SubRegistrarSC(lObjExcepcionPP, "Inventario.aspx - mostrar aviso");
            }
        }

        /// <summary>Lee el identificador del repuesto de ?entrada=; 0 si no es un número válido.</summary>
        private int FncLeerIdEntradaPP()
        {
            int lIntIdRepuestoPP;
            return int.TryParse(Request.QueryString["entrada"], out lIntIdRepuestoPP) ? lIntIdRepuestoPP : 0;
        }

        // ===================== Formato (se usan desde Inventario.aspx) =====================

        /// <summary>Codifica un valor para mostrarlo de forma segura dentro del HTML.</summary>
        protected string FncCodificarPP(object pObjValorPP)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(pObjValorPP));
        }

        /// <summary>Dos primeras letras del nombre, en mayúsculas, para el círculo de la tabla.</summary>
        protected static string FncInicialesPP(string pStrNombrePP)
        {
            string lStrNombrePP = (pStrNombrePP ?? string.Empty).Trim();
            return lStrNombrePP.Substring(0, Math.Min(2, lStrNombrePP.Length)).ToUpperInvariant();
        }

        /// <summary>Texto del stock: «1 ud.», «12 uds.» o «Sin stock».</summary>
        protected static string FncTextoStockPP(int pIntStockPP)
        {
            if (pIntStockPP <= 0)
            {
                return "Sin stock";
            }
            return pIntStockPP == 1 ? "1 ud." : pIntStockPP + " uds.";
        }

        /// <summary>Clase CSS del stock: en rojo cuando no quedan unidades.</summary>
        protected static string FncClaseStockPP(int pIntStockPP)
        {
            return pIntStockPP <= 0 ? "adms-stock adms-stock-agotado" : "adms-stock";
        }

        /// <summary>Dirección que abre la ventana de entrada para un repuesto, conservando la búsqueda.</summary>
        protected string FncUrlEntradaPP(int pIntIdRepuestoPP)
        {
            return gStrUrlVolverPP + (gStrBusquedaPP.Length == 0 ? "?" : "&") + "entrada=" + pIntIdRepuestoPP;
        }
    }
}
