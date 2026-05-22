using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioCalificacion
    {
        void Insertar(int puntaje, int idProducto, int idSupermercado, int idUsuario);
        List<Calificacion> ListarPorProductoSuper(int idProducto, int idSupermercado);
        double ObtenerPromedio(int idProducto, int idSupermercado);
        int ObtenerTotalVotos(int idProducto, int idSupermercado);
    }
}
