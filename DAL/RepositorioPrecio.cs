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
    public class RepositorioPrecio
    {
        private readonly Conexion _conexion = new Conexion();

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
    }
}
