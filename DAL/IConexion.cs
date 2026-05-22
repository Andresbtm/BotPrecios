using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public interface IConexion
    {
        OracleConnection AbrirConexion();
    }
}
