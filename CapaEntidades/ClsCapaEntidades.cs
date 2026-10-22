using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidades
{
    public class ClsCapaEntidades
    {
        //datos de los repuestos en inventario
        public string gstrcodrepuestoPP { get; set; }
        public string gstrnomrepuestoPP { get; set; }
        public string gstrdescrepuestoPP { get; set; }
        public int gintstockrepuestoPP { get; set; }
        public int gintnuevostockPP { get; set; }

        //compatibilidad que tiene con el carro
        public string gstrmarcacarroPP { get; set; }
        public string gstrmodelocarroPP { get; set; }
        public int gintaniocarroPP { get; set; }

        /// <summary>
        ///  generamos el constructor para el ingreso de los datos para el inventario de los respuestos
        /// </summary>
        public ClsCapaEntidades() : this("", "", "", 0, "", "", 0, 0) { }

        public ClsCapaEntidades(string pstrcodrespuestoPP, string pstrnomrepuestoPP, string pstrdescrrepuestoPP, int pintstockrepuestoPP, string pstrmarcarroPP, string pstrmodelocarroPP, int pintaniocarroPP, int pintnuevostockPP)
        {
            this.gstrcodrepuestoPP = pstrcodrespuestoPP;
            this.gstrnomrepuestoPP = pstrnomrepuestoPP;
            this.gstrdescrepuestoPP = pstrdescrrepuestoPP;
            this.gintstockrepuestoPP = pintstockrepuestoPP;

            this.gstrmarcacarroPP = pstrmarcarroPP;
            this.gstrmodelocarroPP = pstrmodelocarroPP;
            this.gintaniocarroPP = pintaniocarroPP;
            this.gintnuevostockPP = pintnuevostockPP;
        }
    }
}
