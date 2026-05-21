using DAL;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace BLL
{
    public class ServicioProducto
    {
        private readonly RepositorioProducto _repositorio = new RepositorioProducto();
        private readonly ServicioPrecio _servicioPrecio = new ServicioPrecio();
        private readonly ServicioCalificacion _servicioCalificacion = new ServicioCalificacion();

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
            switch (categoria)
            {
                case "granos": return ("🌾", "Granos");
                case "aceites": return ("🫙", "Aceites");
                case "carnes": return ("🥩", "Carnes");
                default: return ("📦", categoria);
            }
        }
        public string ObtenerCategoriaPorComando(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null) return "granos";

            switch (producto.IdCategoria)
            {
                case 1: return "granos";
                case 2: return "aceites";
                case 3: return "carnes";
                default: return "granos";
            }
        }

        public InlineKeyboardMarkup ObtenerInlineCalificar(string comando)
        {
            var producto = _repositorio.ObtenerPorComando(comando);
            if (producto == null) return null;

            var precios = _servicioPrecio.ObtenerPrecios(producto.Id);
            var filas = new List<InlineKeyboardButton[]>();

            foreach (var p in precios)
            {
                filas.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        $"🏪 {p.NombreSupermercado}",
                        $"calif_{producto.Id}_{p.IdSupermercado}")
                });
            }

            var cat = ObtenerCategoriaPorComando(comando);
            var info = ObtenerDatosCategoria(cat);

            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"🔙 Volver a {info.titulo}", $"volver_prod_{cat}")
            });
            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_nuevo")
            });

            return new InlineKeyboardMarkup(filas);
        }

        // Menú principal de categorías
        public InlineKeyboardMarkup ObtenerInlineCategorias()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🌾 Granos",  "cat_granos"),
                    InlineKeyboardButton.WithCallbackData("🫙 Aceites", "cat_aceites")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🥩 Carnes",  "cat_carnes")
                }
            });
        }

        // Productos de una categoría generados dinámicamente
        public InlineKeyboardMarkup ObtenerInlineProductos(string categoria)
        {
            var productos = _repositorio.ObtenerPorCategoria(categoria);
            var filas = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < productos.Count; i += 2)
            {
                if (i + 1 < productos.Count)
                {
                    filas.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{productos[i].Emoji} {productos[i].Nombre}",
                            $"prod_{productos[i].Comando.Replace("/", "")}"),
                        InlineKeyboardButton.WithCallbackData(
                            $"{productos[i+1].Emoji} {productos[i+1].Nombre}",
                            $"prod_{productos[i+1].Comando.Replace("/", "")}")
                    });
                }
                else
                {
                    filas.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{productos[i].Emoji} {productos[i].Nombre}",
                            $"prod_{productos[i].Comando.Replace("/", "")}")
                    });
                }
            }

            // Botón volver edita el mensaje actual
            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver", "menu_principal")
            });

            return new InlineKeyboardMarkup(filas);
        }
    }
}