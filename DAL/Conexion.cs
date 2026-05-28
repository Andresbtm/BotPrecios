using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace DAL
{
    public class Conexion : IConexion
    {
        public OracleConnection AbrirConexion()
        {
            string cadena = ConfigurationManager.ConnectionStrings["OracleDB"].ConnectionString;
            OracleConnection conexion = new OracleConnection(cadena);
            conexion.Open();
            return conexion;
        }
    }
}