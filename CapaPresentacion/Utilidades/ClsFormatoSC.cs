//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Capa de Presentación
//==============================================================
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using CapaLogica;

namespace CapaPresentacion.Utilidades
{
    /// <summary>Da formato legible a los datos guardados sin separadores.</summary>
    public static class ClsFormatoSC
    {
        // Tipo de placa (P, M, C, A, TC...) seguido de 3 números y 3 letras.
        private static readonly Regex lRegPlacaGuatemalaSC = new Regex(@"^([A-Z]{1,3})(\d{3}[A-Z]{3})$");

        /// <summary>Muestra el DPI como 1234 56789 0101.</summary>
        /// <param name="pStrDpiSC">DPI de 13 dígitos.</param>
        /// <returns>DPI con espacios, o el valor original si no tiene 13 caracteres.</returns>
        public static string fncFormatearDpiSC(string pStrDpiSC)
        {
            if (pStrDpiSC == null || pStrDpiSC.Length != 13)
            {
                return pStrDpiSC ?? string.Empty;
            }
            return pStrDpiSC.Substring(0, 4) + " " + pStrDpiSC.Substring(4, 5) + " " + pStrDpiSC.Substring(9, 4);
        }

        /// <summary>Muestra 8 dígitos como 5555-1234 y 11 dígitos con 502 como +502 5555-1234.</summary>
        /// <param name="pStrTelefonoSC">Teléfono guardado solo con dígitos.</param>
        /// <returns>Teléfono con separadores cuando el formato es conocido.</returns>
        public static string fncFormatearTelefonoSC(string pStrTelefonoSC)
        {
            if (string.IsNullOrEmpty(pStrTelefonoSC))
            {
                return string.Empty;
            }
            if (pStrTelefonoSC.Length == 8)
            {
                return pStrTelefonoSC.Substring(0, 4) + "-" + pStrTelefonoSC.Substring(4);
            }
            if (pStrTelefonoSC.Length == 11 && pStrTelefonoSC.StartsWith("502", StringComparison.Ordinal))
            {
                return "+502 " + pStrTelefonoSC.Substring(3, 4) + "-" + pStrTelefonoSC.Substring(7);
            }
            return pStrTelefonoSC;
        }

        /// <summary>
        /// Muestra una fecha guardada en UTC como hora de Guatemala, por ejemplo 19/10/2026 14:35.
        /// Es el único punto de la interfaz donde se convierten fechas.
        /// </summary>
        /// <param name="pDatUtcSC">Fecha en UTC, tal como llega de la base de datos.</param>
        /// <returns>Fecha con formato día/mes/año y hora de Guatemala.</returns>
        public static string fncFormatearFechaSC(DateTime pDatUtcSC)
        {
            return ClsZonaHorariaLN.fncConvertirUtcAGuatemala(pDatUtcSC)
                .ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Muestra la placa con un espacio tras el tipo, como en las placas de Guatemala:
        /// P123ABC se muestra como "P 123ABC". Otros formatos se muestran tal cual.
        /// </summary>
        /// <param name="pStrPlacaSC">Placa normalizada (sin espacios ni guiones).</param>
        /// <returns>Placa legible.</returns>
        public static string fncFormatearPlacaSC(string pStrPlacaSC)
        {
            if (string.IsNullOrEmpty(pStrPlacaSC))
            {
                return string.Empty;
            }
            Match lObjCoincidenciaSC = lRegPlacaGuatemalaSC.Match(pStrPlacaSC);
            return lObjCoincidenciaSC.Success
                ? lObjCoincidenciaSC.Groups[1].Value + " " + lObjCoincidenciaSC.Groups[2].Value
                : pStrPlacaSC;
        }

        /// <summary>
        /// Devuelve el HTML del VIN para una tabla, con letra de ancho fijo.
        /// El VIN se codifica antes de insertarse.
        /// </summary>
        /// <param name="pStrVinSC">VIN del vehículo.</param>
        /// <returns>Fragmento HTML seguro.</returns>
        public static string fncVinHtmlSC(string pStrVinSC)
        {
            return "<span class=\"adms-vin\">" + HttpUtility.HtmlEncode(pStrVinSC ?? string.Empty) + "</span>";
        }

        /// <summary>Obtiene las iniciales para el avatar: primera letra de nombres y de apellidos.</summary>
        /// <param name="pStrNombresSC">Nombres del cliente.</param>
        /// <param name="pStrApellidosSC">Apellidos del cliente.</param>
        /// <returns>Hasta dos letras en mayúscula.</returns>
        public static string fncObtenerInicialesSC(string pStrNombresSC, string pStrApellidosSC)
        {
            string lStrInicialesSC = string.Empty;
            if (!string.IsNullOrEmpty(pStrNombresSC))
            {
                lStrInicialesSC += char.ToUpperInvariant(pStrNombresSC[0]);
            }
            if (!string.IsNullOrEmpty(pStrApellidosSC))
            {
                lStrInicialesSC += char.ToUpperInvariant(pStrApellidosSC[0]);
            }
            return lStrInicialesSC;
        }

        /// <summary>Muestra una fecha larga en español, por ejemplo "lunes, 19 de octubre de 2026".</summary>
        /// <param name="pDatFechaSC">Fecha ya expresada en hora de Guatemala (no se convierte).</param>
        /// <returns>Fecha larga en español de Guatemala.</returns>
        public static string fncFormatearFechaLargaSC(DateTime pDatFechaSC)
        {
            return pDatFechaSC.ToString("dddd, d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-GT"));
        }

        /// <summary>Devuelve el texto o un reemplazo cuando el dato opcional no existe.</summary>
        /// <param name="pStrValorSC">Dato opcional.</param>
        /// <param name="pStrReemplazoSC">Texto a mostrar cuando el dato está vacío.</param>
        /// <returns>El dato o el reemplazo.</returns>
        public static string fncTextoOpcionalSC(string pStrValorSC, string pStrReemplazoSC)
        {
            return string.IsNullOrWhiteSpace(pStrValorSC) ? pStrReemplazoSC : pStrValorSC;
        }
    }
}
