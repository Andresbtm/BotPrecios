using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PruebaConexion
    {
        private readonly Conexion _conexion = new Conexion();

        public string Probar()
        {
            try
            {
                using (OracleConnection con = _conexion.AbrirConexion())
                {
                    return "Conexión exitosa. Estado: " + con.State;
                }
            }
            catch (Exception ex)
            {
                return "Error de conexión: " + ex.Message;
            }
        }
    }
}
