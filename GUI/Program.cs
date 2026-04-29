using BLL;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GUI
{
    internal class Program
    {
        private static ITelegramBotClient _bot;
        private static ServicioProducto _servicioProducto = new ServicioProducto();

        static void Main(string[] args)
        {
            string token = "8232490880:AAGsgiuCRF7M-OfB5qNkhXlhzXn4hp-5j74";
            _bot = new TelegramBotClient(token);

            var cts = new CancellationTokenSource();

            _bot.StartReceiving(
                updateHandler: ManejarUpdateAsync,
                errorHandler: ManejarErrorAsync,
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cancellationToken: cts.Token
            );

            Console.WriteLine("¡Bot Conectado! Presiona Enter para detener.");
            Console.ReadLine();
            cts.Cancel();
        }

        static async Task ManejarUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            // ── Mensaje de texto ─────────────────────────────────────
            if (update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var usuario = update.Message.From?.FirstName;
                var texto = update.Message.Text.Trim().ToLower();

                Console.WriteLine($"[{usuario}] {texto}");

                if (texto == "/start")
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "👋 ¡Hola! Soy el bot de los mejores precios de productos de la canasta básica.\n\n📦 *Elige una categoría:*",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _servicioProducto.ObtenerInlineCategorias(),
                        cancellationToken: ct
                    );
                }
                
                else 
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "❓ Usa /start para comenzar.",
                        cancellationToken: ct
                    );
                }

                return;
            }

            // ── Botón inline presionado ──────────────────────────────
            if (update.CallbackQuery != null)
            {
                var query = update.CallbackQuery;
                var chatId = query.Message.Chat.Id;
                var messageId = query.Message.MessageId;
                var data = query.Data;
                var usuario = query.From?.FirstName; // 👈 nombre del usuario

                Console.WriteLine($"[{usuario}] botón: {data}"); // 👈 log en consola

                await bot.AnswerCallbackQuery(query.Id, cancellationToken: ct);

                string texto = "";
                InlineKeyboardMarkup teclado = null;
                bool esProducto = false;

                // ── Categorías ───────────────────────────────────────
                if (data == "cat_granos" || data == "cat_aceites" || data == "cat_carnes")
                {
                    var cat = data.Replace("cat_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);
                    texto = _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo);
                    teclado = _servicioProducto.ObtenerInlineProductos(cat);
                }
                // ── Volver al menú principal ─────────────────────────
                else if (data == "menu_principal")
                {
                    texto = "📦 *Elige una categoría:*";
                    teclado = _servicioProducto.ObtenerInlineCategorias();
                }
                // ── Volver a categoría ───────────────────────────────
                else if (data.StartsWith("volver_"))
                {
                    var cat = data.Replace("volver_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);
                    texto = _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo);
                    teclado = _servicioProducto.ObtenerInlineProductos(cat);
                }
                // ── Producto final ───────────────────────────────────
                else if (data.StartsWith("prod_"))
                {
                    var comando = data.Replace("prod_", "/");
                    var cat = _servicioProducto.ObtenerCategoriaPorComando("/" + data.Replace("prod_", ""));
                    texto = _servicioProducto.ObtenerInfoProducto("/" + data.Replace("prod_", ""));
                    teclado = _servicioProducto.ObtenerInlineProductoFinal(cat);
                    esProducto = true;
                }

                // Editar el mismo mensaje
                await bot.EditMessageText(
                    chatId: chatId,
                    messageId: messageId,
                    text: texto,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: teclado,
                    cancellationToken: ct
                );

                if (esProducto)
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "🏪 En este supermercado están los mejores precios: (próximamente)",
                        cancellationToken: ct
                    );
                }
            }
        }

        static Task ManejarErrorAsync(ITelegramBotClient bot, Exception ex, HandleErrorSource source, CancellationToken ct)
        {
            if (ex is ApiRequestException apiEx)
                Console.WriteLine($"Error Telegram [{apiEx.ErrorCode}]: {apiEx.Message}");
            else
                Console.WriteLine($"Error: {ex.Message}");

            return Task.CompletedTask;
        }
    }
}