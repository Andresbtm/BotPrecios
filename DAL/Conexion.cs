using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class Conexion : IConexion
    {
        private const string _cadena =
            "User Id=carlos;" +
            "Password=carlos123;" +
            "Data Source=localhost:1521/XEPDB1;";

        public OracleConnection AbrirConexion()
        {
            OracleConnection conexion = new OracleConnection(_cadena);
            conexion.Open();
            return conexion;
        }
    }
}
