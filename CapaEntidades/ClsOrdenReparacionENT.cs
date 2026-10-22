//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Capa de Entidades
// Parte de Neider: todo el código de este archivo es de su módulo.
// Ajuste de integración (Sharon): nombres según la convención del docente (prefijo g en campos y propiedades, Sub en procedimientos).
//==============================================================
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa una orden de reparación de AutoManager DMS.
    /// </summary>
    public class ClsOrdenReparacionENT
    {
        /// <summary>Obtiene o establece el número de la orden, generado por SQL Server.</summary>
        public int gIntIdOrdenNT { get; set; }

        /// <summary>Obtiene o establece el identificador del vehículo (debe corresponder al mismo vehículo que el VIN).</summary>
        public int gIntIdVehiculoNT { get; set; }

        /// <summary>Obtiene o establece el VIN del vehículo; relaciona la orden con dbo.Vehiculos.</summary>
        public string gStrVinNT { get; set; }

        /// <summary>Obtiene o establece la placa que tenía el vehículo al abrir la orden (dato histórico).</summary>
        public string gStrPlacaNT { get; set; }

        /// <summary>Obtiene o establece la fecha de apertura en UTC; se convierte a hora de Guatemala al mostrarla.</summary>
        public DateTime gDatFechaAperturaNT { get; set; }

        /// <summary>Obtiene o establece el motivo por el que ingresó el vehículo (obligatorio).</summary>
        public string gStrMotivoIngresoNT { get; set; }

        /// <summary>Obtiene o establece el diagnóstico del mecánico; es opcional.</summary>
        public string gStrDiagnosticoNT { get; set; }

        /// <summary>Obtiene o establece el estado de la orden: Abierta o Cerrada.</summary>
        public string gStrEstadoNT { get; set; }

        /// <summary>Obtiene o establece la fecha de cierre en UTC; es nula mientras la orden está abierta.</summary>
        public DateTime? gDatFechaCierreNT { get; set; }

        /// <summary>Obtiene o establece las observaciones registradas al cerrar la orden.</summary>
        public string gStrObservacionesCierreNT { get; set; }

        /// <summary>Obtiene o establece los datos del vehículo, cuando una consulta los incluye.</summary>
        public ClsVehiculoENSC gObjVehiculoNT { get; set; }

        /// <summary>Inicializa los textos vacíos y el estado inicial Abierta.</summary>
        public ClsOrdenReparacionENT()
        {
            gStrVinNT = string.Empty;
            gStrPlacaNT = string.Empty;
            gStrMotivoIngresoNT = string.Empty;
            gStrDiagnosticoNT = string.Empty;
            gStrEstadoNT = "Abierta";
            gStrObservacionesCierreNT = string.Empty;
        }
    }
}
