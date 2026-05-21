using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class RepositorioProducto
    {
        private readonly Conexion _conexion = new Conexion();

        public List<Producto> ObtenerPorCategoria(string nombreCategoria)
        {
            List<Producto> lista = new List<Producto>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                int l_idCategoria = ObtenerIdCategoria(con, nombreCategoria);
                if (l_idCategoria == 0) return lista;

                using (OracleCommand cmd = new OracleCommand("PKG_PRODUCTO.PR_LISTAR_POR_CATEGORIA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_categoria", OracleDbType.Int32)
                    {
                        Value = l_idCategoria
                    });
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearProducto(reader));
                        }
                    }
                }
            }

            return lista;
        }

        public Producto ObtenerPorComando(string comando)
        {
            return ObtenerTodos().Find(p => p.Comando == comando);
        }

        public List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new List<Producto>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_PRODUCTO.PR_LISTAR_TODOS", con))
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
                            lista.Add(MapearProducto(reader));
                        }
                    }
                }
            }

            return lista;
        }

        private int ObtenerIdCategoria(OracleConnection con, string nombreCategoria)
        {
            using (OracleCommand cmd = new OracleCommand("PKG_CATEGORIA.PR_LISTAR_CATEGORIAS", con))
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
                        if (reader["nombre"].ToString().ToLower() == nombreCategoria.ToLower())
                            return Convert.ToInt32(reader["id_categoria"]);
                    }
                }
            }

            return 0;
        }

        private Producto MapearProducto(OracleDataReader reader)
        {
            string l_nombre = reader["nombre"].ToString();

            return new Producto
            {
                Id = Convert.ToInt32(reader["id_producto"]),
                Nombre = l_nombre,
                Unidad = reader["unidad"] == DBNull.Value ? "" : reader["unidad"].ToString(),
                Emoji = reader["emoji"] == DBNull.Value ? "" : reader["emoji"].ToString(),
                IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                Comando = "/" + l_nombre
                                  .ToLower()
                                  .Replace(" ", "")
                                  .Replace("á", "a").Replace("é", "e")
                                  .Replace("í", "i").Replace("ó", "o")
                                  .Replace("ú", "u")
            };
        }
    }
}
