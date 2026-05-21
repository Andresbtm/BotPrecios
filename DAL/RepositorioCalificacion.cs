using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RepositorioCalificacion
    {
        private readonly Conexion _conexion = new Conexion();

        public void Insertar(int puntaje, int idProducto, int idSupermercado, int idUsuario)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_CALIFICACION.PR_INSERTAR_CALIFICACION", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_puntaje", OracleDbType.Int32)
                    {
                        Value = puntaje
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_supermercado", OracleDbType.Int32)
                    {
                        Value = idSupermercado
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_usuario", OracleDbType.Int32)
                    {
                        Value = idUsuario
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Calificacion> ListarPorProductoSuper(int idProducto, int idSupermercado)
        {
            List<Calificacion> lista = new List<Calificacion>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_CALIFICACION.PR_LISTAR_POR_PRODUCTO_SUPER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_producto", OracleDbType.Int32)
                    {
                        Value = idProducto
                    });
                    cmd.Parameters.Add(new OracleParameter("p_id_supermercado", OracleDbType.Int32)
                    {
                        Value = idSupermercado
                    });
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Calificacion
                            {
                                IdCalificacion = Convert.ToInt32(reader["id_calificacion"]),
                                Puntaje = Convert.ToInt32(reader["puntaje"]),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                IdProducto = Convert.ToInt32(reader["id_producto"]),
                                IdSupermercado = Convert.ToInt32(reader["id_supermercado"]),
                                IdUsuario = Convert.ToInt32(reader["id_usuario"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public double ObtenerPromedio(int idProducto, int idSupermercado)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand(
                    "BEGIN :resultado := PKG_CALIFICACION.FX_PROMEDIO_CALIFICACION(:p_id_producto, :p_id_supermercado); END;", con))
                {
                    cmd.Parameters.Add(new OracleParameter("resultado", OracleDbType.Decimal)
                    {
                        Direction = ParameterDirection.Output
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
                    return Convert.ToDouble(cmd.Parameters["resultado"].Value.ToString());
                }
            }
        }

        public int ObtenerTotalVotos(int idProducto, int idSupermercado)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand(
                    "BEGIN :resultado := PKG_CALIFICACION.FX_TOTAL_CALIFICACIONES(:p_id_producto, :p_id_supermercado); END;", con))
                {
                    cmd.Parameters.Add(new OracleParameter("resultado", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
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
                    return Convert.ToInt32(cmd.Parameters["resultado"].Value.ToString());
                }
            }
        }
    }
}
