using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
                updateHandler: ManejarMensajeAsync,
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

        static async Task ManejarMensajeAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Message == null || update.Message.Text == null) return;

            var chatId = update.Message.Chat.Id;
            var usuario = update.Message.From?.FirstName;
            var texto = update.Message.Text.Trim().ToLower();

            Console.WriteLine($"[{usuario}] {texto}");

            string respuesta;
            bool esProductoFinal = false;

            switch (texto)
            {
                case "/start":
                    respuesta = "👋 ¡Hola! Soy el bot de la canasta básica.\n\n" +
                                "📦 *Elige una categoría:*\n" +
                                "/granos — Granos y cereales\n" +
                                "/aceites — Aceites y grasas\n" +
                                "/carnes — Carnes y proteínas";
                    break;

                case "/granos":
                    respuesta = _servicioProducto.ObtenerMenuCategoria("granos", "🌾", "Granos");
                    break;

                case "/aceites":
                    respuesta = _servicioProducto.ObtenerMenuCategoria("aceites", "🫙", "Aceites");
                    break;

                case "/carnes":
                    respuesta = _servicioProducto.ObtenerMenuCategoria("carnes", "🥩", "Carnes");
                    break;

                default:
                    if (_servicioProducto.EsComandoProducto(texto))
                    {
                        respuesta = _servicioProducto.ObtenerInfoProducto(texto);
                        esProductoFinal = true;
                    }
                    else
                    {
                        respuesta = "❓ Comando no reconocido.\n\nUsa /start para ver las categorías.";
                    }
                    break;
            }

            await bot.SendMessage(
                chatId: chatId,
                text: respuesta,
                parseMode: ParseMode.Markdown,
                cancellationToken: ct
            );

            if (esProductoFinal)
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: "🏪 En este supermercado están los mejores precios: (próximamente)",
                    cancellationToken: ct
                );
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
