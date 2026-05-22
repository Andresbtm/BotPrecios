using DAL;
using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public class ServicioSupermercado
    {
        private readonly IRepositorioSupermercado _repositorio;

        public ServicioSupermercado(IRepositorioSupermercado repositorio)
        {
            _repositorio = repositorio;
        }

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
