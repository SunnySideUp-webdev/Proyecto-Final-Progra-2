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
    /// cumple una regla lanza ClsValidacionExcepcionLN con el mensaje de cada campo.
    /// </summary>
    public class ClsRepuestoLNPP
    {
        /// <summary>Cantidad máxima de repuestos que devuelve una consulta.</summary>
        public const int lIntMaximoResultadosPP = 100;

        /// <summary>Longitud máxima del texto de búsqueda.</summary>
        public const int lIntLongitudMaximaBusquedaPP = 50;

        /// <summary>Stock inicial máximo que se acepta al registrar un repuesto.</summary>
        public const int lIntStockMaximoPP = 99999;

        /// <summary>Unidades máximas que se aceptan en una sola entrada a bodega.</summary>
        public const int lIntEntradaMaximaPP = 9999;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLN.
        public const string lStrCampoCodigoPP = nameof(ClsRepuestoENPP.lStrCodigoPP);
        public const string lStrCampoNombrePP = nameof(ClsRepuestoENPP.lStrNombrePP);
        public const string lStrCampoDescripcionPP = nameof(ClsRepuestoENPP.lStrDescripcionPP);
        public const string lStrCampoStockPP = nameof(ClsRepuestoENPP.lIntStockPP);
        public const string lStrCampoMarcaPP = nameof(ClsRepuestoENPP.lStrMarcaCarroPP);
        public const string lStrCampoModeloPP = nameof(ClsRepuestoENPP.lStrModeloCarroPP);
        public const string lStrCampoAnioPP = nameof(ClsRepuestoENPP.lIntAnioCarroPP);
        public const string lStrCampoCantidadPP = "Cantidad";
        public const string lStrCampoRepuestoPP = "Repuesto";
        public const string lStrCampoBusquedaPP = "Busqueda";

        private const string lStrMensajeCodigoDuplicadoPP = "Ya existe un repuesto registrado con este código.";

        // Código: letras y números separados por guiones, por ejemplo FIL-ACE-COR15.
        private static readonly Regex lRegCodigoPP = new Regex(@"^[A-Z0-9]+(-[A-Z0-9]+)*$");
        private static readonly Regex lRegNombrePP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .,'/()+&-]*$");
        private static readonly Regex lRegDescripcionPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .,;:'""/()%+&#°-]*$");
        // Mismas reglas que la marca y el modelo de un vehículo (ClsVehiculoLNSC).
        private static readonly Regex lRegMarcaPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .&'-]*$");
        private static readonly Regex lRegModeloPP = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .'/+-]*$");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos, que int.Parse no convierte.
        private static readonly Regex lRegEnteroPP = new Regex(@"^[0-9]+$");
        private static readonly Regex lRegAnioPP = new Regex(@"^[0-9]{4}$");
        private static readonly Regex lRegEspaciosPP = new Regex(@"\s+");

        private readonly ClsRepuestoDAPP gObjRepuestoDAPP = new ClsRepuestoDAPP();

        /// <summary>Normaliza, valida y registra un repuesto nuevo.</summary>
        /// <param name="pObjRepuestoPP">Datos de texto del formulario.</param>
        /// <param name="pStrStockPP">Stock inicial tal como lo escribió el usuario (vacío = 0).</param>
        /// <param name="pStrAnioPP">Año del vehículo compatible tal como lo escribió el usuario.</param>
        /// <returns>Identificador asignado al nuevo repuesto.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">Algún dato no cumple las reglas o el código ya existe.</exception>
        public int FncRegistrarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP, string pStrStockPP, string pStrAnioPP)
        {
            if (pObjRepuestoPP == null)
            {
                throw new ArgumentNullException(nameof(pObjRepuestoPP));
            }

            SubNormalizarRepuestoPP(pObjRepuestoPP);
            Dictionary<string, string> lDicErroresPP = FncValidarRepuestoPP(pObjRepuestoPP, pStrStockPP, pStrAnioPP);

            if (!lDicErroresPP.ContainsKey(lStrCampoCodigoPP) && gObjRepuestoDAPP.FncExisteCodigoPP(pObjRepuestoPP.lStrCodigoPP))
            {
                lDicErroresPP[lStrCampoCodigoPP] = lStrMensajeCodigoDuplicadoPP;
            }

            if (lDicErroresPP.Count > 0)
            {
                throw new ClsValidacionExcepcionLN(lDicErroresPP);
            }

            try
            {
                return gObjRepuestoDAPP.FncAgregarRepuestoPP(pObjRepuestoPP);
            }
            catch (ClsDatoDuplicadoExcepcionDA)
            {
                // Otro usuario registró el mismo código entre la consulta previa y el INSERT;
                // la restricción UQ_Repuesto_Codigo de SQL Server lo detuvo.
                throw new ClsValidacionExcepcionLN(lStrCampoCodigoPP, lStrMensajeCodigoDuplicadoPP);
            }
        }

        /// <summary>
        /// Consulta repuestos por nombre (sin importar tildes ni mayúsculas) o por código exacto.
        /// </summary>
        /// <param name="pStrCriterioPP">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="lIntMaximoResultadosPP"/> repuestos.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">El texto de búsqueda es demasiado largo.</exception>
        public List<ClsRepuestoENPP> FncConsultarRepuestosPP(string pStrCriterioPP)
        {
            string lStrCriterioPP = FncNormalizarEspaciosPP(pStrCriterioPP);
            if (lStrCriterioPP.Length > lIntLongitudMaximaBusquedaPP)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoBusquedaPP,
                    "La búsqueda admite como máximo " + lIntLongitudMaximaBusquedaPP + " caracteres.");
            }

            return gObjRepuestoDAPP.FncConsultarPP(lStrCriterioPP.Length == 0 ? null : lStrCriterioPP, lIntMaximoResultadosPP);
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
        /// <exception cref="ClsValidacionExcepcionLN">La cantidad no es válida o el repuesto no existe.</exception>
        public ClsRepuestoENPP FncRegistrarEntradaPP(int pIntIdRepuestoPP, string pStrCantidadPP)
        {
            string lStrCantidadPP = (pStrCantidadPP ?? string.Empty).Trim();
            if (lStrCantidadPP.Length == 0)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoCantidadPP, "Escribe cuántas unidades ingresan.");
            }

            int lIntCantidadPP;
            if (!lRegEnteroPP.IsMatch(lStrCantidadPP) || !int.TryParse(lStrCantidadPP, out lIntCantidadPP)
                || lIntCantidadPP < 1 || lIntCantidadPP > lIntEntradaMaximaPP)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoCantidadPP,
                    "La cantidad debe ser un número entero entre 1 y " + lIntEntradaMaximaPP.ToString("N0") + ".");
            }

            if (pIntIdRepuestoPP <= 0 || !gObjRepuestoDAPP.FncActualizarStockPP(pIntIdRepuestoPP, lIntCantidadPP))
            {
                throw new ClsValidacionExcepcionLN(lStrCampoRepuestoPP, "El repuesto seleccionado ya no existe.");
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
            pObjRepuestoPP.lStrCodigoPP = lRegEspaciosPP.Replace(pObjRepuestoPP.lStrCodigoPP ?? string.Empty, string.Empty)
                .ToUpperInvariant();
            pObjRepuestoPP.lStrNombrePP = FncNormalizarEspaciosPP(pObjRepuestoPP.lStrNombrePP);
            pObjRepuestoPP.lStrDescripcionPP = FncNormalizarEspaciosPP(pObjRepuestoPP.lStrDescripcionPP);
            pObjRepuestoPP.lStrMarcaCarroPP = FncNormalizarEspaciosPP(pObjRepuestoPP.lStrMarcaCarroPP);
            pObjRepuestoPP.lStrModeloCarroPP = FncNormalizarEspaciosPP(pObjRepuestoPP.lStrModeloCarroPP);
        }

        /// <summary>Revisa cada campo, devuelve todos los errores y asigna el stock y el año si son válidos.</summary>
        private static Dictionary<string, string> FncValidarRepuestoPP(ClsRepuestoENPP pObjRepuestoPP,
            string pStrStockPP, string pStrAnioPP)
        {
            Dictionary<string, string> lDicErroresPP = new Dictionary<string, string>();

            SubValidarTextoPP(lDicErroresPP, lStrCampoCodigoPP, pObjRepuestoPP.lStrCodigoPP, 15, lRegCodigoPP,
                "El código es obligatorio.",
                "El código admite como máximo 15 caracteres.",
                "El código solo admite letras, números y guiones, por ejemplo FIL-ACE-001.");
            SubValidarTextoPP(lDicErroresPP, lStrCampoNombrePP, pObjRepuestoPP.lStrNombrePP, 50, lRegNombrePP,
                "El nombre es obligatorio.",
                "El nombre admite como máximo 50 caracteres.",
                "El nombre solo puede contener letras, números, espacios y los signos . , ' / ( ) + & -");
            SubValidarTextoPP(lDicErroresPP, lStrCampoDescripcionPP, pObjRepuestoPP.lStrDescripcionPP, 100, lRegDescripcionPP,
                "La descripción es obligatoria.",
                "La descripción admite como máximo 100 caracteres.",
                "La descripción solo puede contener letras, números, espacios y signos de puntuación comunes.");
            SubValidarTextoPP(lDicErroresPP, lStrCampoMarcaPP, pObjRepuestoPP.lStrMarcaCarroPP, 50, lRegMarcaPP,
                "La marca es obligatoria.",
                "La marca admite como máximo 50 caracteres.",
                "La marca solo puede contener letras, números, espacios y los signos . & ' -");
            SubValidarTextoPP(lDicErroresPP, lStrCampoModeloPP, pObjRepuestoPP.lStrModeloCarroPP, 50, lRegModeloPP,
                "El modelo es obligatorio.",
                "El modelo admite como máximo 50 caracteres.",
                "El modelo solo puede contener letras, números, espacios y los signos . ' / + -");

            // Stock inicial: si se deja vacío se registra con 0 unidades.
            string lStrStockPP = (pStrStockPP ?? string.Empty).Trim();
            int lIntStockPP = 0;
            if (lStrStockPP.Length > 0
                && (!lRegEnteroPP.IsMatch(lStrStockPP) || !int.TryParse(lStrStockPP, out lIntStockPP) || lIntStockPP > lIntStockMaximoPP))
            {
                lDicErroresPP[lStrCampoStockPP] =
                    "El stock inicial debe ser un número entero entre 0 y " + lIntStockMaximoPP.ToString("N0") + ".";
            }
            else
            {
                pObjRepuestoPP.lIntStockPP = lIntStockPP;
            }

            // El año sigue la misma regla que los vehículos registrados (ClsVehiculoLNSC).
            string lStrAnioPP = (pStrAnioPP ?? string.Empty).Trim();
            if (lStrAnioPP.Length == 0)
            {
                lDicErroresPP[lStrCampoAnioPP] = "El año es obligatorio.";
            }
            else if (!lRegAnioPP.IsMatch(lStrAnioPP))
            {
                lDicErroresPP[lStrCampoAnioPP] = "Escribe el año con 4 dígitos, por ejemplo 2018.";
            }
            else
            {
                int lIntAnioPP = int.Parse(lStrAnioPP);
                if (ClsVehiculoLNSC.fncEsAnioValidoSC(lIntAnioPP))
                {
                    pObjRepuestoPP.lIntAnioCarroPP = lIntAnioPP;
                }
                else
                {
                    lDicErroresPP[lStrCampoAnioPP] = "El año debe estar entre " + ClsVehiculoLNSC.lIntAnioMinimoSC
                        + " y " + ClsVehiculoLNSC.fncObtenerAnioMaximoSC() + ".";
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
            return lRegEspaciosPP.Replace(pStrTextoPP ?? string.Empty, " ").Trim();
        }
    }
}
