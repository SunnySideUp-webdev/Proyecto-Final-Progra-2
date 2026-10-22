//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Capa de Presentación
//==============================================================
using System;
using System.Diagnostics;

namespace CapaPresentacion.Utilidades
{
    /// <summary>
    /// Registra los errores inesperados para el equipo de desarrollo. Al usuario
    /// solo se le muestra un mensaje general, nunca el detalle técnico.
    /// </summary>
    public static class ClsRegistroErrores
    {
        /// <summary>Escribe el error en la consola de XSP4 y en el seguimiento de Visual Studio.</summary>
        /// <param name="pObjExcepcion">Error capturado.</param>
        /// <param name="pStrContexto">Operación que se estaba realizando.</param>
        public static void fncRegistrar(Exception pObjExcepcion, string pStrContexto)
        {
            string lStrMensaje = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] ERROR en {1}: {2}",
                DateTime.Now, pStrContexto, pObjExcepcion);

            Trace.TraceError(lStrMensaje);
            Console.Error.WriteLine(lStrMensaje);
        }
    }
}
