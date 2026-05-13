using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion
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
