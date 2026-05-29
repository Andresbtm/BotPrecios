using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class RepositorioPrecio : IRepositorioPrecio
    {
        private readonly IConexion _conexion;

        public RepositorioPrecio(IConexion conexion)
        {
            _conexion = conexion;
        }

        public void Insertar(decimal valor, string fuente, int idProducto, int idSupermercado)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_PRECIO.PR_INSERTAR_PRECIO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_valor", OracleDbType.Decimal)
                    {
                        Value = valor
                    });
                    cmd.Parameters.Add(new OracleParameter("p_fuente", OracleDbType.Varchar2)
                    {
                        Value = fuente ?? "manual"
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_supermercado", OracleDbType.Int32)
                    {
                        Value = idSupermercado
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<PrecioDetalle> ListarRecientesPorProducto(int idProducto)
        {
            List<PrecioDetalle> lista = new List<PrecioDetalle>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_PRECIO.PR_LISTAR_RECIENTES_POR_PRODUCTO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new PrecioDetalle
                            {
                                IdPrecio = Convert.ToInt32(reader["id_precio"]),
                                Valor = Convert.ToDecimal(reader["valor"]),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                Fuente = reader["fuente"] == DBNull.Value ? "" : reader["fuente"].ToString(),
                                IdProducto = Convert.ToInt32(reader["id_producto"]),
                                IdSupermercado = Convert.ToInt32(reader["id_supermercado"]),
                                NombreSupermercado = reader["supermercado"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public List<Precio> ObtenerTodos()
        {
            List<Precio> lista = new List<Precio>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_PRECIO.PR_LISTAR_TODOS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Precio
                            {
                                IdPrecio = Convert.ToInt32(reader["id_precio"]),
                                Valor = Convert.ToDecimal(reader["valor"]),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                Fuente = reader["fuente"] == DBNull.Value ? "" : reader["fuente"].ToString(),
                                IdProducto = Convert.ToInt32(reader["id_producto"]),
                                IdSupermercado = Convert.ToInt32(reader["id_supermercado"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public decimal ObtenerPrecioMinimo(int idProducto)
        {
            return EjecutarFuncionDecimal(
                "BEGIN :resultado := PKG_PRECIO.FX_PRECIO_MINIMO(:p_id_producto); END;",
                idProducto);
        }

        public decimal ObtenerPrecioMaximo(int idProducto)
        {
            return EjecutarFuncionDecimal(
                "BEGIN :resultado := PKG_PRECIO.FX_PRECIO_MAXIMO(:p_id_producto); END;",
                idProducto);
        }

        public decimal ObtenerPrecioPromedio(int idProducto)
        {
            return EjecutarFuncionDecimal(
                "BEGIN :resultado := PKG_PRECIO.FX_PRECIO_PROMEDIO(:p_id_producto); END;",
                idProducto);
        }

        public string ObtenerSupermercadoMasBarato(int idProducto)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand(
                    "BEGIN :resultado := PKG_PRECIO.FX_SUPERMERCADO_MAS_BARATO(:p_id_producto); END;", con))
                {
                    cmd.Parameters.Add(new OracleParameter("resultado", OracleDbType.Varchar2, 100)
                    {
                        Direction = ParameterDirection.Output
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });

                    cmd.ExecuteNonQuery();
                    return cmd.Parameters["resultado"].Value.ToString();
                }
            }
        }

        // Método privado reutilizable para funciones que devuelven NUMBER
        private decimal EjecutarFuncionDecimal(string sql, int idProducto)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("resultado", OracleDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });

                    cmd.ExecuteNonQuery();
                    return Convert.ToDecimal(cmd.Parameters["resultado"].Value.ToString());
                }
            }
        }
    }
}
