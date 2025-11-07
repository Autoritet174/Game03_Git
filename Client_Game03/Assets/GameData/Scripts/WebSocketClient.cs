using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.GameData.Scripts
{
    public class WebSocketClient
    {
        const string serverUrl = "ws://localhost:5001/ws/";
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _serverUri;
        private readonly CancellationTokenSource _cts;
        private bool _isReceiving = false;
        public bool Connected { get; private set; } = false;

        // Событие для уведомления о статусе аутентификации
        //public event Action<bool, string> OnAuthenticationResult;

        public WebSocketClient()
        {
            _webSocket = new ClientWebSocket();
            _serverUri = new Uri(serverUrl);
            _cts = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            
            Exception ex1 = null;


            // Добавляем JWT токен в заголовки, если он предоставлен
            if (!string.IsNullOrEmpty(GlobalVariables.Jwt_token))
            {
                _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {GlobalVariables.Jwt_token}");
            }


            for (int i = 0; i < 3; i++) // делаем 3 попытки подключения
            {
                try
                {
                    await _webSocket.ConnectAsync(_serverUri, CancellationToken.None);

                    // Запускаем прием сообщений без привязки к _cts.Token
                    _isReceiving = true;
                    _ = Task.Run(ReceiveMessagesAsync);
                    Connected = true;
                }
                catch (Exception ex)
                {
                    ex1 = ex;
                    Console.WriteLine($"Ошибка подключения: {ex.Message}");
                    await Task.Delay(1000); // Задержка перед повторной попыткой
                }
                if (Connected)
                {
                    break;
                }
            }
            if (!Connected && ex1 != null)
            {
                GameMessage.Show($"Ошибка подключения: {ex1.Message}", true);
                //OnAuthenticationResult?.Invoke(false, $"Ошибка подключения: {ex1.Message}");
            }
        }

        /// <summary>
        /// Приём сообщений от сервера.
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (_isReceiving && _webSocket.State == WebSocketState.Open)
                {
                    // Используем CancellationToken.None вместо _cts.Token
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //Console.WriteLine($"Получено: {message}");
                    //ProcessReceivedMessage(message);
                }
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                Console.WriteLine("Соединение закрыто сервером");
                //OnAuthenticationResult?.Invoke(false, "Соединение закрыто сервером");
            }
            catch (Exception ex)
            {
                if (_isReceiving) // Логируем только если не было запланированного отключения
                {
                    Console.WriteLine($"Ошибка приема: {ex.Message}");
                    //OnAuthenticationResult?.Invoke(false, $"Ошибка приема: {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Отправить сообщение на сервер.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                Console.WriteLine("WebSocket не подключен");
                return;
            }

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token // Для отправки можно использовать _cts.Token
                );
                //Console.WriteLine($"Отправлено: {message}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Отправка отменена");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки: {ex.Message}");
            }
        }

        public async Task StartSendingMessages(Action<int> onMessagesSent = null)
        {
            while (!_cts.Token.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
            {
                //for (int i = 0; i < 1; i++)
                //{
                //    var message = $"Сообщение #{++messageCount} - {DateTime.Now:HH:mm:ss}";
                //    await SendMessageAsync(message);
                //}
                string com = Console.ReadLine();
                Random r = new();
                if (com == "1")
                {

                    var data = new
                    {
                        command = "AddItem",
                        item = new
                        {
                            damage = r.Next(1, 10),
                            health = r.Next(10, 20),
                            location = Guid.NewGuid()
                        }
                    };

                    // Просто сериализуйте - Newtonsoft.Json по умолчанию не экранирует Unicode
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    Console.WriteLine(json);
                    await SendMessageAsync(json);

                }


                // Сообщаем о количестве отправленных сообщений
                onMessagesSent?.Invoke(100);

                await Task.Delay(10, _cts.Token);

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            onMessagesSent?.Invoke(0); // Завершение
        }
        public async Task DisconnectAsync()
        {
            try
            {
                _isReceiving = false; // Останавливаем прием сообщений
                _cts.Cancel(); // Отменяем операции отправки

                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Закрытие клиентом",
                        CancellationToken.None // Не используем _cts.Token здесь
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отключении: {ex.Message}");
            }
            finally
            {
                _webSocket.Dispose();
                Console.WriteLine("Отключено");
            }
        }
    }
}
