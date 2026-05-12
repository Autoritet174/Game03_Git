using Cysharp.Threading.Tasks;
using General;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;

namespace Assets.GameData.Scripts
{
    /// <summary>
    /// Статический класс, предназначенный для отправки запросов серверу игры с целью проверки его доступности ("Ping").
    /// </summary>
    internal static class GameServerPinger
    {
        /// <summary>
        /// Внутренний статический экземпляр HTTP-клиента для взаимодействия с удалённым сервером.
        /// </summary>
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Асинхронный метод, отправляющий GET-запрос серверу и проверяющий наличие отклика 'pong'.
        /// Возвращает true, если отклик соответствует ожиданиям, иначе возвращает false. CancellationToken генерируется автоматически из CancellationTokenManager.
        /// </summary>
        /// <returns>True, если сервер ответил "pong"; False в противном случае.</returns>
        internal static async UniTask<bool> PingAsync()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, Url.PING);
                CancellationToken ct = CancellationTokenManager.Create("ping", 5);
                using HttpResponseMessage response = await _httpClient.SendAsync(request, ct).AsUniTask();
                string responseContent = await response.Content.ReadAsStringAsync().AsUniTask();
                if (!string.IsNullOrWhiteSpace(responseContent))
                {

                    var jObject = JObject.Parse(responseContent);
                    if (jObject is not null)
                    {
                        JToken m = jObject["message"];
                        return m != null && string.Compare(m.ToString(), "pong", StringComparison.InvariantCultureIgnoreCase) == 0;
                    }

                }
            }
            catch { }

            return false;
        }
    }
}
