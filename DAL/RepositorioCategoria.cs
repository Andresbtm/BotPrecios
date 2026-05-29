using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class RepositorioCategoria : IRepositorioCategoria
    {
        private readonly IConexion _conexion;

        public RepositorioCategoria(IConexion conexion)
        {
            _conexion = conexion;
        }

        public List<Categoria> ObtenerTodos()
        {
            List<Categoria> lista = new List<Categoria>();

            using (OracleConnection con = _conexion.AbrirConexion())
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
                            lista.Add(MapearCategoria(reader));
                    }
                }
            }

            return lista;
        }

        public Categoria BuscarPorNombre(string nombre)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_CATEGORIA.PR_BUSCAR_POR_NOMBRE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2)
                    {
                        Value = nombre
                    });
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapearCategoria(reader);
                    }
                }
            }

            return null;
        }

        private Categoria MapearCategoria(OracleDataReader reader)
        {
            return new Categoria
            {
                IdCategoria = Convert.ToInt32(reader["id_categoria"]),
                Nombre = reader["nombre"].ToString(),
                Emoji = reader["emoji"] == DBNull.Value ? "" : reader["emoji"].ToString(),
                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"])
            };
        }
    }
}
