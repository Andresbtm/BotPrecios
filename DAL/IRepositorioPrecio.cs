using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioPrecio : IRepositorio<Precio>
    {
        void Insertar(decimal valor, string fuente, int idProducto, int idSupermercado);
        List<PrecioDetalle> ListarRecientesPorProducto(int idProducto);
        decimal ObtenerPrecioMinimo(int idProducto);
        decimal ObtenerPrecioMaximo(int idProducto);
        decimal ObtenerPrecioPromedio(int idProducto);
        string ObtenerSupermercadoMasBarato(int idProducto);
    }
}
