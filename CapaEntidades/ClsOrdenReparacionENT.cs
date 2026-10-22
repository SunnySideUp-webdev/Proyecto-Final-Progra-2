
using System;

namespace CapaEntidades
{
    /// <summary>
    /// Representa una orden de reparación de AutoManager DMS.
    /// </summary>
    public class ClsOrdenReparacionENT
    {
        public int lIntIdOrdenNT { get; set; }

        public int lIntIdVehiculoNT { get; set; }

        public string lStrVinNT { get; set; }

        public string lStrPlacaNT { get; set; }

        public DateTime lDatFechaAperturaNT { get; set; }

        public string lStrMotivoIngresoNT { get; set; }

        public string lStrDiagnosticoNT { get; set; }

        public string lStrEstadoNT { get; set; }

        public DateTime? lDatFechaCierreNT { get; set; }

        public string lStrObservacionesCierreNT { get; set; }

        // Información del vehículo obtenida mediante consultas
        public ClsVehiculoENSC lObjVehiculoNT { get; set; }

        public ClsOrdenReparacionENT()
        {
            lStrVinNT = string.Empty;
            lStrPlacaNT = string.Empty;
            lStrMotivoIngresoNT = string.Empty;
            lStrDiagnosticoNT = string.Empty;
            lStrEstadoNT = "Abierta";
            lStrObservacionesCierreNT = string.Empty;
        }
    }
}
