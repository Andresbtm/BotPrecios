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
    public class RepositorioUsuario
    {
        private readonly Conexion _conexion = new Conexion();

        public void Insertar(long idChat, string nombre)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_USUARIO.PR_INSERTAR_USUARIO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_chat", OracleDbType.Int64)
                    {
                        Value = idChat
                    });
                    cmd.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2)
                    {
                        Value = nombre ?? "Sin nombre"
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Usuario BuscarPorChat(long idChat)
        {
            using (OracleConnection con = _conexion.AbrirConexion())
            {
                using (OracleCommand cmd = new OracleCommand("PKG_USUARIO.PR_BUSCAR_POR_CHAT", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("p_id_chat", OracleDbType.Int64)
                    {
                        Value = idChat
                    });
                    cmd.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    });

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                IdChat = Convert.ToInt64(reader["id_chat"]),
                                Nombre = reader["nombre"] == DBNull.Value ? "" : reader["nombre"].ToString(),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"])
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
