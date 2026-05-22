using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public interface IServicioPrecio
    {
        void Insertar(decimal valor, string fuente, int idProducto, int idSupermercado);
        List<PrecioDetalle> ObtenerPrecios(int idProducto);
        decimal ObtenerPrecioMinimo(int idProducto);
        decimal ObtenerPrecioMaximo(int idProducto);
        decimal ObtenerPrecioPromedio(int idProducto);
        string ObtenerSupermercadoMasBarato(int idProducto);
    }
}
