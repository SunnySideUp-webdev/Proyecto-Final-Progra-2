//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Lógica de Negocio
//==============================================================
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    /// <summary>
    /// Aplica las reglas de negocio de los vehículos y coordina el intercambio
    /// de información entre la Capa de Presentación y la Capa de Datos.
    /// </summary>
    public class ClsVehiculoLNSC
    {
        /// <summary>
        /// Primer año aceptado: desde 1981 los vehículos tienen VIN de 17 caracteres, que es
        /// obligatorio. Coincide con la restricción CK_Vehiculos_Anio de SQL Server.
        /// </summary>
        public const int lIntAnioMinimoSC = 1981;

        /// <summary>Cantidad máxima de vehículos que devuelve una búsqueda.</summary>
        public const int lIntMaximoResultadosSC = 50;

        /// <summary>Longitud máxima del texto de búsqueda tal como lo escribe el usuario.</summary>
        public const int lIntLongitudMaximaBusquedaSC = 40;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLN.
        public const string lStrCampoPropietarioSC = nameof(ClsVehiculoENSC.lIntIdClienteSC);
        public const string lStrCampoPlacaSC = nameof(ClsVehiculoENSC.lStrPlacaSC);
        public const string lStrCampoVinSC = nameof(ClsVehiculoENSC.lStrVinSC);
        public const string lStrCampoMarcaSC = nameof(ClsVehiculoENSC.lStrMarcaSC);
        public const string lStrCampoModeloSC = nameof(ClsVehiculoENSC.lStrModeloSC);
        public const string lStrCampoAnioSC = nameof(ClsVehiculoENSC.lIntAnioSC);
        public const string lStrCampoBusquedaSC = "Busqueda";

        private const string lStrMensajePlacaDuplicadaSC = "Ya existe un vehículo registrado con esta placa.";
        private const string lStrMensajeVinDuplicadoSC = "Ya existe un vehículo registrado con este VIN.";
        private const string lStrRestriccionVinSC = "UQ_Vehiculos_VIN";

        private static readonly Regex lRegSeparadoresSC = new Regex(@"[\s\-.]");
        private static readonly Regex lRegNoAlfanumericoSC = new Regex(@"[^A-Za-z0-9]");
        private static readonly Regex lRegPlacaSC = new Regex(@"^[A-Z0-9]{5,10}$");
        // VIN estándar: 17 caracteres sin I, O ni Q (se confunden con 1 y 0).
        private static readonly Regex lRegVinSC = new Regex(@"^[A-HJ-NPR-Z0-9]{17}$");
        private static readonly Regex lRegMarcaSC = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .&'-]*$");
        private static readonly Regex lRegModeloSC = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .'/+-]*$");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos, que int.Parse no convierte.
        private static readonly Regex lRegAnioSC = new Regex(@"^[0-9]{4}$");
        private static readonly Regex lRegEspaciosSC = new Regex(@"\s+");

        private readonly ClsVehiculoDASC lObjVehiculoDASC = new ClsVehiculoDASC();
        private readonly ClsClienteDASC lObjClienteDASC = new ClsClienteDASC();

        /// <summary>Normaliza, valida y registra un vehículo asociado a un cliente existente.</summary>
        /// <param name="pObjVehiculoSC">Datos del formulario, con el identificador del propietario.</param>
        /// <param name="pStrAnioSC">Año tal como lo escribió el usuario.</param>
        /// <returns>Identificador asignado al nuevo vehículo.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">Algún dato no cumple las reglas o ya existe.</exception>
        public int fncRegistrarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC, string pStrAnioSC)
        {
            if (pObjVehiculoSC == null)
            {
                throw new ArgumentNullException(nameof(pObjVehiculoSC));
            }

            fncNormalizarVehiculoSC(pObjVehiculoSC);
            Dictionary<string, string> lDicErroresSC = fncValidarVehiculoSC(pObjVehiculoSC, pStrAnioSC);

            // El propietario debe existir en la base, no solo venir en el formulario.
            if (!lDicErroresSC.ContainsKey(lStrCampoPropietarioSC)
                && lObjClienteDASC.fncObtenerClientePorIdSC(pObjVehiculoSC.lIntIdClienteSC) == null)
            {
                lDicErroresSC[lStrCampoPropietarioSC] = "El cliente seleccionado ya no existe. Búscalo de nuevo.";
            }

            if (!lDicErroresSC.ContainsKey(lStrCampoPlacaSC) && lObjVehiculoDASC.fncExistePlacaSC(pObjVehiculoSC.lStrPlacaSC))
            {
                lDicErroresSC[lStrCampoPlacaSC] = lStrMensajePlacaDuplicadaSC;
            }

            if (!lDicErroresSC.ContainsKey(lStrCampoVinSC) && lObjVehiculoDASC.fncExisteVinSC(pObjVehiculoSC.lStrVinSC))
            {
                lDicErroresSC[lStrCampoVinSC] = lStrMensajeVinDuplicadoSC;
            }

            if (lDicErroresSC.Count > 0)
            {
                throw new ClsValidacionExcepcionLN(lDicErroresSC);
            }

            try
            {
                return lObjVehiculoDASC.fncInsertarVehiculoSC(pObjVehiculoSC);
            }
            catch (ClsDatoDuplicadoExcepcionDA lObjExcepcionSC)
            {
                // Otro usuario registró la misma placa o VIN entre la consulta previa y el INSERT;
                // la restricción de SQL Server lo detuvo. Se informa en el campo correspondiente.
                if (lObjExcepcionSC.lStrRestriccion == lStrRestriccionVinSC)
                {
                    throw new ClsValidacionExcepcionLN(lStrCampoVinSC, lStrMensajeVinDuplicadoSC);
                }
                throw new ClsValidacionExcepcionLN(lStrCampoPlacaSC, lStrMensajePlacaDuplicadaSC);
            }
        }

        /// <summary>
        /// Busca vehículos por placa o VIN. Se ignoran espacios, guiones y mayúsculas,
        /// así "p-123 abc" encuentra la placa P123ABC.
        /// </summary>
        /// <param name="pStrTerminoSC">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="lIntMaximoResultadosSC"/> vehículos con su propietario.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">El texto de búsqueda no es válido.</exception>
        public List<ClsVehiculoENSC> fncBuscarVehiculosSC(string pStrTerminoSC)
        {
            string lStrTerminoSC = (pStrTerminoSC ?? string.Empty).Trim();
            if (lStrTerminoSC.Length > lIntLongitudMaximaBusquedaSC)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoBusquedaSC,
                    "La búsqueda admite como máximo " + lIntLongitudMaximaBusquedaSC + " caracteres.");
            }

            string lStrNormalizadoSC = lRegNoAlfanumericoSC.Replace(lStrTerminoSC, string.Empty).ToUpperInvariant();
            if (lStrNormalizadoSC.Length > 17)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoBusquedaSC,
                    "Escribe una placa o un VIN; el VIN tiene como máximo 17 caracteres.");
            }

            return lObjVehiculoDASC.fncBuscarVehiculosSC(
                lStrNormalizadoSC.Length == 0 ? null : lStrNormalizadoSC, lIntMaximoResultadosSC);
        }

        /// <summary>Obtiene un vehículo con los datos de su propietario.</summary>
        /// <param name="pIntIdVehiculoSC">Identificador del vehículo.</param>
        /// <returns>El vehículo o <c>null</c> si no existe.</returns>
        public ClsVehiculoENSC fncObtenerVehiculoSC(int pIntIdVehiculoSC)
        {
            return pIntIdVehiculoSC <= 0 ? null : lObjVehiculoDASC.fncObtenerVehiculoPorIdSC(pIntIdVehiculoSC);
        }

        /// <summary>Lista los vehículos asociados a un cliente.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente propietario.</param>
        /// <returns>Vehículos del cliente; vacío si el identificador no es válido o no tiene vehículos.</returns>
        public List<ClsVehiculoENSC> fncListarVehiculosPorClienteSC(int pIntIdClienteSC)
        {
            return pIntIdClienteSC <= 0
                ? new List<ClsVehiculoENSC>()
                : lObjVehiculoDASC.fncListarVehiculosPorClienteSC(pIntIdClienteSC);
        }

        /// <summary>Cuenta los vehículos registrados.</summary>
        /// <returns>Cantidad total de vehículos.</returns>
        public int fncContarVehiculosSC()
        {
            return lObjVehiculoDASC.fncContarVehiculosSC();
        }

        /// <summary>
        /// Calcula el último año aceptado: el siguiente al de la fecha indicada,
        /// porque los modelos del próximo año se venden antes de que empiece.
        /// </summary>
        /// <param name="pDatFechaReferenciaSC">Fecha con la que se evalúa la regla.</param>
        /// <returns>Año máximo permitido para un vehículo.</returns>
        public static int fncObtenerAnioMaximoSC(DateTime pDatFechaReferenciaSC)
        {
            return pDatFechaReferenciaSC.Year + 1;
        }

        /// <summary>Calcula el último año aceptado según la fecha actual de Guatemala.</summary>
        /// <returns>Año máximo permitido para un vehículo.</returns>
        public static int fncObtenerAnioMaximoSC()
        {
            return fncObtenerAnioMaximoSC(ClsZonaHorariaLN.fncObtenerHoyGuatemala());
        }

        /// <summary>
        /// Indica si un año de modelo está dentro del rango permitido. La validación
        /// vive aquí y no en un CHECK de SQL Server porque depende de la fecha actual.
        /// </summary>
        /// <param name="pIntAnioSC">Año que escribió el usuario.</param>
        /// <param name="pDatFechaReferenciaSC">Fecha con la que se evalúa la regla.</param>
        /// <returns><c>true</c> si el año es válido; de lo contrario, <c>false</c>.</returns>
        public static bool fncEsAnioValidoSC(int pIntAnioSC, DateTime pDatFechaReferenciaSC)
        {
            return pIntAnioSC >= lIntAnioMinimoSC
                && pIntAnioSC <= fncObtenerAnioMaximoSC(pDatFechaReferenciaSC);
        }

        /// <summary>Indica si un año de modelo es válido según la fecha actual de Guatemala.</summary>
        /// <param name="pIntAnioSC">Año que escribió el usuario.</param>
        /// <returns><c>true</c> si el año es válido; de lo contrario, <c>false</c>.</returns>
        public static bool fncEsAnioValidoSC(int pIntAnioSC)
        {
            return fncEsAnioValidoSC(pIntAnioSC, ClsZonaHorariaLN.fncObtenerHoyGuatemala());
        }

        /// <summary>Quita separadores y espacios sobrantes para guardar datos uniformes.</summary>
        private static void fncNormalizarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC)
        {
            pObjVehiculoSC.lStrPlacaSC = lRegSeparadoresSC.Replace(pObjVehiculoSC.lStrPlacaSC ?? string.Empty, string.Empty)
                .ToUpperInvariant();

            pObjVehiculoSC.lStrVinSC = lRegSeparadoresSC.Replace(pObjVehiculoSC.lStrVinSC ?? string.Empty, string.Empty)
                .ToUpperInvariant();

            pObjVehiculoSC.lStrMarcaSC = fncNormalizarEspaciosSC(pObjVehiculoSC.lStrMarcaSC);
            pObjVehiculoSC.lStrModeloSC = fncNormalizarEspaciosSC(pObjVehiculoSC.lStrModeloSC);
        }

        /// <summary>Revisa cada campo, devuelve todos los errores y asigna el año si es válido.</summary>
        private static Dictionary<string, string> fncValidarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC, string pStrAnioSC)
        {
            Dictionary<string, string> lDicErroresSC = new Dictionary<string, string>();

            if (pObjVehiculoSC.lIntIdClienteSC <= 0)
            {
                lDicErroresSC[lStrCampoPropietarioSC] = "Selecciona el cliente propietario del vehículo.";
            }

            if (pObjVehiculoSC.lStrPlacaSC.Length == 0)
            {
                lDicErroresSC[lStrCampoPlacaSC] = "La placa es obligatoria.";
            }
            else if (!lRegPlacaSC.IsMatch(pObjVehiculoSC.lStrPlacaSC))
            {
                lDicErroresSC[lStrCampoPlacaSC] = "La placa debe tener entre 5 y 10 letras o números, por ejemplo P-123ABC.";
            }

            if (pObjVehiculoSC.lStrVinSC.Length == 0)
            {
                lDicErroresSC[lStrCampoVinSC] = "El VIN es obligatorio. Está en la tarjeta de circulación o en el parabrisas.";
            }
            else if (!lRegVinSC.IsMatch(pObjVehiculoSC.lStrVinSC))
            {
                lDicErroresSC[lStrCampoVinSC] = pObjVehiculoSC.lStrVinSC.Length != 17
                    ? "El VIN debe tener exactamente 17 caracteres."
                    : "El VIN solo admite letras y números, sin I, O ni Q.";
            }

            fncValidarTextoSC(lDicErroresSC, lStrCampoMarcaSC, pObjVehiculoSC.lStrMarcaSC, lRegMarcaSC,
                "La marca es obligatoria.",
                "La marca admite como máximo 50 caracteres.",
                "La marca solo puede contener letras, números, espacios y los signos . & ' -");
            fncValidarTextoSC(lDicErroresSC, lStrCampoModeloSC, pObjVehiculoSC.lStrModeloSC, lRegModeloSC,
                "El modelo es obligatorio.",
                "El modelo admite como máximo 50 caracteres.",
                "El modelo solo puede contener letras, números, espacios y los signos . ' / + -");

            string lStrAnioSC = (pStrAnioSC ?? string.Empty).Trim();
            int lIntAnioMaximoSC = fncObtenerAnioMaximoSC();
            if (lStrAnioSC.Length == 0)
            {
                lDicErroresSC[lStrCampoAnioSC] = "El año es obligatorio.";
            }
            else if (!lRegAnioSC.IsMatch(lStrAnioSC))
            {
                lDicErroresSC[lStrCampoAnioSC] = "Escribe el año con 4 dígitos, por ejemplo 2018.";
            }
            else
            {
                int lIntAnioSC = int.Parse(lStrAnioSC);
                if (fncEsAnioValidoSC(lIntAnioSC))
                {
                    pObjVehiculoSC.lIntAnioSC = lIntAnioSC;
                }
                else
                {
                    lDicErroresSC[lStrCampoAnioSC] = "El año debe estar entre " + lIntAnioMinimoSC + " y " + lIntAnioMaximoSC + ".";
                }
            }

            return lDicErroresSC;
        }

        /// <summary>Valida un texto obligatorio de hasta 50 caracteres con un formato dado.</summary>
        private static void fncValidarTextoSC(Dictionary<string, string> pDicErroresSC, string pStrCampoSC,
            string pStrValorSC, Regex pRegFormatoSC, string pStrMensajeObligatorioSC,
            string pStrMensajeLongitudSC, string pStrMensajeFormatoSC)
        {
            if (pStrValorSC.Length == 0)
            {
                pDicErroresSC[pStrCampoSC] = pStrMensajeObligatorioSC;
            }
            else if (pStrValorSC.Length > 50)
            {
                pDicErroresSC[pStrCampoSC] = pStrMensajeLongitudSC;
            }
            else if (!pRegFormatoSC.IsMatch(pStrValorSC))
            {
                pDicErroresSC[pStrCampoSC] = pStrMensajeFormatoSC;
            }
        }

        /// <summary>Quita espacios al inicio y al final y deja un solo espacio entre palabras.</summary>
        private static string fncNormalizarEspaciosSC(string pStrTextoSC)
        {
            return lRegEspaciosSC.Replace(pStrTextoSC ?? string.Empty, " ").Trim();
        }
    }
}
