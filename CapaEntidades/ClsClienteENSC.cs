//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa un cliente del taller. Corresponde a la tabla dbo.Clientes
    /// y se transfiere entre las capas de Presentación, Lógica y Datos.
    /// </summary>
    public class ClsClienteENSC
    {
        /// <summary>Obtiene o establece el identificador generado por SQL Server.</summary>
        public int gIntIdClienteSC { get; set; }

        /// <summary>Obtiene o establece el DPI (CUI) de 13 dígitos; no se repite entre clientes.</summary>
        public string gStrDpiSC { get; set; }

        /// <summary>Obtiene o establece los nombres del cliente.</summary>
        public string gStrNombresSC { get; set; }

        /// <summary>Obtiene o establece los apellidos del cliente.</summary>
        public string gStrApellidosSC { get; set; }

        /// <summary>Obtiene o establece el teléfono, guardado solo con dígitos (8 a 15).</summary>
        public string gStrTelefonoSC { get; set; }

        /// <summary>Obtiene o establece el correo electrónico; es opcional y puede ser nulo.</summary>
        public string gStrCorreoSC { get; set; }

        /// <summary>Obtiene o establece la dirección; es opcional y puede ser nula.</summary>
        public string gStrDireccionSC { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro en UTC, tal como la guarda SQL Server.
        /// Se convierte a la hora de Guatemala solo al mostrarse.
        /// </summary>
        public DateTime gDatFechaRegistroUtcSC { get; set; }

        /// <summary>Inicializa los campos obligatorios con valores no nulos.</summary>
        public ClsClienteENSC()
        {
            gStrDpiSC = string.Empty;
            gStrNombresSC = string.Empty;
            gStrApellidosSC = string.Empty;
            gStrTelefonoSC = string.Empty;
        }
    }
}
