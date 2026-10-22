//==============================================================
// Nombre: Pavel Alexander Palma Carrillo
// Carné: 0900-24-4296
// Tema: AutoManager DMS - Módulo 3 Inventario - Lógica de Negocio
//==============================================================
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    /// <summary>
    /// Reglas del inventario: valida los datos de un repuesto antes de registrarlo, valida las
    /// entradas de stock y coordina las consultas con la Capa de Datos. Cuando un dato no
    /// cumple una regla lanza ClsValidacionExcepcionLNSC con el mensaje de cada campo.
    /// </summary>
    public class ClsRepuestoLNPP
    {
        /// <summary>Cantidad máxima de repuestos que devuelve una consulta.</summary>
        public const int gIntMaximoResultadosPP = 100;

        /// <summary>Longitud máxima del texto de búsqueda.</summary>
        public const int gIntLongitudMaximaBusquedaPP = 50;

        /// <summary>Stock inicial máximo que se acepta al registrar un repuesto.</summary>
        public const int gIntStockMaximoPP = 99999;

        /// <summary>Unidades máximas que se aceptan en una sola entrada a bodega.</summary>
        public const int gIntEntradaMaximaPP = 9999;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLNSC.
        public const string gStrCampoCodigoPP = nameof(ClsRepuestoENPP.gStrCodigoPP);
        public const string gStrCampoNombrePP = nameof(ClsRepuestoENPP.gStrNombrePP);
        public const string gStrCampoDescripcionPP = nameof(ClsRepuestoENPP.gStrDescripcionPP);
        public const string gStrCampoStockPP = nameof(ClsRepuestoENPP.gIntStockPP);
        public const string gStrCampoMarcaPP = nameof(ClsRepuestoENPP.gStrMarcaCarroPP);
        public const string gStrCampoModeloPP = nameof(ClsRepuestoENPP.gStrModeloCarroPP);
        public const string gStrCampoAnioPP = nameof(ClsRepuestoENPP.gIntAnioCarroPP);
        public const string gStrCampoCantidadPP = "Cantidad";
        public const string gStrCampoRepuestoPP = "Repuesto";
        public const string gStrCampoBusquedaPP = "Busqueda";

        private const string gStrMensajeCodigoDuplicadoPP = "Ya existe un repuesto registrado con este código.";

        // Código: letras y números separados por guiones, por ejemplo FIL-ACE-COR15.
        private static readonly Regex gRegCodigoPP = new Regex(@"^[A-Z0-9]+(-[A-Z0-9]+)*$");
        private static readonly Regex gRegNombrePP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .,'/()+&-]*$");
        private static readonly Regex gRegDescripcionPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .,;:'""/()%+&#°-]*$");
        // Mismas reglas que la marca y el modelo de un vehículo (ClsVehiculoLNSC).
        private static readonly Regex gRegMarcaPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .&'-]*$");
        private static readonly Regex gRegModeloPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .'/+-]*$");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos, que int.Parse no convierte.
        private static readonly Regex gRegEnteroPP = new Regex(@"^[0-9]+$");
        private static readonly Regex gRegAnioPP = new Regex(@"^[0-9]{4}$");
        private static readonly Regex gRegEspaciosPP = new Regex(@"\s+");

        private readonly ClsRepuestoDAPP gObjRepuestoDAPP = new ClsRepuestoDAPP();

        /// <summary>Normaliza, valida y registra un repuesto nuevo.</summary>
        /// <param name="pObjRepuestoPP">Datos de texto del formulario.</param>
        /// <param name="pStrStockPP">Stock inicial tal como lo escribió el usuario (vacío = 0).</param>
        /// <param name="pStrAnioPP">Año del vehículo compatible tal como lo escribió el usuario.</param>
        /// <returns>Identificador asignado al nuevo repuesto.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">Algún dato no cumple las reglas o el código ya existe.</exception>
        public int FncRegistrarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP, string pStrStockPP, string pStrAnioPP)
        {
            if (pObjRepuestoPP == null)
            {
                throw new ArgumentNullException(nameof(pObjRepuestoPP));
            }

            SubNormalizarRepuestoPP(pObjRepuestoPP);
            Dictionary<string, string> lDicErroresPP = FncValidarRepuestoPP(pObjRepuestoPP, pStrStockPP, pStrAnioPP);

            if (!lDicErroresPP.ContainsKey(gStrCampoCodigoPP) && gObjRepuestoDAPP.FncExisteCodigoPP(pObjRepuestoPP.gStrCodigoPP))
            {
                lDicErroresPP[gStrCampoCodigoPP] = gStrMensajeCodigoDuplicadoPP;
            }

            if (lDicErroresPP.Count > 0)
            {
                throw new ClsValidacionExcepcionLNSC(lDicErroresPP);
            }

            try
            {
                return gObjRepuestoDAPP.FncAgregarRepuestoPP(pObjRepuestoPP);
            }
            catch (ClsDatoDuplicadoExcepcionDASC)
            {
                // Otro usuario registró el mismo código entre la consulta previa y el INSERT;
                // la restricción UQ_Repuesto_Codigo de SQL Server lo detuvo.
                throw new ClsValidacionExcepcionLNSC(gStrCampoCodigoPP, gStrMensajeCodigoDuplicadoPP);
            }
        }

        /// <summary>
        /// Consulta repuestos por nombre (sin importar tildes ni mayúsculas) o por código exacto.
        /// </summary>
        /// <param name="pStrCriterioPP">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="gIntMaximoResultadosPP"/> repuestos.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">El texto de búsqueda es demasiado largo.</exception>
        public List<ClsRepuestoENPP> FncConsultarRepuestosPP(string pStrCriterioPP)
        {
            string lStrCriterioPP = FncNormalizarEspaciosPP(pStrCriterioPP);
            if (lStrCriterioPP.Length > gIntLongitudMaximaBusquedaPP)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoBusquedaPP,
                    "La búsqueda admite como máximo " + gIntLongitudMaximaBusquedaPP + " caracteres.");
            }

            return gObjRepuestoDAPP.FncConsultarPP(lStrCriterioPP.Length == 0 ? null : lStrCriterioPP, gIntMaximoResultadosPP);
        }

        /// <summary>Obtiene un repuesto por su identificador.</summary>
        /// <param name="pIntIdRepuestoPP">Identificador del repuesto.</param>
        /// <returns>El repuesto o <c>null</c> si no existe.</returns>
        public ClsRepuestoENPP FncObtenerRepuestoPP(int pIntIdRepuestoPP)
        {
            return pIntIdRepuestoPP <= 0 ? null : gObjRepuestoDAPP.FncObtenerRepuestoPP(pIntIdRepuestoPP);
        }

        /// <summary>
        /// Registra una entrada a bodega: suma unidades al stock del repuesto. Las salidas no se
        /// hacen aquí; se descuentan al registrar los repuestos utilizados en una orden.
        /// </summary>
        /// <param name="pIntIdRepuestoPP">Identificador del repuesto.</param>
        /// <param name="pStrCantidadPP">Unidades que ingresan, tal como las escribió el usuario.</param>
        /// <returns>El repuesto con el stock ya actualizado.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">La cantidad no es válida o el repuesto no existe.</exception>
        public ClsRepuestoENPP FncRegistrarEntradaPP(int pIntIdRepuestoPP, string pStrCantidadPP)
        {
            string lStrCantidadPP = (pStrCantidadPP ?? string.Empty).Trim();
            if (lStrCantidadPP.Length == 0)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoCantidadPP, "Escribe cuántas unidades ingresan.");
            }

            int lIntCantidadPP;
            if (!gRegEnteroPP.IsMatch(lStrCantidadPP) || !int.TryParse(lStrCantidadPP, out lIntCantidadPP)
                || lIntCantidadPP < 1 || lIntCantidadPP > gIntEntradaMaximaPP)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoCantidadPP,
                    "La cantidad debe ser un número entero entre 1 y " + gIntEntradaMaximaPP.ToString("N0") + ".");
            }

            if (pIntIdRepuestoPP <= 0 || !gObjRepuestoDAPP.FncActualizarStockPP(pIntIdRepuestoPP, lIntCantidadPP))
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoRepuestoPP, "El repuesto seleccionado ya no existe.");
            }

            return gObjRepuestoDAPP.FncObtenerRepuestoPP(pIntIdRepuestoPP);
        }

        /// <summary>Cuenta los repuestos registrados.</summary>
        /// <returns>Cantidad total de repuestos.</returns>
        public int FncContarRepuestosPP()
        {
            return gObjRepuestoDAPP.FncContarRepuestosPP();
        }

        /// <summary>
        /// Quita espacios sobrantes y deja el código en mayúsculas y sin espacios, para guardar
        /// datos uniformes.
        /// </summary>
        private static void SubNormalizarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP)
        {
            pObjRepuestoPP.gStrCodigoPP = gRegEspaciosPP.Replace(pObjRepuestoPP.gStrCodigoPP ?? string.Empty, string.Empty)
                .ToUpperInvariant();
            pObjRepuestoPP.gStrNombrePP = FncNormalizarEspaciosPP(pObjRepuestoPP.gStrNombrePP);
            pObjRepuestoPP.gStrDescripcionPP = FncNormalizarEspaciosPP(pObjRepuestoPP.gStrDescripcionPP);
            pObjRepuestoPP.gStrMarcaCarroPP = FncNormalizarEspaciosPP(pObjRepuestoPP.gStrMarcaCarroPP);
            pObjRepuestoPP.gStrModeloCarroPP = FncNormalizarEspaciosPP(pObjRepuestoPP.gStrModeloCarroPP);
        }

        /// <summary>Revisa cada campo, devuelve todos los errores y asigna el stock y el año si son válidos.</summary>
        private static Dictionary<string, string> FncValidarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP,
            string pStrStockPP, string pStrAnioPP)
        {
            Dictionary<string, string> lDicErroresPP = new Dictionary<string, string>();

            SubValidarTextoPP(lDicErroresPP, gStrCampoCodigoPP, pObjRepuestoPP.gStrCodigoPP, 15, gRegCodigoPP,
                "El código es obligatorio.",
                "El código admite como máximo 15 caracteres.",
                "El código solo admite letras, números y guiones, por ejemplo FIL-ACE-001.");
            SubValidarTextoPP(lDicErroresPP, gStrCampoNombrePP, pObjRepuestoPP.gStrNombrePP, 50, gRegNombrePP,
                "El nombre es obligatorio.",
                "El nombre admite como máximo 50 caracteres.",
                "El nombre solo puede contener letras, números, espacios y los signos . , ' / ( ) + & -");
            SubValidarTextoPP(lDicErroresPP, gStrCampoDescripcionPP, pObjRepuestoPP.gStrDescripcionPP, 100, gRegDescripcionPP,
                "La descripción es obligatoria.",
                "La descripción admite como máximo 100 caracteres.",
                "La descripción solo puede contener letras, números, espacios y signos de puntuación comunes.");
            SubValidarTextoPP(lDicErroresPP, gStrCampoMarcaPP, pObjRepuestoPP.gStrMarcaCarroPP, 50, gRegMarcaPP,
                "La marca es obligatoria.",
                "La marca admite como máximo 50 caracteres.",
                "La marca solo puede contener letras, números, espacios y los signos . & ' -");
            SubValidarTextoPP(lDicErroresPP, gStrCampoModeloPP, pObjRepuestoPP.gStrModeloCarroPP, 50, gRegModeloPP,
                "El modelo es obligatorio.",
                "El modelo admite como máximo 50 caracteres.",
                "El modelo solo puede contener letras, números, espacios y los signos . ' / + -");

            // Stock inicial: si se deja vacío se registra con 0 unidades.
            string lStrStockPP = (pStrStockPP ?? string.Empty).Trim();
            int lIntStockPP = 0;
            if (lStrStockPP.Length > 0
                && (!gRegEnteroPP.IsMatch(lStrStockPP) || !int.TryParse(lStrStockPP, out lIntStockPP) || lIntStockPP > gIntStockMaximoPP))
            {
                lDicErroresPP[gStrCampoStockPP] =
                    "El stock inicial debe ser un número entero entre 0 y " + gIntStockMaximoPP.ToString("N0") + ".";
            }
            else
            {
                pObjRepuestoPP.gIntStockPP = lIntStockPP;
            }

            // El año sigue la misma regla que los vehículos registrados (ClsVehiculoLNSC).
            string lStrAnioPP = (pStrAnioPP ?? string.Empty).Trim();
            if (lStrAnioPP.Length == 0)
            {
                lDicErroresPP[gStrCampoAnioPP] = "El año es obligatorio.";
            }
            else if (!gRegAnioPP.IsMatch(lStrAnioPP))
            {
                lDicErroresPP[gStrCampoAnioPP] = "Escribe el año con 4 dígitos, por ejemplo 2018.";
            }
            else
            {
                int lIntAnioPP = int.Parse(lStrAnioPP);
                if (ClsVehiculoLNSC.FncEsAnioValidoSC(lIntAnioPP))
                {
                    pObjRepuestoPP.gIntAnioCarroPP = lIntAnioPP;
                }
                else
                {
                    lDicErroresPP[gStrCampoAnioPP] = "El año debe estar entre " + ClsVehiculoLNSC.gIntAnioMinimoSC
                        + " y " + ClsVehiculoLNSC.FncObtenerAnioMaximoSC() + ".";
                }
            }

            return lDicErroresPP;
        }

        /// <summary>Valida un texto obligatorio con una longitud máxima y un formato dado.</summary>
        private static void SubValidarTextoPP(Dictionary<string, string> pDicErroresPP, string pStrCampoPP,
            string pStrValorPP, int pIntLongitudMaximaPP, Regex pRegFormatoPP, string pStrMensajeObligatorioPP,
            string pStrMensajeLongitudPP, string pStrMensajeFormatoPP)
        {
            if (pStrValorPP.Length == 0)
            {
                pDicErroresPP[pStrCampoPP] = pStrMensajeObligatorioPP;
            }
            else if (pStrValorPP.Length > pIntLongitudMaximaPP)
            {
                pDicErroresPP[pStrCampoPP] = pStrMensajeLongitudPP;
            }
            else if (!pRegFormatoPP.IsMatch(pStrValorPP))
            {
                pDicErroresPP[pStrCampoPP] = pStrMensajeFormatoPP;
            }
        }

        /// <summary>Quita espacios al inicio y al final y deja un solo espacio entre palabras.</summary>
        private static string FncNormalizarEspaciosPP(string pStrTextoPP)
        {
            return gRegEspaciosPP.Replace(pStrTextoPP ?? string.Empty, " ").Trim();
        }
    }
}
