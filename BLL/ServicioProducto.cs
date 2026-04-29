using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioProducto
    {
        private readonly RepositorioProducto _repositorio = new RepositorioProducto();

        public string ObtenerMenuCategoria(string categoria, string emoji, string titulo)
        {
            var productos = _repositorio.ObtenerPorCategoria(categoria);
            var sb = new StringBuilder();
            sb.AppendLine($"{emoji} *Categoría: {titulo}*\n");
            sb.AppendLine("Selecciona un producto:");

            foreach (var p in productos)
                sb.AppendLine($"{p.Comando} — {p.Nombre}");

            return sb.ToString();
        }

        public string ObtenerInfoProducto(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null)
                return "Producto no encontrado.";

            return $"{producto.Emoji} *{producto.Nombre}*\n" +
                   $"Precio referencial: $XX.XX por {producto.Unidad}";
        }

        public bool EsComandoProducto(string comando)
        {
            return _repositorio.ObtenerPorComando(comando) != null;
        }
    }
}
