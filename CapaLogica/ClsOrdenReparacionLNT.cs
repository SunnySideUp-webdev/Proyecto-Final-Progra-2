//==============================================================
// Nombre: Neider Ernesto Tomas Raxcaco
// Carné: 0900-16-12945
// Tema: AutoManager DMS - Módulo 2 Órdenes de reparación - Lógica de Negocio
// Parte de Neider: todo el código de este archivo es de su módulo.
//==============================================================
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

        /// <summary>Crea la lógica de órdenes con su acceso a datos.</summary>
        public ClsOrdenReparacionLNT()
        {
            gObjOrdenDatosNT = new ClsOrdenReparacionDANT();
        }

        /// <summary>Consulta las órdenes que siguen abiertas.</summary>
        /// <returns>Órdenes abiertas.</returns>
        public List<ClsOrdenReparacionENT>
            FncConsultarOrdenesAbiertasNT()
        {
            return gObjOrdenDatosNT.FncConsultarOrdenesAbiertasNT();
        }


        /// <summary>Indica si un VIN tiene 17 caracteres válidos (letras y números, sin I, O ni Q).</summary>
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

        /// <summary>Indica si existe un vehículo registrado con el VIN indicado.</summary>
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

        /// <summary>Indica si el identificador y el VIN corresponden al mismo vehículo registrado.</summary>
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


        /// <summary>Valida y registra una orden nueva: el vehículo debe existir y coincidir con el VIN, y el motivo es obligatorio.</summary>
        /// <param name="pObjOrdenNT">Datos de la orden escritos en el formulario.</param>
        /// <returns>Número asignado a la orden.</returns>
        /// <exception cref="ArgumentException">Algún dato no es válido; el mensaje se muestra al usuario.</exception>
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

        /// <summary>Valida y cierra una orden; las observaciones de cierre son obligatorias.</summary>
        /// <param name="pIntIdOrdenNT">Número de la orden.</param>
        /// <param name="pStrObservacionesNT">Observaciones de cierre.</param>
        /// <returns><c>true</c> si la orden se cerró; <c>false</c> si no existe o ya estaba cerrada.</returns>
        /// <exception cref="ArgumentException">El número de orden o las observaciones no son válidos.</exception>
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

        /// <summary>Obtiene una orden por su número.</summary>
        /// <param name="pIntIdOrdenNT">Número de la orden.</param>
        /// <returns>La orden o <c>null</c> si no existe.</returns>
        /// <exception cref="ArgumentException">El número de orden no es válido.</exception>
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



        /// <summary>Consulta el historial de reparaciones de un vehículo por su VIN (sin importar mayúsculas).</summary>
        /// <param name="pStrVinNT">VIN escrito por el usuario.</param>
        /// <returns>Órdenes del vehículo, de la más reciente a la más antigua.</returns>
        /// <exception cref="ArgumentException">El VIN está vacío o no tiene 17 caracteres válidos.</exception>
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
