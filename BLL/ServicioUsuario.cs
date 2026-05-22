using DAL;
using ENTITY;

namespace BLL
{
    public class ServicioUsuario
    {
        private readonly IRepositorioUsuario _repositorio;

        public ServicioUsuario(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        public void RegistrarSiNoExiste(long idChat, string nombre)
        {
            _repositorio.Insertar(idChat, nombre);
        }

        public Usuario BuscarPorChat(long idChat)
        {
            return _repositorio.BuscarPorChat(idChat);
        }
    }
}
