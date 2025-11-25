using General;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.GameData.Scripts
{
    internal static class GameServerPinger
    {
        private static readonly HttpClient _httpClient = new();
        internal static async Task<bool> Ping()
        {
            using HttpRequestMessage request = new(HttpMethod.Get, Url.Ping);
            using HttpResponseMessage response = await _httpClient.SendAsync(request, CancelToken.Create("ping", 2));
            string responseContent = await response.Content.ReadAsStringAsync();
            if (!responseContent.IsEmpty())
            {
                try
                {
                    var jObject = JObject.Parse(responseContent);
                    if (jObject is not null)
                    {
                        JToken m = jObject["message"];
                        return m != null && string.Compare(m.ToString(), "pong", StringComparison.InvariantCultureIgnoreCase) == 0;
                    }
                }
                catch { }
            }
            return false;
        }
    }
}
