
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapaEntidadesNT;
using Microsoft.EntityFrameworkCore;
using System;

namespace CapaAccesoDatosNT
{
    public class clsOrdenReparacionDANT
    {
        private readonly clsAutoManagerContextNT gObjContextoNT;

        public clsOrdenReparacionDANT(
            clsAutoManagerContextNT pObjContextoNT)
        {
            gObjContextoNT = pObjContextoNT;
        }

        // Obtener las órdenes de reparación abiertas
        public async Task<List<clsOrdenReparacionNT>>
            FncConsultarOrdenesAbiertasNT()

        {

            List<clsOrdenReparacionNT> lObjOrdenesNT =
                await gObjContextoNT.OrdenesReparacion
                    .AsNoTracking()
                    .Where(pObjOrdenNT =>
                        pObjOrdenNT.Estado == "Abierta")
                    .OrderByDescending(pObjOrdenNT =>
                        pObjOrdenNT.FechaApertura)
                    .ToListAsync();

            return lObjOrdenesNT;
        }

        // Registrar una nueva orden de reparación
        public async Task<int> FncRegistrarOrdenNT(
            clsOrdenReparacionNT pObjOrdenNT)
        {
            pObjOrdenNT.IdOrden = 0;
            pObjOrdenNT.VIN = pObjOrdenNT.VIN.Trim().ToUpperInvariant();
            pObjOrdenNT.Placa = pObjOrdenNT.Placa?.Trim().ToUpperInvariant();
            pObjOrdenNT.FechaApertura = DateTime.Now;
            pObjOrdenNT.Estado = "Abierta";
            pObjOrdenNT.FechaCierre = null;
            pObjOrdenNT.ObservacionesCierre = null;

            gObjContextoNT.OrdenesReparacion.Add(pObjOrdenNT);

            await gObjContextoNT.SaveChangesAsync();

            return pObjOrdenNT.IdOrden;
        }

    }
}
