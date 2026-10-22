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
    public class ClsValidacionExcepcionLN : Exception
    {
        private readonly Dictionary<string, string> lDicErrores;

        /// <summary>Obtiene los errores: la clave es el nombre del campo y el valor, el mensaje.</summary>
        public IReadOnlyDictionary<string, string> lDicErroresCampos
        {
            get { return lDicErrores; }
        }

        /// <summary>Crea la excepción con uno o más errores de campo.</summary>
        /// <param name="pDicErrores">Errores encontrados durante la validación.</param>
        public ClsValidacionExcepcionLN(IDictionary<string, string> pDicErrores)
            : base("Hay datos que no cumplen las reglas de negocio.")
        {
            lDicErrores = new Dictionary<string, string>(pDicErrores);
        }

        /// <summary>Crea la excepción con un único error de campo.</summary>
        /// <param name="pStrCampo">Nombre del campo con error.</param>
        /// <param name="pStrMensaje">Mensaje para el usuario.</param>
        public ClsValidacionExcepcionLN(string pStrCampo, string pStrMensaje)
            : this(new Dictionary<string, string> { { pStrCampo, pStrMensaje } })
        {
        }
    }
}
