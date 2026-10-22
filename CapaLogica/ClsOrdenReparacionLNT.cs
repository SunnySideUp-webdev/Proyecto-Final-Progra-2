
using CapaDatos;
using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaLogica
{
    /// <summary>
    /// Lógica de negocio del módulo de órdenes de reparación.
    /// </summary>
    public class ClsOrdenReparacionLNT
    {
        private readonly ClsOrdenReparacionDANT gObjOrdenDatosNT;

        public ClsOrdenReparacionLNT()
        {
            gObjOrdenDatosNT = new ClsOrdenReparacionDANT();
        }

        // Consulta las órdenes abiertas.
        public List<ClsOrdenReparacionENT>
            FncConsultarOrdenesAbiertasNT()
        {
            return gObjOrdenDatosNT.FncConsultarOrdenesAbiertasNT();
        }


        private bool FncValidarVinNT(string pStrVinNT)
        {
            if (string.IsNullOrWhiteSpace(pStrVinNT))
            {
                return false;
            }

            string lStrVinNT = pStrVinNT.Trim().ToUpperInvariant();

            return lStrVinNT.Length == 17
                && lStrVinNT.All(pObjCaracterNT =>
                    (pObjCaracterNT >= 'A' && pObjCaracterNT <= 'Z'
                        && pObjCaracterNT != 'I'
                        && pObjCaracterNT != 'O'
                        && pObjCaracterNT != 'Q')
                    || (pObjCaracterNT >= '0' && pObjCaracterNT <= '9'));
        }

        private bool FncExisteVehiculoNT(string pStrVinNT)
        {
            if (!FncValidarVinNT(pStrVinNT))
            {
                return false;
            }

            ClsVehiculoDASC lObjVehiculoDatosNT =
                new ClsVehiculoDASC();

            string lStrVinNT = pStrVinNT.Trim().ToUpperInvariant();

            return lObjVehiculoDatosNT.fncExisteVinSC(lStrVinNT);
        }

        private bool FncValidarVehiculoNT(
            int pIntIdVehiculoNT,
            string pStrVinNT)
        {
            if (pIntIdVehiculoNT <= 0 ||
                !FncValidarVinNT(pStrVinNT))
            {
                return false;
            }

            ClsVehiculoDASC lObjVehiculoDatosNT =
                new ClsVehiculoDASC();

            ClsVehiculoENSC lObjVehiculoNT =
                lObjVehiculoDatosNT.fncObtenerVehiculoPorIdSC(
                    pIntIdVehiculoNT);

            if (lObjVehiculoNT == null)
            {
                return false;
            }

            return string.Equals(
                lObjVehiculoNT.lStrVinSC,
                pStrVinNT.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }


        public int FncRegistrarOrdenNT(
            ClsOrdenReparacionENT pObjOrdenNT)
        {
            if (pObjOrdenNT == null)
            {
                throw new ArgumentNullException(
                    nameof(pObjOrdenNT));
            }

            if (!FncValidarVehiculoNT(
                pObjOrdenNT.lIntIdVehiculoNT,
                pObjOrdenNT.lStrVinNT))
            {
                throw new ArgumentException(
                    "El VIN no corresponde a un vehículo registrado.");
            }

            if (string.IsNullOrWhiteSpace(
                pObjOrdenNT.lStrMotivoIngresoNT))
            {
                throw new ArgumentException(
                    "Debe ingresar el motivo de la reparación.");
            }

            pObjOrdenNT.lStrVinNT =
                pObjOrdenNT.lStrVinNT.Trim().ToUpperInvariant();

            pObjOrdenNT.lStrMotivoIngresoNT =
                pObjOrdenNT.lStrMotivoIngresoNT.Trim();

            if (pObjOrdenNT.lStrMotivoIngresoNT.Length > 500)
            {
                throw new ArgumentException(
                    "El motivo no puede superar 500 caracteres.");
            }

            if (!string.IsNullOrEmpty(pObjOrdenNT.lStrDiagnosticoNT)
                && pObjOrdenNT.lStrDiagnosticoNT.Length > 1000)
            {
                throw new ArgumentException(
                    "El diagnóstico no puede superar 1000 caracteres.");
            }

            // Obtener la placa del vehículo registrado,
            // evitando que se ingrese una placa incorrecta.
            ClsVehiculoDASC lObjVehiculoDatosNT =
                new ClsVehiculoDASC();

            ClsVehiculoENSC lObjVehiculoNT =
                lObjVehiculoDatosNT.fncObtenerVehiculoPorIdSC(
                    pObjOrdenNT.lIntIdVehiculoNT);

            if (lObjVehiculoNT == null)
            {
                throw new ArgumentException(
                    "No se encontró el vehículo.");
            }

            pObjOrdenNT.lStrPlacaNT = lObjVehiculoNT.lStrPlacaSC;
            pObjOrdenNT.lStrEstadoNT = "Abierta";

            return gObjOrdenDatosNT.FncRegistrarOrdenNT(pObjOrdenNT);
        }

        public bool FncCerrarOrdenNT(
            int pIntIdOrdenNT,
            string pStrObservacionesNT)
        {
            if (pIntIdOrdenNT <= 0)
            {
                throw new ArgumentException(
                    "El número de orden no es válido.");
            }

            if (string.IsNullOrWhiteSpace(pStrObservacionesNT))
            {
                throw new ArgumentException(
                    "Debe ingresar las observaciones de cierre.");
            }

            string lStrObservacionesNT =
                pStrObservacionesNT.Trim();

            if (lStrObservacionesNT.Length > 1000)
            {
                throw new ArgumentException(
                    "Las observaciones no pueden superar 1000 caracteres.");
            }

            return gObjOrdenDatosNT.FncCerrarOrdenNT(
                pIntIdOrdenNT,
                lStrObservacionesNT);
        }

        public ClsOrdenReparacionENT FncObtenerOrdenPorIdNT(
            int pIntIdOrdenNT)
        {
            if (pIntIdOrdenNT <= 0)
            {
                throw new ArgumentException(
                    "El número de orden no es válido.");
            }

            return gObjOrdenDatosNT.FncObtenerOrdenPorIdNT(
                pIntIdOrdenNT);
        }



        public List<ClsOrdenReparacionENT> FncConsultarHistorialPorVinNT(
            string pStrVinNT)
        {
            if (string.IsNullOrWhiteSpace(pStrVinNT))
            {
                throw new ArgumentException(
                    "Debe ingresar el VIN del vehículo.");
            }

            string lStrVinNT =
                pStrVinNT.Trim().ToUpperInvariant();

            if (!FncValidarVinNT(lStrVinNT))
            {
                throw new ArgumentException(
                    "El VIN debe contener 17 caracteres válidos.");
            }

            return gObjOrdenDatosNT.FncConsultarHistorialPorVinNT(
                lStrVinNT);
        }




    }
}
