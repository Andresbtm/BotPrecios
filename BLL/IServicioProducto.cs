using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public interface IServicioProducto
    {
        string ObtenerMenuCategoria(string categoria, string emoji, string titulo);
        string ObtenerInfoProducto(string comando);
        string ObtenerComparacionConCalificacion(string comando);
        (string emoji, string titulo) ObtenerDatosCategoria(string categoria);
        string ObtenerCategoriaPorComando(string comando);
        List<Producto> ObtenerProductosPorCategoria(string categoria);
        Producto ObtenerProductoPorComando(string comando);
    }
}
