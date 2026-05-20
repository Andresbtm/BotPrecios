using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioPrecio
    {
        private readonly RepositorioPrecio _repositorio = new RepositorioPrecio();

        public void Insertar(decimal valor, string fuente, int idProducto, int idSupermercado)
        {
            _repositorio.Insertar(valor, fuente, idProducto, idSupermercado);
        }

        public List<PrecioDetalle> ObtenerPrecios(int idProducto)
        {
            return _repositorio.ListarRecientesPorProducto(idProducto);
        }

        public string ObtenerComparacionPrecios(int idProducto)
        {
            var precios = _repositorio.ListarRecientesPorProducto(idProducto);

            if (precios.Count == 0)
                return "⚠️ No hay precios registrados para este producto.";

            var sb = new StringBuilder();
            sb.AppendLine("🏪 *Comparación de precios:*\n");

            for (int i = 0; i < precios.Count; i++)
            {
                string icono = i == 0 ? "🟢" : "⚪";
                sb.AppendLine($"{icono} *{precios[i].NombreSupermercado}*: `${precios[i].Valor:N0}` por kg");
            }

            sb.AppendLine($"\n_Actualizado: {precios[0].FechaRegistro:dd/MM/yyyy HH:mm}_");
            return sb.ToString();
        }
    }
}
