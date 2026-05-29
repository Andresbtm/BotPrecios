using DAL;
using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public class ServicioCategoria : IServicioCategoria
    {
        private readonly IRepositorioCategoria _repositorio;

        public ServicioCategoria(IRepositorioCategoria repositorio)
        {
            _repositorio = repositorio;
        }

        public List<Categoria> ObtenerTodos()
        {
            return _repositorio.ObtenerTodos();
        }

        public (string emoji, string titulo) ObtenerDatosCategoria(string nombreCategoria)
        {
            var categoria = _repositorio.BuscarPorNombre(nombreCategoria);
            if (categoria == null) return ("📦", nombreCategoria);
            return (categoria.Emoji, categoria.Nombre);
        }

        public string ObtenerNombrePorId(int idCategoria)
        {
            var categorias = _repositorio.ObtenerTodos();
            var categoria = categorias.Find(c => c.IdCategoria == idCategoria);
            return categoria != null ? categoria.Nombre.ToLower() : "granos";
        }
    }
}
