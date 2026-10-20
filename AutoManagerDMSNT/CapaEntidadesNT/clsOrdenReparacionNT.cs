
using System;

namespace CapaEntidadesNT
{
    public class clsOrdenReparacionNT
    {
        public int IdOrden { get; set; }

        public string VIN { get; set; } = string.Empty;

        public string? Placa { get; set; }

        public DateTime FechaApertura { get; set; }

        public string MotivoIngreso { get; set; } = string.Empty;

        public string? Diagnostico { get; set; }

        public string Estado { get; set; } = "Abierta";

        public DateTime? FechaCierre { get; set; }

        public string? ObservacionesCierre { get; set; }
    }
}
