using ENTITY;

namespace DAL
{
    public interface IRepositorioUsuario
    {
        void Insertar(long idChat, string nombre);
        Usuario BuscarPorChat(long idChat);
    }
}
