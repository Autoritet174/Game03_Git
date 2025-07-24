using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GameData.Scripts
{
    internal static class HttpHelper
    {/// <summary>
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
    }
}
