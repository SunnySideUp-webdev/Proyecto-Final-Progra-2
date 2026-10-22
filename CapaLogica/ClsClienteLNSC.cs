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
        public const int lIntMaximoResultadosSC = 50;

        /// <summary>Longitud máxima del texto de búsqueda.</summary>
        public const int lIntLongitudMaximaBusquedaSC = 80;

        /// <summary>Cantidad máxima de palabras en una búsqueda.</summary>
        public const int lIntMaximoPalabrasSC = 5;

        // Nombres de campo usados como clave en ClsValidacionExcepcionLN.
        public const string lStrCampoDpiSC = nameof(ClsClienteENSC.lStrDpiSC);
        public const string lStrCampoNombresSC = nameof(ClsClienteENSC.lStrNombresSC);
        public const string lStrCampoApellidosSC = nameof(ClsClienteENSC.lStrApellidosSC);
        public const string lStrCampoTelefonoSC = nameof(ClsClienteENSC.lStrTelefonoSC);
        public const string lStrCampoCorreoSC = nameof(ClsClienteENSC.lStrCorreoSC);
        public const string lStrCampoDireccionSC = nameof(ClsClienteENSC.lStrDireccionSC);
        public const string lStrCampoBusquedaSC = "Busqueda";

        private const string lStrMensajeDpiDuplicadoSC = "Ya existe un cliente registrado con este DPI.";

        // Letras (con tildes y ñ), espacios, apóstrofo, punto y guion; debe empezar con letra.
        private static readonly Regex lRegNombreSC = new Regex(@"^\p{L}[\p{L}\p{M} .'-]*$");
        private static readonly Regex lRegCorreoSC = new Regex(@"^[^@\s<>]+@[^@\s<>]+\.[^@\s<>]+$");
        private static readonly Regex lRegSeparadoresDpiSC = new Regex(@"[\s-]");
        private static readonly Regex lRegSeparadoresTelefonoSC = new Regex(@"[\s\-().+]");
        // [0-9] y no \d: \d también acepta dígitos de otros alfabetos (por ejemplo ١٢٣),
        // que SQL Server rechazaría con CK_Clientes_DPI o CK_Clientes_Telefono.
        private static readonly Regex lRegSoloDigitosSC = new Regex(@"^[0-9]+$");
        private static readonly Regex lRegTerminoNumericoSC = new Regex(@"^[\d\s\-().+]+$");
        private static readonly Regex lRegEspaciosSC = new Regex(@"\s+");

        private readonly ClsClienteDASC lObjClienteDASC = new ClsClienteDASC();

        /// <summary>Normaliza, valida y registra un cliente nuevo.</summary>
        /// <param name="pObjClienteSC">Datos escritos en el formulario.</param>
        /// <returns>Identificador asignado al nuevo cliente.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">Algún dato no cumple las reglas o el DPI ya existe.</exception>
        public int fncRegistrarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            if (pObjClienteSC == null)
            {
                throw new ArgumentNullException(nameof(pObjClienteSC));
            }

            fncNormalizarClienteSC(pObjClienteSC);
            Dictionary<string, string> lDicErroresSC = fncValidarClienteSC(pObjClienteSC);

            // Solo se consulta el duplicado si el DPI tiene un formato válido.
            if (!lDicErroresSC.ContainsKey(lStrCampoDpiSC) && lObjClienteDASC.fncExisteDpiSC(pObjClienteSC.lStrDpiSC))
            {
                lDicErroresSC[lStrCampoDpiSC] = lStrMensajeDpiDuplicadoSC;
            }

            if (lDicErroresSC.Count > 0)
            {
                throw new ClsValidacionExcepcionLN(lDicErroresSC);
            }

            try
            {
                return lObjClienteDASC.fncInsertarClienteSC(pObjClienteSC);
            }
            catch (ClsDatoDuplicadoExcepcionDA)
            {
                // Otro usuario registró el mismo DPI entre la consulta previa y el INSERT;
                // la restricción UQ_Clientes_DPI de SQL Server lo detuvo.
                throw new ClsValidacionExcepcionLN(lStrCampoDpiSC, lStrMensajeDpiDuplicadoSC);
            }
        }

        /// <summary>
        /// Busca clientes por palabras: cada palabra debe aparecer en el DPI, el teléfono,
        /// los nombres o los apellidos (por ejemplo, "Pérez Ana" o "Ana 5555"). Si el texto
        /// solo tiene números y separadores, se busca como un único número sin separadores.
        /// </summary>
        /// <param name="pStrTerminoSC">Texto escrito por el usuario; vacío lista los más recientes.</param>
        /// <returns>Hasta <see cref="lIntMaximoResultadosSC"/> clientes.</returns>
        /// <exception cref="ClsValidacionExcepcionLN">El texto es demasiado largo o tiene demasiadas palabras.</exception>
        public List<ClsClienteENSC> fncBuscarClientesSC(string pStrTerminoSC)
        {
            string lStrTerminoSC = fncNormalizarEspaciosSC(pStrTerminoSC);

            if (lStrTerminoSC.Length > lIntLongitudMaximaBusquedaSC)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoBusquedaSC,
                    "La búsqueda admite como máximo " + lIntLongitudMaximaBusquedaSC + " caracteres.");
            }

            List<string> lLstPalabrasSC;
            if (lRegTerminoNumericoSC.IsMatch(lStrTerminoSC))
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

            if (lLstPalabrasSC.Count > lIntMaximoPalabrasSC)
            {
                throw new ClsValidacionExcepcionLN(lStrCampoBusquedaSC,
                    "Escribe como máximo " + lIntMaximoPalabrasSC + " palabras en la búsqueda.");
            }

            return lObjClienteDASC.fncBuscarClientesSC(lLstPalabrasSC, lIntMaximoResultadosSC);
        }

        /// <summary>Obtiene los datos de un cliente.</summary>
        /// <param name="pIntIdClienteSC">Identificador del cliente.</param>
        /// <returns>El cliente o <c>null</c> si no existe.</returns>
        public ClsClienteENSC fncObtenerClienteSC(int pIntIdClienteSC)
        {
            return pIntIdClienteSC <= 0 ? null : lObjClienteDASC.fncObtenerClientePorIdSC(pIntIdClienteSC);
        }

        /// <summary>Cuenta los clientes registrados.</summary>
        /// <returns>Cantidad total de clientes.</returns>
        public int fncContarClientesSC()
        {
            return lObjClienteDASC.fncContarClientesSC();
        }

        /// <summary>
        /// Cuenta los clientes registrados desde el primer día del mes actual en Guatemala.
        /// El inicio del mes se convierte a UTC porque así se guardan las fechas.
        /// </summary>
        /// <returns>Cantidad de clientes nuevos en el mes.</returns>
        public int fncContarClientesDelMesSC()
        {
            DateTime lDatAhoraSC = ClsZonaHorariaLN.fncObtenerAhoraGuatemala();
            DateTime lDatInicioMesSC = new DateTime(lDatAhoraSC.Year, lDatAhoraSC.Month, 1);
            return lObjClienteDASC.fncContarClientesDesdeSC(ClsZonaHorariaLN.fncConvertirGuatemalaAUtc(lDatInicioMesSC));
        }

        /// <summary>Obtiene los últimos clientes registrados.</summary>
        /// <param name="pIntCantidadSC">Cantidad de clientes a devolver (1 a 50).</param>
        /// <returns>Clientes ordenados del más reciente al más antiguo.</returns>
        public List<ClsClienteENSC> fncObtenerClientesRecientesSC(int pIntCantidadSC)
        {
            if (pIntCantidadSC < 1 || pIntCantidadSC > lIntMaximoResultadosSC)
            {
                throw new ArgumentOutOfRangeException(nameof(pIntCantidadSC),
                    "La cantidad debe estar entre 1 y " + lIntMaximoResultadosSC + ".");
            }
            return lObjClienteDASC.fncBuscarClientesSC(new List<string>(), pIntCantidadSC);
        }

        /// <summary>Quita espacios sobrantes y separadores para guardar datos uniformes.</summary>
        private static void fncNormalizarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            pObjClienteSC.lStrDpiSC = lRegSeparadoresDpiSC.Replace(pObjClienteSC.lStrDpiSC ?? string.Empty, string.Empty);
            pObjClienteSC.lStrNombresSC = fncNormalizarEspaciosSC(pObjClienteSC.lStrNombresSC);
            pObjClienteSC.lStrApellidosSC = fncNormalizarEspaciosSC(pObjClienteSC.lStrApellidosSC);
            pObjClienteSC.lStrTelefonoSC = lRegSeparadoresTelefonoSC.Replace(pObjClienteSC.lStrTelefonoSC ?? string.Empty, string.Empty);

            string lStrCorreoSC = (pObjClienteSC.lStrCorreoSC ?? string.Empty).Trim().ToLowerInvariant();
            pObjClienteSC.lStrCorreoSC = lStrCorreoSC.Length == 0 ? null : lStrCorreoSC;

            string lStrDireccionSC = fncNormalizarEspaciosSC(pObjClienteSC.lStrDireccionSC);
            pObjClienteSC.lStrDireccionSC = lStrDireccionSC.Length == 0 ? null : lStrDireccionSC;
        }

        /// <summary>Revisa cada campo y devuelve todos los errores encontrados.</summary>
        private static Dictionary<string, string> fncValidarClienteSC(ClsClienteENSC pObjClienteSC)
        {
            Dictionary<string, string> lDicErroresSC = new Dictionary<string, string>();

            if (pObjClienteSC.lStrDpiSC.Length == 0)
            {
                lDicErroresSC[lStrCampoDpiSC] = "El DPI es obligatorio.";
            }
            else if (!lRegSoloDigitosSC.IsMatch(pObjClienteSC.lStrDpiSC))
            {
                lDicErroresSC[lStrCampoDpiSC] = "El DPI solo puede contener números.";
            }
            else if (pObjClienteSC.lStrDpiSC.Length != 13)
            {
                lDicErroresSC[lStrCampoDpiSC] = "El DPI debe tener exactamente 13 dígitos.";
            }

            fncValidarNombreSC(lDicErroresSC, lStrCampoNombresSC, pObjClienteSC.lStrNombresSC, "Los nombres");
            fncValidarNombreSC(lDicErroresSC, lStrCampoApellidosSC, pObjClienteSC.lStrApellidosSC, "Los apellidos");

            if (pObjClienteSC.lStrTelefonoSC.Length == 0)
            {
                lDicErroresSC[lStrCampoTelefonoSC] = "El teléfono es obligatorio.";
            }
            else if (!lRegSoloDigitosSC.IsMatch(pObjClienteSC.lStrTelefonoSC))
            {
                lDicErroresSC[lStrCampoTelefonoSC] = "El teléfono solo puede contener números, espacios y guiones.";
            }
            else if (pObjClienteSC.lStrTelefonoSC.Length < 8 || pObjClienteSC.lStrTelefonoSC.Length > 15)
            {
                lDicErroresSC[lStrCampoTelefonoSC] = "El teléfono debe tener entre 8 y 15 dígitos.";
            }

            if (pObjClienteSC.lStrCorreoSC != null)
            {
                if (pObjClienteSC.lStrCorreoSC.Length > 254)
                {
                    lDicErroresSC[lStrCampoCorreoSC] = "El correo admite como máximo 254 caracteres.";
                }
                else if (!lRegCorreoSC.IsMatch(pObjClienteSC.lStrCorreoSC))
                {
                    lDicErroresSC[lStrCampoCorreoSC] = "Escribe un correo válido, por ejemplo nombre@dominio.com.";
                }
            }

            if (pObjClienteSC.lStrDireccionSC != null)
            {
                if (pObjClienteSC.lStrDireccionSC.Length > 200)
                {
                    lDicErroresSC[lStrCampoDireccionSC] = "La dirección admite como máximo 200 caracteres.";
                }
                else if (pObjClienteSC.lStrDireccionSC.IndexOfAny(new[] { '<', '>' }) >= 0)
                {
                    lDicErroresSC[lStrCampoDireccionSC] = "La dirección no puede contener los caracteres < ni >.";
                }
            }

            return lDicErroresSC;
        }

        /// <summary>Valida un campo de nombres o apellidos.</summary>
        private static void fncValidarNombreSC(
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
            else if (!lRegNombreSC.IsMatch(pStrValorSC))
            {
                pDicErroresSC[pStrCampoSC] = pStrEtiquetaSC + " solo pueden contener letras, espacios, puntos, apóstrofos y guiones.";
            }
        }

        /// <summary>Quita espacios al inicio y al final y deja un solo espacio entre palabras.</summary>
        private static string fncNormalizarEspaciosSC(string pStrTextoSC)
        {
            return lRegEspaciosSC.Replace(pStrTextoSC ?? string.Empty, " ").Trim();
        }
    }
}
