using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    internal static class HttpHelper
    {
        /// <summary>
        /// Метод для преобразования заголовков в строку
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static string GetHeadersAsString(HttpResponseMessage response)
        {
            HttpResponseHeaders headers = response.Headers;
            HttpContentHeaders contentHeaders = response.Content?.Headers;

            string headersString = $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}\n";

            // Добавляем основные заголовки ответа
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> header in headers)
            {
                headersString += $"{header.Key}: {string.Join(", ", header.Value)}\n";
            }

            // Добавляем заголовки содержимого (если есть)
            if (contentHeaders != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> header in contentHeaders)
                {
                    headersString += $"{header.Key}: {string.Join(", ", header.Value)}\n";
                }
            }

            return headersString;
        }


        private static readonly HttpClient _httpClient;
        static HttpHelper() {
            _httpClient = new();

            //Ini ini = Ini.Create("");
            //ini.Read();
            string s = Application.dataPath;

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Выполняет HTTP POST-запрос с JSON-телом и возвращает ответ в виде строки.
        /// </summary>
        /// <param name="uri">URI конечной точки запроса.</param>
        /// <param name="jsonBody">Тело запроса в формате JSON.</param>
        /// <returns>
        /// Строка с телом ответа при успешном выполнении запроса (код 2xx); в противном случае — null.
        /// </returns>
        /// <exception cref="ArgumentNullException">Если uri или jsonBody равны null.</exception>
        public static async Task<string> TryHttpRequestAsync(Uri uri, string jsonBody)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (jsonBody == null)
            {
                throw new ArgumentNullException(nameof(jsonBody));
            }

            try
            {
                StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync(uri, content);

                return !response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
