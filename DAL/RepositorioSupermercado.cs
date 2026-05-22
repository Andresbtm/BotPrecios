using ENTITY;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class RepositorioSupermercado : IRepositorioSupermercado
    {
        private readonly IConexion _conexion;

        public RepositorioSupermercado(IConexion conexion)
        {
            _conexion = conexion;
        }

        public void Insertar(string nombre, string ciudad)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_SUPERMERCADO.PR_INSERTAR_SUPERMERCADO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2)
                    {
                        Value = nombre
                    });
                    cmd.Parameters.Add(new OracleParameter("p_ciudad", OracleDbType.Varchar2)
                    {
                        Value = ciudad ?? ""
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Supermercado> ListarTodos()
        {
            return Listar("PKG_SUPERMERCADO.PR_LISTAR_TODOS");
        }

        public List<Supermercado> ListarActivos()
        {
            return Listar("PKG_SUPERMERCADO.PR_LISTAR_ACTIVOS");
        }

        public Supermercado BuscarPorId(int idSupermercado)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_SUPERMERCADO.PR_BUSCAR_POR_ID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
                        if (reader.Read())
                            return MapearSupermercado(reader);
                    }
                }
            }

            return null;
        }

        public void ActualizarActivo(int idSupermercado, int activo)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_SUPERMERCADO.PR_ACTUALIZAR_ACTIVO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_supermercado", OracleDbType.Int32)
                    {
                        Value = idSupermercado
                    });
                    cmd.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Int32)
                    {
                        Value = activo
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<Supermercado> Listar(string procedimiento)
        {
            List<Supermercado> lista = new List<Supermercado>();

            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand(procedimiento, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearSupermercado(reader));
                    }
                }
            }

            return lista;
        }

        private Supermercado MapearSupermercado(OracleDataReader reader)
        {
            return new Supermercado
            {
                IdSupermercado = Convert.ToInt32(reader["id_supermercado"]),
                Nombre = reader["nombre"].ToString(),
                Ciudad = reader["ciudad"] == DBNull.Value ? "" : reader["ciudad"].ToString(),
                Activo = Convert.ToInt32(reader["activo"]),
                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"])
            };
        }
    }
}
