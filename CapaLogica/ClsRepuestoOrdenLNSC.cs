//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Repuestos utilizados en órdenes - Lógica de Negocio
//==============================================================
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    /// <summary>
    /// Reglas de los repuestos utilizados en una orden: solo una orden abierta puede recibir o
    /// devolver repuestos, la cantidad debe ser un entero positivo y no puede superar el stock.
    /// Une el módulo de órdenes (Neider) con el inventario (Pavel).
    /// </summary>
    public class ClsRepuestoOrdenLNSC
    {
        /// <summary>Unidades máximas que se aceptan en un solo registro.</summary>
        public const int gIntCantidadMaximaSC = 999;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLNSC.
        public const string gStrCampoOrdenSC = "Orden";
        public const string gStrCampoRepuestoSC = "Repuesto";
        public const string gStrCampoCantidadSC = "Cantidad";

        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos, que int.Parse no convierte.
        private static readonly Regex gRegEnteroSC = new Regex(@"^[0-9]{1,3}$");

        private readonly ClsRepuestoOrdenDASC gObjRepuestoOrdenDASC = new ClsRepuestoOrdenDASC();
        private readonly ClsRepuestoDAPP gObjRepuestoDASC = new ClsRepuestoDAPP();

        /// <summary>Lista los repuestos utilizados en una orden.</summary>
        /// <param name="pIntIdOrdenSC">Identificador de la orden.</param>
        /// <returns>Repuestos de la orden; vacío si el identificador no es válido.</returns>
        public List<ClsRepuestoOrdenENSC> FncListarRepuestosDeOrdenSC(int pIntIdOrdenSC)
        {
            return pIntIdOrdenSC <= 0
                ? new List<ClsRepuestoOrdenENSC>()
                : gObjRepuestoOrdenDASC.FncListarPorOrdenSC(pIntIdOrdenSC);
        }

        /// <summary>Lista los repuestos con unidades disponibles, para elegir cuál se utilizó.</summary>
        /// <returns>Repuestos con stock mayor que cero, ordenados por nombre.</returns>
        public List<ClsRepuestoENPP> FncListarRepuestosDisponiblesSC()
        {
            return gObjRepuestoOrdenDASC.FncListarRepuestosDisponiblesSC();
        }

        /// <summary>
        /// Registra un repuesto utilizado en una orden y lo descuenta del inventario.
        /// </summary>
        /// <param name="pIntIdOrdenSC">Orden de reparación (debe estar abierta).</param>
        /// <param name="pStrIdRepuestoSC">Repuesto elegido en la lista (identificador como texto).</param>
        /// <param name="pStrCantidadSC">Unidades utilizadas, tal como las escribió el usuario.</param>
        /// <exception cref="ClsValidacionExcepcionLNSC">
        /// Falta un dato, la orden no está abierta, el repuesto no existe o el stock no alcanza.
        /// </exception>
        public void SubRegistrarRepuestoSC(int pIntIdOrdenSC, string pStrIdRepuestoSC, string pStrCantidadSC)
        {
            Dictionary<string, string> lDicErroresSC = new Dictionary<string, string>();

            int lIntIdRepuestoSC;
            if (!int.TryParse(pStrIdRepuestoSC, out lIntIdRepuestoSC) || lIntIdRepuestoSC <= 0)
            {
                lDicErroresSC[gStrCampoRepuestoSC] = "Selecciona el repuesto que se utilizó.";
            }

            string lStrCantidadSC = (pStrCantidadSC ?? string.Empty).Trim();
            int lIntCantidadSC = 0;
            if (lStrCantidadSC.Length == 0)
            {
                lDicErroresSC[gStrCampoCantidadSC] = "Escribe cuántas unidades se utilizaron.";
            }
            else if (!gRegEnteroSC.IsMatch(lStrCantidadSC) || int.Parse(lStrCantidadSC) < 1)
            {
                lDicErroresSC[gStrCampoCantidadSC] = "La cantidad debe ser un número entero entre 1 y " + gIntCantidadMaximaSC + ".";
            }
            else
            {
                lIntCantidadSC = int.Parse(lStrCantidadSC);
            }

            if (lDicErroresSC.Count > 0)
            {
                throw new ClsValidacionExcepcionLNSC(lDicErroresSC);
            }

            ResultadoRepuestoOrdenSC lEnmResultadoSC =
                gObjRepuestoOrdenDASC.FncRegistrarUsoSC(pIntIdOrdenSC, lIntIdRepuestoSC, lIntCantidadSC);

            switch (lEnmResultadoSC)
            {
                case ResultadoRepuestoOrdenSC.Correcto:
                    return;
                case ResultadoRepuestoOrdenSC.StockInsuficiente:
                    ClsRepuestoENPP lObjRepuestoSC = gObjRepuestoDASC.FncObtenerRepuestoPP(lIntIdRepuestoSC);
                    throw new ClsValidacionExcepcionLNSC(gStrCampoCantidadSC, lObjRepuestoSC == null
                        ? "No hay suficientes unidades en bodega."
                        : "No hay suficientes unidades: el stock actual de «" + lObjRepuestoSC.gStrNombrePP + "» es "
                            + lObjRepuestoSC.gIntStockPP + ".");
                case ResultadoRepuestoOrdenSC.RepuestoNoExiste:
                    throw new ClsValidacionExcepcionLNSC(gStrCampoRepuestoSC, "El repuesto seleccionado ya no existe.");
                default:
                    throw FncCrearErrorOrdenSC(lEnmResultadoSC, "no se le pueden agregar repuestos");
            }
        }

        /// <summary>Quita un repuesto de una orden abierta y devuelve sus unidades al inventario.</summary>
        /// <param name="pIntIdOrdenSC">Orden de reparación (debe estar abierta).</param>
        /// <param name="pIntIdRepuestoOrdenSC">Registro que se quita.</param>
        /// <exception cref="ClsValidacionExcepcionLNSC">La orden no está abierta o el registro ya no existe.</exception>
        public void SubQuitarRepuestoSC(int pIntIdOrdenSC, int pIntIdRepuestoOrdenSC)
        {
            ResultadoRepuestoOrdenSC lEnmResultadoSC = gObjRepuestoOrdenDASC.FncQuitarUsoSC(pIntIdOrdenSC, pIntIdRepuestoOrdenSC);
            if (lEnmResultadoSC == ResultadoRepuestoOrdenSC.Correcto)
            {
                return;
            }
            if (lEnmResultadoSC == ResultadoRepuestoOrdenSC.UsoNoExiste)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoRepuestoSC, "Ese repuesto ya no está registrado en la orden.");
            }
            throw FncCrearErrorOrdenSC(lEnmResultadoSC, "su lista de repuestos ya no se puede cambiar");
        }

        /// <summary>Mensaje cuando la orden no existe o ya está cerrada.</summary>
        private static ClsValidacionExcepcionLNSC FncCrearErrorOrdenSC(ResultadoRepuestoOrdenSC pEnmResultadoSC, string pStrConsecuenciaSC)
        {
            return new ClsValidacionExcepcionLNSC(gStrCampoOrdenSC, pEnmResultadoSC == ResultadoRepuestoOrdenSC.OrdenNoExiste
                ? "La orden de reparación ya no existe."
                : "La orden ya está cerrada: " + pStrConsecuenciaSC + ".");
        }
    }
}
