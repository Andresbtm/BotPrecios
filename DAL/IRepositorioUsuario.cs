using ENTITY;

namespace DAL
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        void Insertar(long idChat, string nombre);
        Usuario BuscarPorChat(long idChat);
    }
}
