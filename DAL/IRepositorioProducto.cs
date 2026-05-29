using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioProducto : IRepositorio<Producto>
    {
        List<Producto> ObtenerPorCategoria(string nombreCategoria);
        Producto ObtenerPorComando(string comando);
    }
}
