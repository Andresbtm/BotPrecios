using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioCategoria
    {
        List<Categoria> ListarTodas();
        Categoria BuscarPorNombre(string nombre);
    }
}
