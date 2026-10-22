//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Lógica de Negocio
//==============================================================
using System;
using System.Collections.Generic;

namespace CapaLogica
{
    /// <summary>
    /// Agrupa los errores de validación por campo para que la Capa de Presentación
    /// pueda mostrar cada mensaje junto al control correspondiente.
    /// </summary>
    public class ClsValidacionExcepcionLNSC : Exception
    {
        private readonly Dictionary<string, string> gDicErroresSC;

        /// <summary>Obtiene los errores: la clave es el nombre del campo y el valor, el mensaje.</summary>
        public IReadOnlyDictionary<string, string> gDicErroresCamposSC
        {
            get { return gDicErroresSC; }
        }

        /// <summary>Crea la excepción con uno o más errores de campo.</summary>
        /// <param name="pDicErrores">Errores encontrados durante la validación.</param>
        public ClsValidacionExcepcionLNSC(IDictionary<string, string> pDicErroresSC)
            : base("Hay datos que no cumplen las reglas de negocio.")
        {
            gDicErroresSC = new Dictionary<string, string>(pDicErroresSC);
        }

        /// <summary>Crea la excepción con un único error de campo.</summary>
        /// <param name="pStrCampo">Nombre del campo con error.</param>
        /// <param name="pStrMensaje">Mensaje para el usuario.</param>
        public ClsValidacionExcepcionLNSC(string pStrCampoSC, string pStrMensajeSC)
            : this(new Dictionary<string, string> { { pStrCampoSC, pStrMensajeSC } })
        {
        }
    }
}
