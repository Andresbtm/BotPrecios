using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioProducto
    {
        List<Producto> ObtenerPorCategoria(string nombreCategoria);
        Producto ObtenerPorComando(string comando);
        List<Producto> ObtenerTodos();
    }
}
