using DAL;
using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public class ServicioPrecio : IServicioPrecio
    {
        private readonly IRepositorioPrecio _repositorio;

        public ServicioPrecio(IRepositorioPrecio repositorio)
        {
            _repositorio = repositorio;
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
    }
}
