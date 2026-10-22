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
        public const int gIntAnioMinimoSC = 1981;

        /// <summary>Cantidad máxima de vehículos que devuelve una búsqueda.</summary>
        public const int gIntMaximoResultadosSC = 50;

        /// <summary>Longitud máxima del texto de búsqueda tal como lo escribe el usuario.</summary>
        public const int gIntLongitudMaximaBusquedaSC = 40;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLNSC.
        public const string gStrCampoPropietarioSC = nameof(ClsVehiculoENSC.gIntIdClienteSC);
        public const string gStrCampoPlacaSC = nameof(ClsVehiculoENSC.gStrPlacaSC);
        public const string gStrCampoVinSC = nameof(ClsVehiculoENSC.gStrVinSC);
        public const string gStrCampoMarcaSC = nameof(ClsVehiculoENSC.gStrMarcaSC);
        public const string gStrCampoModeloSC = nameof(ClsVehiculoENSC.gStrModeloSC);
        public const string gStrCampoAnioSC = nameof(ClsVehiculoENSC.gIntAnioSC);
        public const string gStrCampoBusquedaSC = "Busqueda";

        private const string gStrMensajePlacaDuplicadaSC = "Ya existe un vehículo registrado con esta placa.";
        private const string gStrMensajeVinDuplicadoSC = "Ya existe un vehículo registrado con este VIN.";
        private const string gStrRestriccionVinSC = "UQ_Vehiculos_VIN";

        private static readonly Regex gRegSeparadoresSC = new Regex(@"[\s\-.]");
        private static readonly Regex gRegNoAlfanumericoSC = new Regex(@"[^A-Za-z0-9]");
        private static readonly Regex gRegPlacaSC = new Regex(@"^[A-Z0-9]{5,10}$");
        // VIN estándar: 17 caracteres sin I, O ni Q (se confunden con 1 y 0).
        private static readonly Regex gRegVinSC = new Regex(@"^[A-HJ-NPR-Z0-9]{17}$");
        private static readonly Regex gRegMarcaSC = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .&'-]*$");
        private static readonly Regex gRegModeloSC = new Regex(@"^[\p{L}\d][\p{L}\p{M}\d .'/+-]*$");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos, que int.Parse no convierte.
        private static readonly Regex gRegAnioSC = new Regex(@"^[0-9]{4}$");
        private static readonly Regex gRegEspaciosSC = new Regex(@"\s+");

        private readonly ClsVehiculoDASC gObjVehiculoDASC = new ClsVehiculoDASC();
        private readonly ClsClienteDASC gObjClienteDASC = new ClsClienteDASC();

        /// <summary>Normaliza, valida y registra un vehículo asociado a un cliente existente.</summary>
        /// <param name="pObjVehiculoSC">Datos del formulario, con el identificador del propietario.</param>
        /// <param name="pStrAnioSC">Año tal como lo escribió el usuario.</param>
        /// <returns>Identificador asignado al nuevo vehículo.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">Algún dato no cumple las reglas o ya existe.</exception>
        public int FncRegistrarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC, string pStrAnioSC)
        {
            if (pObjVehiculoSC == null)
            {
                throw new ArgumentNullException(nameof(pObjVehiculoSC));
            }

            SubNormalizarVehiculoSC(pObjVehiculoSC);
            Dictionary<string, string> lDicErroresSC = FncValidarVehiculoSC(pObjVehiculoSC, pStrAnioSC);

            // El propietario debe existir en la base, no solo venir en el formulario.
            if (!lDicErroresSC.ContainsKey(gStrCampoPropietarioSC)
                && gObjClienteDASC.FncObtenerClientePorIdSC(pObjVehiculoSC.gIntIdClienteSC) == null)
            {
                lDicErroresSC[gStrCampoPropietarioSC] = "El cliente seleccionado ya no existe. Búscalo de nuevo.";
            }

            if (!lDicErroresSC.ContainsKey(gStrCampoPlacaSC) && gObjVehiculoDASC.FncExistePlacaSC(pObjVehiculoSC.gStrPlacaSC))
            {
                lDicErroresSC[gStrCampoPlacaSC] = gStrMensajePlacaDuplicadaSC;
            }

            if (!lDicErroresSC.ContainsKey(gStrCampoVinSC) && gObjVehiculoDASC.FncExisteVinSC(pObjVehiculoSC.gStrVinSC))
            {
                lDicErroresSC[gStrCampoVinSC] = gStrMensajeVinDuplicadoSC;
            }

            if (lDicErroresSC.Count > 0)
            {
                throw new ClsValidacionExcepcionLNSC(lDicErroresSC);
            }

            try
            {
                return gObjVehiculoDASC.FncInsertarVehiculoSC(pObjVehiculoSC);
            }
            catch (ClsDatoDuplicadoExcepcionDASC lObjExcepcionSC)
            {
                // Otro usuario registró la misma placa o VIN entre la consulta previa y el INSERT;
                // la restricción de SQL Server lo detuvo. Se informa en el campo correspondiente.
                if (lObjExcepcionSC.gStrRestriccionSC == gStrRestriccionVinSC)
                {
                    throw new ClsValidacionExcepcionLNSC(gStrCampoVinSC, gStrMensajeVinDuplicadoSC);
                }
                throw new ClsValidacionExcepcionLNSC(gStrCampoPlacaSC, gStrMensajePlacaDuplicadaSC);
            }
        }

        /// <summary>
        /// Busca vehículos por placa o VIN. Se ignoran espacios, guiones y mayúsculas,
        /// así "p-123 abc" encuentra la placa P123ABC.
        /// </summary>
        /// <param name="pStrTerminoSC">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="gIntMaximoResultadosSC"/> vehículos con su propietario.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">El texto de búsqueda no es válido.</exception>
        public List<ClsVehiculoENSC> FncBuscarVehiculosSC(string pStrTerminoSC)
        {
            string lStrTerminoSC = (pStrTerminoSC ?? string.Empty).Trim();
            if (lStrTerminoSC.Length > gIntLongitudMaximaBusquedaSC)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoBusquedaSC,
                    "La búsqueda admite como máximo " + gIntLongitudMaximaBusquedaSC + " caracteres.");
            }

            string lStrNormalizadoSC = gRegNoAlfanumericoSC.Replace(lStrTerminoSC, string.Empty).ToUpperInvariant();
            if (lStrNormalizadoSC.Length > 17)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoBusquedaSC,
                    "Escribe una placa o un VIN; el VIN tiene como máximo 17 caracteres.");
            }

            return gObjVehiculoDASC.FncBuscarVehiculosSC(
                lStrNormalizadoSC.Length == 0 ? null : lStrNormalizadoSC, gIntMaximoResultadosSC);
        }

        /// <summary>Obtiene un vehículo con los datos de su propietario.</summary>
        /// <param name="pIntIdVehiculoSC">Identificador del vehículo.</param>
        /// <returns>El vehículo o <c>null</c> si no existe.</returns>
        public ClsVehiculoENSC FncObtenerVehiculoSC(int pIntIdVehiculoSC)
        {
            return pIntIdVehiculoSC <= 0 ? null : gObjVehiculoDASC.FncObtenerVehiculoPorIdSC(pIntIdVehiculoSC);
        }

        /// <summary>Lista los vehículos asociados a un cliente.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente propietario.</param>
        /// <returns>Vehículos del cliente; vacío si el identificador no es válido o no tiene vehículos.</returns>
        public List<ClsVehiculoENSC> FncListarVehiculosPorClienteSC(int pIntIdClienteSC)
        {
            return pIntIdClienteSC <= 0
                ? new List<ClsVehiculoENSC>()
                : gObjVehiculoDASC.FncListarVehiculosPorClienteSC(pIntIdClienteSC);
        }

        /// <summary>Cuenta los vehículos registrados.</summary>
        /// <returns>Cantidad total de vehículos.</returns>
        public int FncContarVehiculosSC()
        {
            return gObjVehiculoDASC.FncContarVehiculosSC();
        }

        /// <summary>
        /// Calcula el último año aceptado: el siguiente al de la fecha indicada,
        /// porque los modelos del próximo año se venden antes de que empiece.
        /// </summary>
        /// <param name="pDatFechaReferenciaSC">Fecha con la que se evalúa la regla.</param>
        /// <returns>Año máximo permitido para un vehículo.</returns>
        public static int FncObtenerAnioMaximoSC(DateTime pDatFechaReferenciaSC)
        {
            return pDatFechaReferenciaSC.Year + 1;
        }

        /// <summary>Calcula el último año aceptado según la fecha actual de Guatemala.</summary>
        /// <returns>Año máximo permitido para un vehículo.</returns>
        public static int FncObtenerAnioMaximoSC()
        {
            return FncObtenerAnioMaximoSC(ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC());
        }

        /// <summary>
        /// Indica si un año de modelo está dentro del rango permitido. La validación
        /// vive aquí y no en un CHECK de SQL Server porque depende de la fecha actual.
        /// </summary>
        /// <param name="pIntAnioSC">Año que escribió el usuario.</param>
        /// <param name="pDatFechaReferenciaSC">Fecha con la que se evalúa la regla.</param>
        /// <returns><c>true</c> si el año es válido; de lo contrario, <c>false</c>.</returns>
        public static bool FncEsAnioValidoSC(int pIntAnioSC, DateTime pDatFechaReferenciaSC)
        {
            return pIntAnioSC >= gIntAnioMinimoSC
                && pIntAnioSC <= FncObtenerAnioMaximoSC(pDatFechaReferenciaSC);
        }

        /// <summary>Indica si un año de modelo es válido según la fecha actual de Guatemala.</summary>
        /// <param name="pIntAnioSC">Año que escribió el usuario.</param>
        /// <returns><c>true</c> si el año es válido; de lo contrario, <c>false</c>.</returns>
        public static bool FncEsAnioValidoSC(int pIntAnioSC)
        {
            return FncEsAnioValidoSC(pIntAnioSC, ClsZonaHorariaLNSC.FncObtenerHoyGuatemalaSC());
        }

        /// <summary>Quita separadores y espacios sobrantes para guardar datos uniformes.</summary>
        private static void SubNormalizarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC)
        {
            pObjVehiculoSC.gStrPlacaSC = gRegSeparadoresSC.Replace(pObjVehiculoSC.gStrPlacaSC ?? string.Empty, string.Empty)
                .ToUpperInvariant();

            pObjVehiculoSC.gStrVinSC = gRegSeparadoresSC.Replace(pObjVehiculoSC.gStrVinSC ?? string.Empty, string.Empty)
                .ToUpperInvariant();

            pObjVehiculoSC.gStrMarcaSC = FncNormalizarEspaciosSC(pObjVehiculoSC.gStrMarcaSC);
            pObjVehiculoSC.gStrModeloSC = FncNormalizarEspaciosSC(pObjVehiculoSC.gStrModeloSC);
        }

        /// <summary>Revisa cada campo, devuelve todos los errores y asigna el año si es válido.</summary>
        private static Dictionary<string, string> FncValidarVehiculoSC(ClsVehiculoENSC pObjVehiculoSC, string pStrAnioSC)
        {
            Dictionary<string, string> lDicErroresSC = new Dictionary<string, string>();

            if (pObjVehiculoSC.gIntIdClienteSC <= 0)
            {
                lDicErroresSC[gStrCampoPropietarioSC] = "Selecciona el cliente propietario del vehículo.";
            }

            if (pObjVehiculoSC.gStrPlacaSC.Length == 0)
            {
                lDicErroresSC[gStrCampoPlacaSC] = "La placa es obligatoria.";
            }
            else if (!gRegPlacaSC.IsMatch(pObjVehiculoSC.gStrPlacaSC))
            {
                lDicErroresSC[gStrCampoPlacaSC] = "La placa debe tener entre 5 y 10 letras o números, por ejemplo P-123ABC.";
            }

            if (pObjVehiculoSC.gStrVinSC.Length == 0)
            {
                lDicErroresSC[gStrCampoVinSC] = "El VIN es obligatorio. Está en la tarjeta de circulación o en el parabrisas.";
            }
            else if (!gRegVinSC.IsMatch(pObjVehiculoSC.gStrVinSC))
            {
                lDicErroresSC[gStrCampoVinSC] = pObjVehiculoSC.gStrVinSC.Length != 17
                    ? "El VIN debe tener exactamente 17 caracteres."
                    : "El VIN solo admite letras y números, sin I, O ni Q.";
            }

            SubValidarTextoSC(lDicErroresSC, gStrCampoMarcaSC, pObjVehiculoSC.gStrMarcaSC, gRegMarcaSC,
                "La marca es obligatoria.",
                "La marca admite como máximo 50 caracteres.",
                "La marca solo puede contener letras, números, espacios y los signos . & ' -");
            SubValidarTextoSC(lDicErroresSC, gStrCampoModeloSC, pObjVehiculoSC.gStrModeloSC, gRegModeloSC,
                "El modelo es obligatorio.",
                "El modelo admite como máximo 50 caracteres.",
                "El modelo solo puede contener letras, números, espacios y los signos . ' / + -");

            string lStrAnioSC = (pStrAnioSC ?? string.Empty).Trim();
            int lIntAnioMaximoSC = FncObtenerAnioMaximoSC();
            if (lStrAnioSC.Length == 0)
            {
                lDicErroresSC[gStrCampoAnioSC] = "El año es obligatorio.";
            }
            else if (!gRegAnioSC.IsMatch(lStrAnioSC))
            {
                lDicErroresSC[gStrCampoAnioSC] = "Escribe el año con 4 dígitos, por ejemplo 2018.";
            }
            else
            {
                int lIntAnioSC = int.Parse(lStrAnioSC);
                if (FncEsAnioValidoSC(lIntAnioSC))
                {
                    pObjVehiculoSC.gIntAnioSC = lIntAnioSC;
                }
                else
                {
                    lDicErroresSC[gStrCampoAnioSC] = "El año debe estar entre " + gIntAnioMinimoSC + " y " + lIntAnioMaximoSC + ".";
                }
            }

            return lDicErroresSC;
        }

        /// <summary>Valida un texto obligatorio de hasta 50 caracteres con un formato dado.</summary>
        private static void SubValidarTextoSC(Dictionary<string, string> pDicErroresSC, string pStrCampoSC,
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
        private static string FncNormalizarEspaciosSC(string pStrTextoSC)
        {
            return gRegEspaciosSC.Replace(pStrTextoSC ?? string.Empty, " ").Trim();
        }
    }
}
