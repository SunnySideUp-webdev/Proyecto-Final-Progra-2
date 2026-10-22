using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CapaEntidades;
using CapaLogicadeNegocio;

namespace Inventario
{
    public partial class RegistroRepuesto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegresarprincipal_click(object sender, EventArgs e)
        {
            Response.Redirect("Inventario.aspx");
        }

        protected void btnguardarinfo_Click(object sender, EventArgs e)
        {
            try
            {
                ClsCapaEntidades lobjentidadesPP = new ClsCapaEntidades();
                ClslogicaNegocios lobjlogicaPP = new ClslogicaNegocios();

                lobjentidadesPP.gstrcodrepuestoPP = txtcodigo.Text.Trim();
                lobjentidadesPP.gstrnomrepuestoPP = txtnomrepuesto.Text.Trim();
                lobjentidadesPP.gstrdescrepuestoPP = txtdescrepuesto.Text.Trim();
                lobjentidadesPP.gintstockrepuestoPP = string.IsNullOrEmpty(txtstockrepuesto.Text) ? 0 : int.Parse(txtstockrepuesto.Text);
                lobjentidadesPP.gstrmarcacarroPP = txtmarcacompatible.Text.Trim();
                lobjentidadesPP.gstrmodelocarroPP = modelocompatible.Text.Trim();
                lobjentidadesPP.gintaniocarroPP = string.IsNullOrEmpty(txtaniocarro.Text) ? 0 : int.Parse(txtaniocarro.Text);

                string lstrresultadoPP = lobjlogicaPP.fncRegistroRepuestoPP(lobjentidadesPP);

                lblmensaje.Text = lstrresultadoPP;
                lblmensaje.Visible = true;

                if (lstrresultadoPP.Contains("Exito"))
                {
                    fnclimpiarformularioPP();   
                }
            }
            catch (Exception lstrerrrorPP)
            {
                lblmensaje.Text = "error en el ingreso de los datos: " + lstrerrrorPP.Message;
                lblmensaje.Visible = true;
            }
        }

        private void fnclimpiarformularioPP()
        {
            txtcodigo.Text = "";
            txtnomrepuesto.Text = "";
            txtdescrepuesto.Text = "";
            txtstockrepuesto.Text = "";
            txtmarcacompatible.Text = "";
            modelocompatible.Text = "";
            txtaniocarro.Text = "";
        }
    }
}