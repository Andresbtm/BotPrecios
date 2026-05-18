using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioSupermercado
    {
        private readonly RepositorioSupermercado _repositorio = new RepositorioSupermercado();

        public void Insertar(string nombre, string ciudad)
        {
            _repositorio.Insertar(nombre, ciudad);
        }

        public List<Supermercado> ListarTodos()
        {
            return _repositorio.ListarTodos();
        }

        public List<Supermercado> ListarActivos()
        {
            return _repositorio.ListarActivos();
        }

        public Supermercado BuscarPorId(int idSupermercado)
        {
            return _repositorio.BuscarPorId(idSupermercado);
        }

        public void ActualizarActivo(int idSupermercado, int activo)
        {
            _repositorio.ActualizarActivo(idSupermercado, activo);
        }
    }
}
