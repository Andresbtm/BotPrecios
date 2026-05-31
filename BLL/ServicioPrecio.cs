using DAL;
using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public class ServicioPrecio : IServicioPrecio
    {
        private readonly IRepositorioPrecio _repositorio;
        private readonly IRepositorioProducto _repositorioProducto;

        public ServicioPrecio(IRepositorioPrecio repositorio, IRepositorioProducto repositorioProducto)
        {
            _repositorio = repositorio;
            _repositorioProducto = repositorioProducto;
        }

        public void Insertar(decimal valor, string fuente, int idProducto, int idSupermercado)
        {
            _repositorio.Insertar(valor, fuente, idProducto, idSupermercado);
        }

        public List<PrecioDetalle> ObtenerPrecios(int idProducto)
        {
            return _repositorio.ListarRecientesPorProducto(idProducto);
        }

        public decimal ObtenerPrecioMinimo(int idProducto)
        {
            return _repositorio.ObtenerPrecioMinimo(idProducto);
        }

        public decimal ObtenerPrecioMaximo(int idProducto)
        {
            return _repositorio.ObtenerPrecioMaximo(idProducto);
        }

        public decimal ObtenerPrecioPromedio(int idProducto)
        {
            return _repositorio.ObtenerPrecioPromedio(idProducto);
        }

        public string ObtenerSupermercadoMasBarato(int idProducto)
        {
            return _repositorio.ObtenerSupermercadoMasBarato(idProducto);
        }

        public PrecioDetalle ObtenerPrecioMinimoConSupermercado(string nombreProducto)
        {
            var producto = _repositorioProducto.ObtenerTodos()
                .Find(p => p.Nombre.ToLower() == nombreProducto.ToLower());

            if (producto == null) return null;

            var valor = _repositorio.ObtenerPrecioMinimo(producto.Id);
            var supermercado = _repositorio.ObtenerSupermercadoMasBarato(producto.Id);

            return new PrecioDetalle
            {
                IdProducto = producto.Id,
                Valor = valor,
                NombreSupermercado = supermercado
            };
        }
    }
}
