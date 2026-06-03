using BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
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
        private static ServicioProducto _servicioProducto = FabricaServicios.CrearServicioProducto();
        private static ServicioUsuario _servicioUsuario = FabricaServicios.CrearServicioUsuario();
        private static ServicioCalificacion _servicioCalificacion = FabricaServicios.CrearServicioCalificacion();

        private static TecladoBot _tecladoBot;
        private static HashSet<long> _esperandoIA = new HashSet<long>();
        private static readonly string _groqApiKey = ConfigurationManager.AppSettings["GroqApiKey"];
        private static readonly HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            _tecladoBot = new TecladoBot(
                FabricaServicios.CrearServicioProducto(),
                FabricaServicios.CrearServicioPrecio(),
                FabricaServicios.CrearServicioCategoria()
            );

            string token = ConfigurationManager.AppSettings["TelegramToken"];
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

                if ( texto == "/precios" || texto == "/start")
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "👋 ¡Hola! Soy el bot de los mejores precios de productos de la canasta básica.\n\n📦 *Elige una categoría:*",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _tecladoBot.ObtenerInlineCategorias(),
                        cancellationToken: ct
                    );
                }
                else if (_esperandoIA.Contains(chatId))
                {
                    await ProcesarConsultaIA(bot, chatId, texto, ct);
                }
                else
                {
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "❓ Usa /start o /precios para comenzar.",
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
                if (data.StartsWith("cat_"))
                {
                    var cat = data.Replace("cat_", "");
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

                    await bot.EditMessageText(
                        chatId: chatId,
                        messageId: messageId,
                        text: _servicioProducto.ObtenerMenuCategoria(cat, info.emoji, info.titulo),
                        parseMode: ParseMode.Markdown,
                        replyMarkup: _tecladoBot.ObtenerInlineProductos(cat),
                        cancellationToken: ct
                    );
                }
                // ── Producto (edita con info, envía nuevo con supermercado) ──
                else if (data.StartsWith("prod_"))
                {
                    var comando = "/" + data.Replace("prod_", "");
                    var cat = _servicioProducto.ObtenerCategoriaPorComando(comando);
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);
                    var infoProducto = _servicioProducto.ObtenerInfoProducto(comando);
                    var comparacion = _servicioProducto.ObtenerComparacionConCalificacion(comando);
                    var teclado = _tecladoBot.ObtenerInlineCalificar(comando, cat, info.titulo);

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
                        replyMarkup: teclado,
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
                            replyMarkup: _tecladoBot.ObtenerInlineProductos(cat),
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
                        replyMarkup: _tecladoBot.ObtenerInlineProductos(cat),
                        cancellationToken: ct
                    );
                }
                // ── Menú principal desde categoría (edita el mensaje actual) ──
                else if (data == "menu_principal")
                {
                    _esperandoIA.Remove(chatId);
                    try
                    {
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: "📦 *Elige una categoría:*",
                            parseMode: ParseMode.Markdown,
                            replyMarkup: _tecladoBot.ObtenerInlineCategorias(),
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
                        replyMarkup: _tecladoBot.ObtenerInlineCategorias(),
                        cancellationToken: ct
                    );
                }
                // ── Seleccionó supermercado para calificar ────────────────
                else if (data.StartsWith("calif_"))
                {
                    var partes = data.Split('_');
                    var idProducto = int.Parse(partes[1]);
                    var idSupermercado = int.Parse(partes[2]);

                    var cat = _servicioProducto.ObtenerCategoriaPorId(idProducto);
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

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
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData($"🔙 Volver a {info.titulo}", $"volver_prod_{cat}")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_nuevo")
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

                    var cat = _servicioProducto.ObtenerCategoriaPorId(idProducto);
                    var info = _servicioProducto.ObtenerDatosCategoria(cat);

                    var tecladoVolver = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData($"🔙 Volver a {info.titulo}", $"volver_prod_{cat}")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_nuevo")
                        }
                    });

                    var usuarioDb = _servicioUsuario.BuscarPorChat(chatId);
                    if (usuarioDb == null)
                    {
                        await bot.SendMessage(
                            chatId: chatId,
                            text: "❓ Usa /start o /precios primero.",
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
                            replyMarkup: tecladoVolver,
                            cancellationToken: ct
                        );
                    }
                    catch (Exception ex)
                    {
                        await bot.SendMessage(
                            chatId: chatId,
                            text: $"⚠️ {ex.Message}",
                            replyMarkup: tecladoVolver,
                            cancellationToken: ct
                        );
                    }
                }

                else if (data == "menu_ia")
                {
                    _esperandoIA.Add(chatId);
                    await bot.SendMessage(
                        chatId: chatId,
                        text: "🤖 *Asistente IA de compras*\n\n" +
                              "Puedes enviarme:\n" +
                              "📋 Una *lista de productos* — te digo dónde comprar más barato.\n" +
                              "🍽️ Una *receta* — te explico cómo hacerla y te doy los mejores precios.\n\n" +
                              "✍️ Escribe tu consulta:",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                            new[] { InlineKeyboardButton.WithCallbackData("🔙 Volver al menú", "menu_nuevo") }
                        }),
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

        static async Task ProcesarConsultaIA(ITelegramBotClient bot, long chatId, string texto, CancellationToken ct)
        {
            _esperandoIA.Remove(chatId);

            await bot.SendMessage(
                chatId: chatId,
                text: "⏳ Consultando con IA...",
                cancellationToken: ct
            );

            try
            {
                var prompt = $@"El usuario escribió: ""{texto}""

Analiza si es una RECETA o una LISTA DE PRODUCTOS.

Responde SOLO en JSON con este formato exacto, sin texto extra, sin bloques de código:

Si es receta:
{{
   ""tipo"": ""receta"",
  ""nombre"": ""Nombre del plato"",
  ""pasos"": [""Paso 1 detallado"", ""Paso 2 detallado"", ""Paso 3 detallado"", ""Paso 4 detallado"", ""Paso 5 detallado""],
  ""ingredientes"": [""Arroz"", ""Pollo"", ""Aceite Vegetal""]
}}

Si es lista:
{{
  ""tipo"": ""lista"",
  ""ingredientes"": [""Leche Entera"", ""Pasta"", ""Frijoles""]
}}

Reglas importantes:
- Los pasos deben ser detallados y específicos para esta receta, mínimo 5 pasos, máximo 8.
- Solo incluye en ingredientes los productos estrictamente necesarios para la receta. No agregues ingredientes genéricos como aceite, sal o pimienta a menos que la receta los requiera de forma esencial.
- Los ingredientes deben coincidir exactamente con estos nombres disponibles:
Arroz, Maíz, Frijoles, Lentejas, Pasta, Aceite Vegetal, Aceite de Maíz, Margarina, Mantequilla,
Pollo, Res, Cerdo, Pescado, Mortadela, Leche Entera, Leche Deslactosada, Leche en Polvo,
Yogur, Queso Costeño, Crema de Leche, Suero Costeño, Agua Botellada, Gaseosa, Jugo Natural,
Jugo en Caja, Café Molido, Café Instantáneo, Chocolate en Polvo, Jabón de Baño, Shampoo,
Jabón Líquido, Detergente en Polvo, Detergente Líquido, Suavizante de Ropa, Blanqueador,
Desinfectante, Papel Higiénico, Toallas de Cocina, Pasta Dental, Desodorante";

                // ── 1. Llamar a Groq ──────────────────────────────
                var body = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.3
                };

                var json = System.Text.Json.JsonSerializer.Serialize(body);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_groqApiKey}");

                var url = "https://api.groq.com/openai/v1/chat/completions";
                var response = await _httpClient.PostAsync(url, content);
                var responseStr = await response.Content.ReadAsStringAsync();

                Console.WriteLine("=== GROQ RESPONSE ===");
                Console.WriteLine(responseStr);
                Console.WriteLine("=====================");

                // ── 2. Extraer texto de Groq ─────────────────────
                string textoIA;
                using (var doc = System.Text.Json.JsonDocument.Parse(responseStr))
                {
                    textoIA = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();
                }

                // Limpiar posibles backticks
                textoIA = textoIA.Trim().Replace("```json", "").Replace("```", "").Trim();

                // ── Parsear JSON de Gemini ───────────────────────────
                string tipo;
                List<string> ingredientes;
                string nombreReceta = null;
                List<string> pasos = null;

                using (var resultado = System.Text.Json.JsonDocument.Parse(textoIA))
                {
                    tipo = resultado.RootElement.GetProperty("tipo").GetString();
                    ingredientes = resultado.RootElement.GetProperty("ingredientes")
                        .EnumerateArray()
                        .Select(e => e.GetString())
                        .ToList();

                    if (tipo == "receta")
                    {
                        nombreReceta = resultado.RootElement.GetProperty("nombre").GetString();
                        pasos = resultado.RootElement.GetProperty("pasos")
                            .EnumerateArray()
                            .Select(e => e.GetString())
                            .ToList();
                    }
                }

                // ── Consultar Oracle ─────────────────────────────────
                var servicioPrecio = FabricaServicios.CrearServicioPrecio();
                var sb = new System.Text.StringBuilder();

                if (tipo == "receta")
                {
                    sb.AppendLine($"🍽️ *{nombreReceta}*\n");
                    sb.AppendLine("📝 *Preparación:*");
                    for (int i = 0; i < pasos.Count; i++)
                        sb.AppendLine($"{i + 1}. {pasos[i]}");
                    sb.AppendLine();
                }

                sb.AppendLine("🛒 *Mejores precios disponibles:*\n");

                foreach (var ingrediente in ingredientes)
                {
                    try
                    {
                        var detalle = servicioPrecio.ObtenerPrecioMinimoConSupermercado(ingrediente);
                        if (detalle != null)
                            sb.AppendLine($"• *{ingrediente}*: ${detalle.Valor:N0} en {detalle.NombreSupermercado}");
                        else
                            sb.AppendLine($"• *{ingrediente}*: sin precio registrado");
                    }
                    catch
                    {
                        sb.AppendLine($"• *{ingrediente}*: sin precio registrado");
                    }
                }

                await bot.SendMessage(
                    chatId: chatId,
                    text: sb.ToString(),
                    parseMode: ParseMode.Markdown,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("🤖 Nueva consulta IA", "menu_ia") },
                        new[] { InlineKeyboardButton.WithCallbackData("🏠 Menú principal", "menu_nuevo") }
                    }),
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                await bot.SendMessage(
                    chatId: chatId,
                    text: $"⚠️ Error procesando tu consulta: {ex.Message}",
                    cancellationToken: ct
                );
            }
        }
    }
}