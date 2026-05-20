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
        private static ServicioCalificacion _servicioCalificacion = new ServicioCalificacion();

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
                    var comparacion = _servicioProducto.ObtenerComparacionConCalificacion(comando);
                    var tecladoCalif = _servicioProducto.ObtenerInlineCalificar(comando);

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
                        parseMode: ParseMode.Markdown,
                        replyMarkup: tecladoCalif,
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
                // ── Seleccionó supermercado para calificar ────────────────
                else if (data.StartsWith("calif_"))
                {
                    var partes = data.Split('_');
                    var idProducto = int.Parse(partes[1]);
                    var idSupermercado = int.Parse(partes[2]);

                    var teclado = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("⭐ 1", $"puntaje_{idProducto}_{idSupermercado}_1"),
                            InlineKeyboardButton.WithCallbackData("⭐⭐ 2", $"puntaje_{idProducto}_{idSupermercado}_2"),
                            InlineKeyboardButton.WithCallbackData("⭐⭐⭐ 3", $"puntaje_{idProducto}_{idSupermercado}_3")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("⭐⭐⭐⭐ 4", $"puntaje_{idProducto}_{idSupermercado}_4"),
                            InlineKeyboardButton.WithCallbackData("⭐⭐⭐⭐⭐ 5", $"puntaje_{idProducto}_{idSupermercado}_5")
                        }
                    });

                    await bot.SendMessage(
                        chatId: chatId,
                        text: "¿Qué calificación le das a este producto en este supermercado?",
                        replyMarkup: teclado,
                        cancellationToken: ct
                    );
                }
                // ── Guardó el puntaje ─────────────────────────────────────
                else if (data.StartsWith("puntaje_"))
                {
                    var partes = data.Split('_');
                    var idProducto = int.Parse(partes[1]);
                    var idSupermercado = int.Parse(partes[2]);
                    var puntaje = int.Parse(partes[3]);

                    var usuarioDb = _servicioUsuario.BuscarPorChat(chatId);
                    if (usuarioDb == null)
                    {
                        await bot.SendMessage(
                            chatId: chatId,
                            text: "❓ Usa /start primero.",
                            cancellationToken: ct
                        );
                        return;
                    }

                    try
                    {
                        _servicioCalificacion.Insertar(
                            puntaje: puntaje,
                            idProducto: idProducto,
                            idSupermercado: idSupermercado,
                            idUsuario: usuarioDb.IdUsuario
                        );

                        await bot.SendMessage(
                            chatId: chatId,
                            text: $"✅ Calificación guardada: {new string('⭐', puntaje)}",
                            cancellationToken: ct
                        );
                    }
                    catch (Exception ex)
                    {
                        await bot.SendMessage(
                            chatId: chatId,
                            text: $"⚠️ {ex.Message}",
                            cancellationToken: ct
                        );
                    }
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