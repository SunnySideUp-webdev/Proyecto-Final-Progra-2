using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogicadeNegocio;

namespace Inventario
{
    public partial class Inventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            fnctablaactualizadaPP();
        }

        private void fnctablaactualizadaPP()
        {
            try
            {
                ClslogicaNegocios lobjlogicaPP = new ClslogicaNegocios();

                DataTable llistresultadoPP = lobjlogicaPP.fncconsultarbdPP("");

                rptrepuesto.DataSource = llistresultadoPP;
                rptrepuesto.DataBind();

                lbltotalregistro.Text = llistresultadoPP.Rows.Count + " repuestos registrados";
                lbltotalregistro.Visible = true;
            }
            catch (Exception ex)
            {
                lblmensaje.Text = "Error al cargar inventario inicial: " + ex.Message;
                lblmensaje.Visible = true;
            }
        }

        protected void btnRegistrarcliente_click(object sender, EventArgs e)
        {
            Response.Redirect("RegistroRepuesto.aspx");
        }

        protected void btnbuscar_Click(object sender, EventArgs e)
        {
            ClslogicaNegocios lobjlogicaPP = new ClslogicaNegocios();
            try
            {
                string lstrcriterioPP = txtbuscar.Text.Trim();
                DataTable llistresultadoPP = lobjlogicaPP.fncconsultarbdPP(lstrcriterioPP);
                rptrepuesto.DataSource = llistresultadoPP;
                rptrepuesto.DataBind();

                lbltotalregistro.Text = llistresultadoPP.Rows.Count + "repuestos encontrados";
                if(llistresultadoPP.Rows.Count == 0)
                {
                    lblmensaje.Text = "no se encontraron repuestos con el criterio ingresado";
                    lblmensaje.Visible = true;
                }
                else
                {
                    lblmensaje.Visible = false;
                }
            }
            catch (Exception lstrerrorPP)
            {
                lblmensaje.Text = "Error al actualizar la busqueda: " + lstrerrorPP.Message;
                lblmensaje.Visible = true;
            }
        }

        protected void lnkactustock_Click(object sender, EventArgs e)
        {
            LinkButton btnactualizar = (LinkButton)sender;

            string[] lstrargumentoPP = btnactualizar.CommandArgument.Split('|');
            string lstrcodigoPP = lstrargumentoPP[0];
            string lstrnombrePP = lstrargumentoPP[1];

            hndcodigorepuesto.Value = lstrcodigoPP;
            txtnombrerepuesto.Text = lstrnombrePP;
            txtnuevostock.Text = "";

            string script = "var myModal = new bootstrap.Modal(document.getElementById('modalStock')); myModal.show();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", script, true);
        }

        protected void btnconfirmarstock_Click(object sender, EventArgs e)
        {
            ClslogicaNegocios lobjlogicaPP = new ClslogicaNegocios();
            try
            {
                string lstrcodigoPP = hndcodigorepuesto.Value;
                int lintnuevostockPP = string.IsNullOrEmpty(txtnuevostock.Text) ? 0 : int.Parse(txtnuevostock.Text);

                string lstrresultadoPP = lobjlogicaPP.fncactualizastockPP(lstrcodigoPP, lintnuevostockPP);

                lblmensaje.Text = lstrresultadoPP;
                lblmensaje.Visible = true;

                if (lstrresultadoPP.Contains("exito"))
                {
                    btnbuscar_Click(null, null);
                }
            }catch (Exception lstrerrorPP)
            {
                lblmensaje.Text = "Error al actualizar la busqueda: " + lstrerrorPP.Message;
                lblmensaje.Visible = true;
            }
        }
    }
}