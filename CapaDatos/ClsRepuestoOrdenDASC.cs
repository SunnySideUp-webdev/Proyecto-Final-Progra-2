//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Repuestos utilizados en órdenes - Capa de Datos
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidades;

namespace CapaDatos
{
    /// <summary>Resultado de registrar o quitar un repuesto de una orden.</summary>
    public enum ResultadoRepuestoOrdenSC
    {
        Correcto,
        OrdenNoExiste,
        OrdenCerrada,
        RepuestoNoExiste,
        StockInsuficiente,
        UsoNoExiste
    }

    /// <summary>
    /// Ejecuta las operaciones de la tabla dbo.RepuestosOrden. Registrar o quitar un repuesto
    /// también cambia el stock en dbo.Repuesto: ambas cosas ocurren en una sola transacción,
    /// así el inventario nunca queda distinto de lo registrado en las órdenes.
    /// </summary>
    public class ClsRepuestoOrdenDASC
    {
        /// <summary>Lista los repuestos utilizados en una orden, en el orden en que se registraron.</summary>
        /// <param name="pIntIdOrdenSC">Identificador de la orden.</param>
        /// <returns>Repuestos de la orden (vacío si no tiene).</returns>
        public List<ClsRepuestoOrdenENSC> FncListarPorOrdenSC(int pIntIdOrdenSC)
        {
            const string lStrConsultaSC = @"
                SELECT ro.IdRepuestoOrden, ro.IdOrden, ro.IdRepuesto, r.codigo, r.nombre, ro.Cantidad, ro.FechaRegistro
                FROM dbo.RepuestosOrden AS ro
                INNER JOIN dbo.Repuesto AS r ON r.idrepuesto = ro.IdRepuesto
                WHERE ro.IdOrden = @IdOrden
                ORDER BY ro.FechaRegistro, ro.IdRepuestoOrden;";

            List<ClsRepuestoOrdenENSC> lLstUsosSC = new List<ClsRepuestoOrdenENSC>();

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjComandoSC.Parameters.Add("@IdOrden", SqlDbType.Int).Value = pIntIdOrdenSC;
                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    while (lObjLectorSC.Read())
                    {
                        lLstUsosSC.Add(new ClsRepuestoOrdenENSC
                        {
                            gIntIdRepuestoOrdenSC = lObjLectorSC.GetInt32(0),
                            gIntIdOrdenSC = lObjLectorSC.GetInt32(1),
                            gIntIdRepuestoSC = lObjLectorSC.GetInt32(2),
                            gStrCodigoSC = lObjLectorSC.GetString(3),
                            gStrNombreRepuestoSC = lObjLectorSC.GetString(4),
                            gIntCantidadSC = lObjLectorSC.GetInt32(5),
                            // SQL Server devuelve DATETIME2 sin zona; la columna se guarda en UTC.
                            gDatFechaRegistroUtcSC = DateTime.SpecifyKind(lObjLectorSC.GetDateTime(6), DateTimeKind.Utc)
                        });
                    }
                }
            }

            return lLstUsosSC;
        }

        /// <summary>Lista los repuestos que tienen unidades en bodega, ordenados por nombre.</summary>
        /// <returns>Repuestos con stock mayor que cero.</returns>
        public List<ClsRepuestoENPP> FncListarRepuestosDisponiblesSC()
        {
            const string lStrConsultaSC = @"
                SELECT idrepuesto, codigo, nombre, stock, marcacarro, modelocarro, aniocarro
                FROM dbo.Repuesto
                WHERE stock > 0
                ORDER BY nombre, codigo;";

            List<ClsRepuestoENPP> lLstRepuestosSC = new List<ClsRepuestoENPP>();

            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, lObjConexionSC))
            {
                lObjConexionSC.Open();
                using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                {
                    while (lObjLectorSC.Read())
                    {
                        lLstRepuestosSC.Add(new ClsRepuestoENPP
                        {
                            gIntIdRepuestoPP = lObjLectorSC.GetInt32(0),
                            gStrCodigoPP = lObjLectorSC.GetString(1),
                            gStrNombrePP = lObjLectorSC.GetString(2),
                            gIntStockPP = lObjLectorSC.GetInt32(3),
                            gStrMarcaCarroPP = lObjLectorSC.GetString(4),
                            gStrModeloCarroPP = lObjLectorSC.GetString(5),
                            gIntAnioCarroPP = lObjLectorSC.GetInt32(6)
                        });
                    }
                }
            }

            return lLstRepuestosSC;
        }

        /// <summary>
        /// Registra que se usaron unidades de un repuesto en una orden abierta y las descuenta
        /// del stock. Si la orden no está abierta o el stock no alcanza, no cambia nada.
        /// </summary>
        /// <param name="pIntIdOrdenSC">Orden de reparación.</param>
        /// <param name="pIntIdRepuestoSC">Repuesto utilizado.</param>
        /// <param name="pIntCantidadSC">Unidades utilizadas (mayor que cero).</param>
        /// <returns>Correcto, o el motivo por el que no se registró.</returns>
        public ResultadoRepuestoOrdenSC FncRegistrarUsoSC(int pIntIdOrdenSC, int pIntIdRepuestoSC, int pIntCantidadSC)
        {
            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            {
                lObjConexionSC.Open();
                using (SqlTransaction lObjTransaccionSC = lObjConexionSC.BeginTransaction())
                {
                    ResultadoRepuestoOrdenSC lEnmEstadoOrdenSC = FncBloquearOrdenAbiertaSC(lObjConexionSC, lObjTransaccionSC, pIntIdOrdenSC);
                    if (lEnmEstadoOrdenSC != ResultadoRepuestoOrdenSC.Correcto)
                    {
                        lObjTransaccionSC.Rollback();
                        return lEnmEstadoOrdenSC;
                    }

                    // Descuenta solo si alcanza: la condición stock >= @Cantidad y la resta ocurren
                    // en la misma instrucción, así dos usuarios no pueden gastar la misma unidad.
                    const string lStrDescontarSC = @"
                        UPDATE dbo.Repuesto SET stock = stock - @Cantidad
                        WHERE idrepuesto = @IdRepuesto AND stock >= @Cantidad;";
                    using (SqlCommand lObjComandoSC = new SqlCommand(lStrDescontarSC, lObjConexionSC, lObjTransaccionSC))
                    {
                        lObjComandoSC.Parameters.Add("@Cantidad", SqlDbType.Int).Value = pIntCantidadSC;
                        lObjComandoSC.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = pIntIdRepuestoSC;
                        if (lObjComandoSC.ExecuteNonQuery() == 0)
                        {
                            bool lBlnExisteSC = FncExisteRepuestoSC(lObjConexionSC, lObjTransaccionSC, pIntIdRepuestoSC);
                            lObjTransaccionSC.Rollback();
                            return lBlnExisteSC ? ResultadoRepuestoOrdenSC.StockInsuficiente : ResultadoRepuestoOrdenSC.RepuestoNoExiste;
                        }
                    }

                    const string lStrInsertarSC = @"
                        INSERT INTO dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad)
                        VALUES (@IdOrden, @IdRepuesto, @Cantidad);";
                    using (SqlCommand lObjComandoSC = new SqlCommand(lStrInsertarSC, lObjConexionSC, lObjTransaccionSC))
                    {
                        lObjComandoSC.Parameters.Add("@IdOrden", SqlDbType.Int).Value = pIntIdOrdenSC;
                        lObjComandoSC.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = pIntIdRepuestoSC;
                        lObjComandoSC.Parameters.Add("@Cantidad", SqlDbType.Int).Value = pIntCantidadSC;
                        lObjComandoSC.ExecuteNonQuery();
                    }

                    lObjTransaccionSC.Commit();
                    return ResultadoRepuestoOrdenSC.Correcto;
                }
            }
        }

        /// <summary>
        /// Quita un repuesto registrado en una orden abierta (por ejemplo, si se registró por
        /// error) y devuelve sus unidades al stock.
        /// </summary>
        /// <param name="pIntIdOrdenSC">Orden de reparación a la que pertenece el registro.</param>
        /// <param name="pIntIdRepuestoOrdenSC">Registro que se quita.</param>
        /// <returns>Correcto, o el motivo por el que no se quitó.</returns>
        public ResultadoRepuestoOrdenSC FncQuitarUsoSC(int pIntIdOrdenSC, int pIntIdRepuestoOrdenSC)
        {
            using (SqlConnection lObjConexionSC = ClsConexionDASC.FncObtenerConexionSC())
            {
                lObjConexionSC.Open();
                using (SqlTransaction lObjTransaccionSC = lObjConexionSC.BeginTransaction())
                {
                    ResultadoRepuestoOrdenSC lEnmEstadoOrdenSC = FncBloquearOrdenAbiertaSC(lObjConexionSC, lObjTransaccionSC, pIntIdOrdenSC);
                    if (lEnmEstadoOrdenSC != ResultadoRepuestoOrdenSC.Correcto)
                    {
                        lObjTransaccionSC.Rollback();
                        return lEnmEstadoOrdenSC;
                    }

                    // OUTPUT devuelve el repuesto y la cantidad del registro borrado para devolverlos al stock.
                    const string lStrBorrarSC = @"
                        DELETE FROM dbo.RepuestosOrden
                        OUTPUT DELETED.IdRepuesto, DELETED.Cantidad
                        WHERE IdRepuestoOrden = @IdRepuestoOrden AND IdOrden = @IdOrden;";
                    int lIntIdRepuestoSC;
                    int lIntCantidadSC;
                    using (SqlCommand lObjComandoSC = new SqlCommand(lStrBorrarSC, lObjConexionSC, lObjTransaccionSC))
                    {
                        lObjComandoSC.Parameters.Add("@IdRepuestoOrden", SqlDbType.Int).Value = pIntIdRepuestoOrdenSC;
                        lObjComandoSC.Parameters.Add("@IdOrden", SqlDbType.Int).Value = pIntIdOrdenSC;
                        using (SqlDataReader lObjLectorSC = lObjComandoSC.ExecuteReader())
                        {
                            if (!lObjLectorSC.Read())
                            {
                                lObjLectorSC.Close();
                                lObjTransaccionSC.Rollback();
                                return ResultadoRepuestoOrdenSC.UsoNoExiste;
                            }
                            lIntIdRepuestoSC = lObjLectorSC.GetInt32(0);
                            lIntCantidadSC = lObjLectorSC.GetInt32(1);
                        }
                    }

                    const string lStrDevolverSC = "UPDATE dbo.Repuesto SET stock = stock + @Cantidad WHERE idrepuesto = @IdRepuesto;";
                    using (SqlCommand lObjComandoSC = new SqlCommand(lStrDevolverSC, lObjConexionSC, lObjTransaccionSC))
                    {
                        lObjComandoSC.Parameters.Add("@Cantidad", SqlDbType.Int).Value = lIntCantidadSC;
                        lObjComandoSC.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = lIntIdRepuestoSC;
                        lObjComandoSC.ExecuteNonQuery();
                    }

                    lObjTransaccionSC.Commit();
                    return ResultadoRepuestoOrdenSC.Correcto;
                }
            }
        }

        /// <summary>
        /// Comprueba que la orden exista y siga abierta, y la bloquea hasta el final de la
        /// transacción (UPDLOCK, HOLDLOCK): mientras tanto nadie puede cerrarla.
        /// </summary>
        private static ResultadoRepuestoOrdenSC FncBloquearOrdenAbiertaSC(SqlConnection pObjConexionSC,
            SqlTransaction pObjTransaccionSC, int pIntIdOrdenSC)
        {
            const string lStrConsultaSC =
                "SELECT Estado FROM dbo.OrdenesReparacion WITH (UPDLOCK, HOLDLOCK) WHERE IdOrden = @IdOrden;";
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, pObjConexionSC, pObjTransaccionSC))
            {
                lObjComandoSC.Parameters.Add("@IdOrden", SqlDbType.Int).Value = pIntIdOrdenSC;
                object lObjEstadoSC = lObjComandoSC.ExecuteScalar();
                if (lObjEstadoSC == null)
                {
                    return ResultadoRepuestoOrdenSC.OrdenNoExiste;
                }
                return Convert.ToString(lObjEstadoSC) == "Abierta"
                    ? ResultadoRepuestoOrdenSC.Correcto
                    : ResultadoRepuestoOrdenSC.OrdenCerrada;
            }
        }

        /// <summary>Indica si un repuesto existe (dentro de la transacción en curso).</summary>
        private static bool FncExisteRepuestoSC(SqlConnection pObjConexionSC, SqlTransaction pObjTransaccionSC, int pIntIdRepuestoSC)
        {
            const string lStrConsultaSC = "SELECT COUNT(1) FROM dbo.Repuesto WHERE idrepuesto = @IdRepuesto;";
            using (SqlCommand lObjComandoSC = new SqlCommand(lStrConsultaSC, pObjConexionSC, pObjTransaccionSC))
            {
                lObjComandoSC.Parameters.Add("@IdRepuesto", SqlDbType.Int).Value = pIntIdRepuestoSC;
                return Convert.ToInt32(lObjComandoSC.ExecuteScalar()) > 0;
            }
        }
    }
}
