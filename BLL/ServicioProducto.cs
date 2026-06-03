using DAL;
using ENTITY;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class ServicioProducto : IServicioProducto
    {
        private readonly IRepositorioProducto _repositorio;
        private readonly IServicioPrecio _servicioPrecio;
        private readonly IServicioCalificacion _servicioCalificacion;
        private readonly IServicioCategoria _servicioCategoria;

        public ServicioProducto(IRepositorioProducto repositorio,
                                IServicioPrecio servicioPrecio,
                                IServicioCalificacion servicioCalificacion,
                                IServicioCategoria servicioCategoria)
        {
            _repositorio = repositorio;
            _servicioPrecio = servicioPrecio;
            _servicioCalificacion = servicioCalificacion;
            _servicioCategoria = servicioCategoria;
        }

        // ── Menú de texto ────────────────────────────────────────────
        public string ObtenerMenuCategoria(string categoria, string emoji, string titulo)
        {
            var productos = _repositorio.ObtenerPorCategoria(categoria);
            var sb = new StringBuilder();
            sb.AppendLine($"{emoji} *Categoría: {titulo}*\n");
            sb.AppendLine("Selecciona un producto:");
            foreach (var p in productos)
                sb.AppendLine($"{p.Emoji} {p.Nombre}");
            return sb.ToString();
        }

        public string ObtenerInfoProducto(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null) return "Producto no encontrado.";

            decimal minimo = _servicioPrecio.ObtenerPrecioMinimo(producto.Id);
            decimal maximo = _servicioPrecio.ObtenerPrecioMaximo(producto.Id);
            decimal promedio = _servicioPrecio.ObtenerPrecioPromedio(producto.Id);
            string masBarato = _servicioPrecio.ObtenerSupermercadoMasBarato(producto.Id);

            // Sin precios registrados
            if (minimo == 0)
            {
                return $"{producto.Emoji} *{producto.Nombre}*\n" +
                       $"Unidad: {producto.Unidad}\n\n" +
                       $"⚠️ Aún no hay precios registrados para este producto.";
            }

            return $"{producto.Emoji} *{producto.Nombre}*\n" +
                   $"Unidad: {producto.Unidad}\n\n" +
                   $"💰 Más barato: *{masBarato}* — `${minimo:N0}`\n" +
                   $"📈 Más caro: `${maximo:N0}`\n" +
                   $"📊 Promedio: `${promedio:N0}`";
        }

        public string ObtenerComparacionConCalificacion(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null) return "⚠️ Producto no encontrado.";

            var precios = _servicioPrecio.ObtenerPrecios(producto.Id);
            if (precios.Count == 0)
                return "⚠️ No hay precios registrados para este producto.";

            var sb = new StringBuilder();
            sb.AppendLine("🏪 *Comparación de precios:*\n");

            for (int i = 0; i < precios.Count; i++)
            {
                string icono = i == 0 ? "🟢" : "⚪";
                double promedio = _servicioCalificacion.ObtenerPromedio(producto.Id, precios[i].IdSupermercado);
                int votos = _servicioCalificacion.ObtenerTotalVotos(producto.Id, precios[i].IdSupermercado);
                string estrellas = promedio > 0 ? $"⭐ {promedio}/5 ({votos} votos)" : "Sin calificaciones";

                sb.AppendLine($"{icono} *{precios[i].NombreSupermercado}*: `${precios[i].Valor:N0}`");
                sb.AppendLine($"   {estrellas}");
            }

            sb.AppendLine($"\n_Actualizado: {precios[0].FechaRegistro:dd/MM/yyyy HH:mm}_");
            sb.AppendLine("\nToca un supermercado para calificar:");
            return sb.ToString();
        }

        public (string emoji, string titulo) ObtenerDatosCategoria(string categoria)
        {
            return _servicioCategoria.ObtenerDatosCategoria(categoria);
        }

        public string ObtenerCategoriaPorId(int idProducto)
        {
            var productos = _repositorio.ObtenerTodos();
            var producto = productos.Find(p => p.Id == idProducto);
            if (producto == null) return "granos";
            return _servicioCategoria.ObtenerNombrePorId(producto.IdCategoria);
        }

        public string ObtenerCategoriaPorComando(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null) return "granos";
            return _servicioCategoria.ObtenerNombrePorId(producto.IdCategoria);
        }

        public List<Producto> ObtenerProductosPorCategoria(string categoria)
        {
            return _repositorio.ObtenerPorCategoria(categoria);
        }

        public Producto ObtenerProductoPorComando(string comando)
        {
            return _repositorio.ObtenerPorComando(comando);
        }
    }
}