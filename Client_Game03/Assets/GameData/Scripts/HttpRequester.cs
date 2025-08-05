using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    internal static class HttpRequester
    {

        private static HttpClient _httpClient;
        private static bool inited = false;

        internal static void Init()
        {
            if (inited)
            {
                Debug.LogError("HttpRequester inited");
                return;
            }

            _httpClient = new();

            Ini ini = Ini.Create(Path.Combine(Application.dataPath, @"GameData\Config\Main.ini"));
            if (!double.TryParse(ini.Read("Http", "timeout"), out double timeout))
            {
                timeout = 10;
            }
            _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            inited = true;
        }


        /// <summary>
        /// Метод для преобразования заголовков в строку
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static string GetHeadersAsString(HttpResponseMessage response)
        {
            HttpResponseHeaders headers = response.Headers;
            HttpContentHeaders contentHeaders = response.Content?.Headers;

            StringBuilder headersString = new();
            _ = headersString.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

            // Добавляем основные заголовки ответа
            foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> header in headers)
            {
                _ = headersString.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            // Добавляем заголовки содержимого (если есть)
            if (contentHeaders != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> header in contentHeaders)
                {
                    _ = headersString.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
            }

            return headersString.ToString();
        }


        /// <summary>
        /// Выполняет HTTP POST-запрос с JSON-телом и возвращает ответ в виде JObject.
        /// </summary>
        /// <param name="uri">URI конечной точки запроса.</param>
        /// <param name="jsonBody">Тело запроса в формате JSON.</param>
        /// <returns>
        /// JObject с телом ответа при успешном выполнении запроса (код 2xx); в противном случае — null.
        /// </returns>
        public static async Task<JObject> GetResponceAsync(Uri uri, string jsonBody = "{}")
        {

            if (uri == null)
            {
                //throw new ArgumentNullException(nameof(uri));
                return null;
            }

            if (jsonBody == null)
            {
                jsonBody = "{}";
            }
            else
            {
                jsonBody = jsonBody.Trim();
                if (jsonBody == string.Empty)
                {
                    jsonBody = "{}";
                }
            }

            try
            {
                using HttpRequestMessage request = new(HttpMethod.Post, uri)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrWhiteSpace(GV.Jwt_token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GV.Jwt_token);
                }

                using HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject resultJObject = JObject.Parse(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    return resultJObject;
                }
                else
                {
                    await GameMessage.ShowLocaleAndWaitCloseAsync(LocalizationManager.GetKeyError(resultJObject));
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                await GameMessage.ShowLocaleAndWaitCloseAsync("Errors.Server_Timeout");
            }
            catch (HttpRequestException ex) when (ex.InnerException is WebException)
            {
                bool haveInternet = await InternetChecker.CheckInternetConnectionAsync();
                await GameMessage.ShowLocaleAndWaitCloseAsync(haveInternet ? "Errors.Server_Unavailable" : "Errors.No_internet_connection");
            }
            catch (Exception ex)
            {
                await GameMessage.ShowErrorAndWaitCloseAsync(ex);
            }

            return null;
        }

    }
}
