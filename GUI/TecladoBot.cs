using BLL;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace GUI
{
    public class TecladoBot
    {
        private readonly ServicioProducto _servicioProducto;
        private readonly ServicioPrecio _servicioPrecio;
        private readonly ServicioCategoria _servicioCategoria;

        public TecladoBot(ServicioProducto servicioProducto,
                          ServicioPrecio servicioPrecio,
                          ServicioCategoria servicioCategoria)
        {
            _servicioProducto = servicioProducto;
            _servicioPrecio = servicioPrecio;
            _servicioCategoria = servicioCategoria;
        }

        public InlineKeyboardMarkup ObtenerInlineCategorias()
        {
            var categorias = _servicioCategoria.ListarTodas();
            var filas = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < categorias.Count; i += 2)
            {
                if (i + 1 < categorias.Count)
                {
                    filas.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{categorias[i].Emoji} {categorias[i].Nombre}",
                            $"cat_{categorias[i].Nombre.ToLower()}"),
                        InlineKeyboardButton.WithCallbackData(
                            $"{categorias[i+1].Emoji} {categorias[i+1].Nombre}",
                            $"cat_{categorias[i+1].Nombre.ToLower()}")
                    });
                }
                else
                {
                    filas.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{categorias[i].Emoji} {categorias[i].Nombre}",
                            $"cat_{categorias[i].Nombre.ToLower()}")
                    });
                }
            }

            return new InlineKeyboardMarkup(filas);
        }

        public InlineKeyboardMarkup ObtenerInlineProductos(string categoria)
        {
            var productos = _servicioProducto.ObtenerProductosPorCategoria(categoria);
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

            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Volver", "menu_principal")
            });

            return new InlineKeyboardMarkup(filas);
        }

        public InlineKeyboardMarkup ObtenerInlineCalificar(string comando,
            string categoria, string tituloCategoria)
        {
            var producto = _servicioProducto.ObtenerProductoPorComando(comando);
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

            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"🔙 Volver a {tituloCategoria}",
                    $"volver_prod_{categoria}")
            });
            filas.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_nuevo")
            });

            return new InlineKeyboardMarkup(filas);
        }
    }
}
