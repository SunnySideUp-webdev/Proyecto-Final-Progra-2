//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Lógica de Negocio
//==============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CapaDatos;
using CapaEntidades;

namespace CapaLogica
{
    /// <summary>
    /// Aplica las reglas de negocio de los clientes y coordina el intercambio
    /// de información entre la Capa de Presentación y la Capa de Datos.
    /// </summary>
    public class ClsClienteLNSC
    {
        /// <summary>Cantidad máxima de clientes que devuelve una búsqueda.</summary>
        public const int gIntMaximoResultadosSC = 50;

        /// <summary>Longitud máxima del texto de búsqueda.</summary>
        public const int gIntLongitudMaximaBusquedaSC = 80;

        /// <summary>Cantidad máxima de palabras en una búsqueda.</summary>
        public const int gIntMaximoPalabrasSC = 5;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLNSC.
        public const string gStrCampoDpiSC = nameof(ClsClienteENSC.gStrDpiSC);
        public const string gStrCampoNombresSC = nameof(ClsClienteENSC.gStrNombresSC);
        public const string gStrCampoApellidosSC = nameof(ClsClienteENSC.gStrApellidosSC);
        public const string gStrCampoTelefonoSC = nameof(ClsClienteENSC.gStrTelefonoSC);
        public const string gStrCampoCorreoSC = nameof(ClsClienteENSC.gStrCorreoSC);
        public const string gStrCampoDireccionSC = nameof(ClsClienteENSC.gStrDireccionSC);
        public const string gStrCampoBusquedaSC = "Busqueda";

        private const string gStrMensajeDpiDuplicadoSC = "Ya existe un cliente registrado con este DPI.";

        // Letras (con tildes y ñ), espacios, apóstrofo, punto y guion; debe empezar con letra.
        private static readonly Regex gRegNombreSC = new Regex(@"^\p{L}[\p{L}\p{M} .'-]*$");
        private static readonly Regex gRegCorreoSC = new Regex(@"^[^@\s<>]+@[^@\s<>]+\.[^@\s<>]+$");
        private static readonly Regex gRegSeparadoresDpiSC = new Regex(@"[\s-]");
        private static readonly Regex gRegSeparadoresTelefonoSC = new Regex(@"[\s\-().+]");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos (por ejemplo ١٢٣),
        // que SQL Server rechazaría con CK_Clientes_DPI o CK_Clientes_Telefono.
        private static readonly Regex gRegSoloDigitosSC = new Regex(@"^[0-9]+$");
        private static readonly Regex gRegTerminoNumericoSC = new Regex(@"^[\d\s\-().+]+$");
        private static readonly Regex gRegEspaciosSC = new Regex(@"\s+");

        private readonly ClsClienteDASC gObjClienteDASC = new ClsClienteDASC();

        /// <summary>Normaliza, valida y registra un cliente nuevo.</summary>
        /// <param name="pObjClienteSC">Datos escritos en el formulario.</param>
        /// <returns>Identificador asignado al nuevo cliente.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">Algún dato no cumple las reglas o el DPI ya existe.</exception>
        public int FncRegistrarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            if (pObjClienteSC == null)
            {
                throw new ArgumentNullException(nameof(pObjClienteSC));
            }

            SubNormalizarClienteSC(pObjClienteSC);
            Dictionary<string, string> lDicErroresSC = FncValidarClienteSC(pObjClienteSC);

            // Solo se consulta el duplicado si el DPI tiene un formato válido.
            if (!lDicErroresSC.ContainsKey(gStrCampoDpiSC) && gObjClienteDASC.FncExisteDpiSC(pObjClienteSC.gStrDpiSC))
            {
                lDicErroresSC[gStrCampoDpiSC] = gStrMensajeDpiDuplicadoSC;
            }

            if (lDicErroresSC.Count > 0)
            {
                throw new ClsValidacionExcepcionLNSC(lDicErroresSC);
            }

            try
            {
                return gObjClienteDASC.FncInsertarClienteSC(pObjClienteSC);
            }
            catch (ClsDatoDuplicadoExcepcionDASC)
            {
                // Otro usuario registró el mismo DPI entre la consulta previa y el INSERT;
                // la restricción UQ_Clientes_DPI de SQL Server lo detuvo.
                throw new ClsValidacionExcepcionLNSC(gStrCampoDpiSC, gStrMensajeDpiDuplicadoSC);
            }
        }

        /// <summary>
        /// Busca clientes por palabras: cada palabra debe aparecer en el DPI, el teléfono,
        /// los nombres o los apellidos (por ejemplo, "Pérez Ana" o "Ana 5555"). Si el texto
        /// solo tiene números y separadores, se busca como un único número sin separadores.
        /// </summary>
        /// <param name="pStrTerminoSC">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="gIntMaximoResultadosSC"/> clientes.</returns>
        /// <exception cref="ClsValidacionExcepcionLNSC">El texto es demasiado largo o tiene demasiadas palabras.</exception>
        public List<ClsClienteENSC> FncBuscarClientesSC(string pStrTerminoSC)
        {
            string lStrTerminoSC = FncNormalizarEspaciosSC(pStrTerminoSC);

            if (lStrTerminoSC.Length > gIntLongitudMaximaBusquedaSC)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoBusquedaSC,
                    "La búsqueda admite como máximo " + gIntLongitudMaximaBusquedaSC + " caracteres.");
            }

            List<string> lLstPalabrasSC;
            if (gRegTerminoNumericoSC.IsMatch(lStrTerminoSC))
            {
                string lStrDigitosSC = Regex.Replace(lStrTerminoSC, @"\D", string.Empty);
                lLstPalabrasSC = lStrDigitosSC.Length == 0 ? new List<string>() : new List<string> { lStrDigitosSC };
            }
            else
            {
                lLstPalabrasSC = lStrTerminoSC.Length == 0
                    ? new List<string>()
                    : lStrTerminoSC.Split(' ').Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }

            if (lLstPalabrasSC.Count > gIntMaximoPalabrasSC)
            {
                throw new ClsValidacionExcepcionLNSC(gStrCampoBusquedaSC,
                    "Escribe como máximo " + gIntMaximoPalabrasSC + " palabras en la búsqueda.");
            }

            return gObjClienteDASC.FncBuscarClientesSC(lLstPalabrasSC, gIntMaximoResultadosSC);
        }

        /// <summary>Obtiene los datos de un cliente.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente.</param>
        /// <returns>El cliente o <c>null</c> si no existe.</returns>
        public ClsClienteENSC FncObtenerClienteSC(int pIntIdClienteSC)
        {
            return pIntIdClienteSC <= 0 ? null : gObjClienteDASC.FncObtenerClientePorIdSC(pIntIdClienteSC);
        }

        /// <summary>Cuenta los clientes registrados.</summary>
        /// <returns>Cantidad total de clientes.</returns>
        public int FncContarClientesSC()
        {
            return gObjClienteDASC.FncContarClientesSC();
        }

        /// <summary>
        /// Cuenta los clientes registrados desde el primer día del mes actual en Guatemala.
        /// El inicio del mes se convierte a UTC porque así se guardan las fechas.
        /// </summary>
        /// <returns>Cantidad de clientes nuevos en el mes.</returns>
        public int FncContarClientesDelMesSC()
        {
            DateTime lDatAhoraSC = ClsZonaHorariaLNSC.FncObtenerAhoraGuatemalaSC();
            DateTime lDatInicioMesSC = new DateTime(lDatAhoraSC.Year, lDatAhoraSC.Month, 1);
            return gObjClienteDASC.FncContarClientesDesdeSC(ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC(lDatInicioMesSC));
        }

        /// <summary>Obtiene los últimos clientes registrados.</summary>
        /// <param name="pIntCantidadSC">Cantidad de clientes a devolver (1 a 50).</param>
        /// <returns>Clientes ordenados del más reciente al más antiguo.</returns>
        public List<ClsClienteENSC> FncObtenerClientesRecientesSC(int pIntCantidadSC)
        {
            if (pIntCantidadSC < 1 || pIntCantidadSC > gIntMaximoResultadosSC)
            {
                throw new ArgumentOutOfRangeException(nameof(pIntCantidadSC),
                    "La cantidad debe estar entre 1 y " + gIntMaximoResultadosSC + ".");
            }
            return gObjClienteDASC.FncBuscarClientesSC(new List<string>(), pIntCantidadSC);
        }

        /// <summary>Quita espacios sobrantes y separadores para guardar datos uniformes.</summary>
        private static void SubNormalizarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            pObjClienteSC.gStrDpiSC = gRegSeparadoresDpiSC.Replace(pObjClienteSC.gStrDpiSC ?? string.Empty, string.Empty);
            pObjClienteSC.gStrNombresSC = FncNormalizarEspaciosSC(pObjClienteSC.gStrNombresSC);
            pObjClienteSC.gStrApellidosSC = FncNormalizarEspaciosSC(pObjClienteSC.gStrApellidosSC);
            pObjClienteSC.gStrTelefonoSC = gRegSeparadoresTelefonoSC.Replace(pObjClienteSC.gStrTelefonoSC ?? string.Empty, string.Empty);

            string lStrCorreoSC = (pObjClienteSC.gStrCorreoSC ?? string.Empty).Trim().ToLowerInvariant();
            pObjClienteSC.gStrCorreoSC = lStrCorreoSC.Length == 0 ? null : lStrCorreoSC;

            string lStrDireccionSC = FncNormalizarEspaciosSC(pObjClienteSC.gStrDireccionSC);
            pObjClienteSC.gStrDireccionSC = lStrDireccionSC.Length == 0 ? null : lStrDireccionSC;
        }

        /// <summary>Revisa cada campo y devuelve todos los errores encontrados.</summary>
        private static Dictionary<string, string> FncValidarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            Dictionary<string, string> lDicErroresSC = new Dictionary<string, string>();

            if (pObjClienteSC.gStrDpiSC.Length == 0)
            {
                lDicErroresSC[gStrCampoDpiSC] = "El DPI es obligatorio.";
            }
            else if (!gRegSoloDigitosSC.IsMatch(pObjClienteSC.gStrDpiSC))
            {
                lDicErroresSC[gStrCampoDpiSC] = "El DPI solo puede contener números.";
            }
            else if (pObjClienteSC.gStrDpiSC.Length != 13)
            {
                lDicErroresSC[gStrCampoDpiSC] = "El DPI debe tener exactamente 13 dígitos.";
            }

            SubValidarNombreSC(lDicErroresSC, gStrCampoNombresSC, pObjClienteSC.gStrNombresSC, "Los nombres");
            SubValidarNombreSC(lDicErroresSC, gStrCampoApellidosSC, pObjClienteSC.gStrApellidosSC, "Los apellidos");

            if (pObjClienteSC.gStrTelefonoSC.Length == 0)
            {
                lDicErroresSC[gStrCampoTelefonoSC] = "El teléfono es obligatorio.";
            }
            else if (!gRegSoloDigitosSC.IsMatch(pObjClienteSC.gStrTelefonoSC))
            {
                lDicErroresSC[gStrCampoTelefonoSC] = "El teléfono solo puede contener números, espacios y guiones.";
            }
            else if (pObjClienteSC.gStrTelefonoSC.Length < 8 || pObjClienteSC.gStrTelefonoSC.Length > 15)
            {
                lDicErroresSC[gStrCampoTelefonoSC] = "El teléfono debe tener entre 8 y 15 dígitos.";
            }

            if (pObjClienteSC.gStrCorreoSC != null)
            {
                if (pObjClienteSC.gStrCorreoSC.Length > 254)
                {
                    lDicErroresSC[gStrCampoCorreoSC] = "El correo admite como máximo 254 caracteres.";
                }
                else if (!gRegCorreoSC.IsMatch(pObjClienteSC.gStrCorreoSC))
                {
                    lDicErroresSC[gStrCampoCorreoSC] = "Escribe un correo válido, por ejemplo nombre@dominio.com.";
                }
            }

            if (pObjClienteSC.gStrDireccionSC != null)
            {
                if (pObjClienteSC.gStrDireccionSC.Length > 200)
                {
                    lDicErroresSC[gStrCampoDireccionSC] = "La dirección admite como máximo 200 caracteres.";
                }
                else if (pObjClienteSC.gStrDireccionSC.IndexOfAny(new[] { '<', '>' }) >= 0)
                {
                    lDicErroresSC[gStrCampoDireccionSC] = "La dirección no puede contener los caracteres < ni >.";
                }
            }

            return lDicErroresSC;
        }

        /// <summary>Valida un campo de nombres o apellidos.</summary>
        private static void SubValidarNombreSC(
            Dictionary<string, string> pDicErroresSC, string pStrCampoSC, string pStrValorSC, string pStrEtiquetaSC)
        {
            if (pStrValorSC.Length == 0)
            {
                pDicErroresSC[pStrCampoSC] = pStrEtiquetaSC + " son obligatorios.";
            }
            else if (pStrValorSC.Length > 80)
            {
                pDicErroresSC[pStrCampoSC] = pStrEtiquetaSC + " admiten como máximo 80 caracteres.";
            }
            else if (!gRegNombreSC.IsMatch(pStrValorSC))
            {
                pDicErroresSC[pStrCampoSC] = pStrEtiquetaSC + " solo pueden contener letras, espacios, puntos, apóstrofos y guiones.";
            }
        }

        /// <summary>Quita espacios al inicio y al final y deja un solo espacio entre palabras.</summary>
        private static string FncNormalizarEspaciosSC(string pStrTextoSC)
        {
            return gRegEspaciosSC.Replace(pStrTextoSC ?? string.Empty, " ").Trim();
        }
    }
}
