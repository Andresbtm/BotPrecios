using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioUsuario
    {
        private readonly RepositorioUsuario _repositorio = new RepositorioUsuario();

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
