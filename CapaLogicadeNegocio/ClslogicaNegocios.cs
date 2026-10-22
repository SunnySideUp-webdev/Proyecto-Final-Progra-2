using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidades;

namespace CapaLogicadeNegocio
{
    public class ClslogicaNegocios
    {

        public string fncRegistroRepuestoPP(ClsCapaEntidades pojbnuevorepuestoPP)
        {
            Clsconexion lobjconexionPP = new Clsconexion();
            if (string.IsNullOrEmpty(pojbnuevorepuestoPP.gstrcodrepuestoPP)) return "Error: El código es obligatorio.";
            if (string.IsNullOrEmpty(pojbnuevorepuestoPP.gstrnomrepuestoPP)) return "Error: El nombre es obligatorio.";
            if (pojbnuevorepuestoPP.gintstockrepuestoPP < 0) return "Error: El stock inicial no puede ser negativo.";

            try
            {
                // Llama a tu método corregido en la capa de datos
                int filasAfectadas = lobjconexionPP.fncagregarepuestoPP(pojbnuevorepuestoPP);

                if (filasAfectadas > 0)
                    return "Éxito: El repuesto se registró correctamente en la base de datos.";
                else
                    return "Error: No se pudo completar el registro en el sistema.";
            }
            catch (Exception ex)
            {
                return "Error en la operación de la base de datos: " + ex.Message;
            }
        }

        public DataTable fncconsultarbdPP(string pstrcriterioPP)
        {
            Clsconexion lobjconexionPP = new Clsconexion();
            return lobjconexionPP.fncconsultarPP(pstrcriterioPP);
        }

        public string fncactualizastockPP(string lstrcodigoPP, int lintnuevostock)
        {
            Clsconexion lobjconexionPP = new Clsconexion();
            if (string.IsNullOrEmpty(lstrcodigoPP)) return "Error: Código de pieza no especificado.";
            if (lintnuevostock < 0)
            {
                return "Error: El stock físico en bodega no puede ser negativo.";
            }
            try
            {
                // Llama a tu método de la capa de datos
                int filasAfectadas = lobjconexionPP.fncactualizarstockPP(lstrcodigoPP, lintnuevostock);

                if (filasAfectadas > 0)
                    return "Éxito: El stock del repuesto se actualizó correctamente en la base de datos.";
                else
                    return "Error: No se encontró ningún repuesto con el código ingresado.";
            }
            catch (Exception ex)
            {
                return "Error en la operación de SQL: " + ex.Message;
            }
        }
    }
}
