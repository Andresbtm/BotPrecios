using ENTITY;

namespace DAL
{
    public interface IRepositorioCategoria : IRepositorio<Categoria>
    {
        Categoria BuscarPorNombre(string nombre);
    }
}
