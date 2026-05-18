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
        private static ServicioUsuario _servicioUsuario = new ServicioUsuario();

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

                // registrar usuario automáticamente si es la primera vez
                _servicioUsuario.RegistrarSiNoExiste(
                    idChat: chatId,
                    nombre: update.Message.From?.FirstName + " " + update.Message.From?.LastName
                );

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
                var usuario = query.From?.FirstName;

                // registrar usuario automáticamente si es la primera vez
                _servicioUsuario.RegistrarSiNoExiste(
                    idChat: chatId,
                    nombre: query.From?.FirstName + " " + query.From?.LastName
                );

                Console.WriteLine($"[{usuario}] botón: {data}");

                await bot.AnswerCallbackQuery(
                    callbackQueryId: query.Id,
                    cancellationToken: ct
                );

                // ── Categorías (edita el mensaje actual) ─────────────
                if (data == "cat_granos" || data == "cat_aceites" || data == "cat_carnes")
                {
                    var cat = data.Replace("cat_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

                    await bot.EditMessageText(
                        chatId: chatId,
                        messageId: messageId,
                        text: _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo),
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _servicioProducto.ObtenerInlineProductos(cat),
                        cancellationToken: ct
                    );
                }
                // ── Producto (edita con info, envía nuevo con supermercado) ──
                else if (data.StartsWith("prod_"))
                {
                    var comando = "/" + data.Replace("prod_", "");
                    var cat = _servicioProducto.ObtenerCategoriaPorComando(comando);
                    var infoProducto = _servicioProducto.ObtenerInfoProducto(comando);
                    var comparacion = _servicioProducto.ObtenerComparacionPrecios(comando);
                    var tecladoVolver = _servicioProducto.ObtenerInlineProductoFinal(cat);

                    await bot.EditMessageText(
                        chatId: chatId,
                        messageId: messageId,
                        text: infoProducto,
                        parseMode: ParseMode.Markdown,
                        replyMarkup: null,
                        cancellationToken: ct
                    );

                    await bot.SendMessage(
                        chatId: chatId,
                        text: comparacion,
                        replyMarkup: tecladoVolver,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct
                    );
                }
                // ── Volver a categoría desde lista de productos (edita) ──
                else if (data.StartsWith("volver_cat_"))
                {
                    var cat = data.Replace("volver_cat_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

                    try
                    {
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo),
                            parseMode: ParseMode.Markdown,
                            replyMarkup: _servicioProducto.ObtenerInlineProductos(cat),
                            cancellationToken: ct
                        );
                    }
                    catch (ApiRequestException ex) when (ex.Message.Contains("message is not modified")) { }
                }
                // ── Volver a categoría desde supermercado (envía nuevo) ──
                else if (data.StartsWith("volver_prod_"))
                {
                    var cat = data.Replace("volver_prod_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

                    await bot.SendMessage(
                        chatId: chatId,
                        text: _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo),
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _servicioProducto.ObtenerInlineProductos(cat),
                        cancellationToken: ct
                    );
                }
                // ── Menú principal desde categoría (edita el mensaje actual) ──
                else if (data == "menu_principal")
                {
                    try
                    {
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: "📦 *Elige una categoría:*",
                            parseMode: ParseMode.Markdown,
                            replyMarkup: _servicioProducto.ObtenerInlineCategorias(),
                            cancellationToken: ct
                        );
                    }
                    catch (ApiRequestException ex) when (ex.Message.Contains("message is not modified")) { }
                }
                // ── Menú principal desde supermercado (envía nuevo mensaje) ──
                else if (data == "menu_nuevo")
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "📦 *Elige una categoría:*",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _servicioProducto.ObtenerInlineCategorias(),
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