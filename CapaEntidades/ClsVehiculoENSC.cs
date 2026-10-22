//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Módulo 1 Clientes y vehículos - Capa de Entidades
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa un vehículo asociado a un cliente. Corresponde a la tabla
    /// dbo.Vehiculos; su identificador es el que usan las órdenes de reparación.
    /// </summary>
    public class ClsVehiculoENSC
    {
        /// <summary>Obtiene o establece el identificador generado por SQL Server.</summary>
        public int lIntIdVehiculoSC { get; set; }

        /// <summary>Obtiene o establece el identificador del cliente propietario.</summary>
        public int lIntIdClienteSC { get; set; }

        /// <summary>Obtiene o establece la placa normalizada (mayúsculas y números, sin guiones).</summary>
        public string lStrPlacaSC { get; set; }

        /// <summary>
        /// Obtiene o establece el VIN de 17 caracteres. Es obligatorio y único: las órdenes
        /// de reparación se relacionan con el vehículo por medio de él.
        /// </summary>
        public string lStrVinSC { get; set; }

        /// <summary>Obtiene o establece la marca del vehículo.</summary>
        public string lStrMarcaSC { get; set; }

        /// <summary>Obtiene o establece el modelo del vehículo.</summary>
        public string lStrModeloSC { get; set; }

        /// <summary>Obtiene o establece el año del modelo.</summary>
        public int lIntAnioSC { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro en UTC, tal como la guarda SQL Server.
        /// Se convierte a la hora de Guatemala solo al mostrarse.
        /// </summary>
        public DateTime lDatFechaRegistroUtcSC { get; set; }

        /// <summary>
        /// Obtiene o establece los datos del cliente propietario. Solo se llena en las
        /// consultas que unen Vehiculos con Clientes; en el registro es nulo.
        /// </summary>
        public ClsClienteENSC lObjPropietarioSC { get; set; }

        /// <summary>Inicializa los campos obligatorios con valores no nulos.</summary>
        public ClsVehiculoENSC()
        {
            lStrPlacaSC = string.Empty;
            lStrVinSC = string.Empty;
            lStrMarcaSC = string.Empty;
            lStrModeloSC = string.Empty;
        }
    }
}
