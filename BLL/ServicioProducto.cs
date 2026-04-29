using DAL;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace BLL
{
    public class ServicioProducto
    {
        private readonly RepositorioProducto _repositorio = new RepositorioProducto();

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
            if (producto == null)
                return "Producto no encontrado.";
            return $"{producto.Emoji} *{producto.Nombre}*\n" +
                   $"Precio referencial: $XX.XX por {producto.Unidad}";
        }

        public bool EsComandoProducto(string comando)
        {
            return _repositorio.ObtenerPorComando(comando) != null;
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
            return producto?.Categoria ?? "granos";
        }

        // ── Teclados Inline ──────────────────────────────────────────

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

            // Botón volver al menú principal
            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver", "menu_principal")
            });

            return new InlineKeyboardMarkup(filas);
        }

        // Botones después de ver un producto
        public InlineKeyboardMarkup ObtenerInlineProductoFinal(string categoria)
        {
            var info = ObtenerDatosCategoria(categoria);
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        $"🔙 Volver a {info.titulo}",
                        $"volver_{categoria}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_principal")
                }
            });
        }
    }
}