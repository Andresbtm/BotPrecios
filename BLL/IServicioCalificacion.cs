namespace BLL
{
    public interface IServicioCalificacion
    {
        void Insertar(int puntaje, int idProducto, int idSupermercado, int idUsuario);
        double ObtenerPromedio(int idProducto, int idSupermercado);
        int ObtenerTotalVotos(int idProducto, int idSupermercado);
    }
}
