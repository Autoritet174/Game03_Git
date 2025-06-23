using Assets.GameData.Scripts;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Button_Reg : MonoBehaviour
{
    public async void OnClick()
    {
        await Task.Run(async () =>
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GV.Jwt_token);

            string json = JsonConvert.SerializeObject("");
            StringContent content = new(json, Encoding.UTF8, "application/json");

            client.Timeout = TimeSpan.FromSeconds(60);
            //_ = log.AppendLine($"{i} попытка авторизоватьс€");
            try
            {
                string s = client.ToString();
                GF.Log("Bearer: " + GV.Jwt_token);
                HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_test, content);
                // ѕолучаем заголовки ответа в виде строки
                string headersString = GetHeadersAsString(response);

                // ѕолучаем тело ответа в виде строки
                string bodyString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    GF.Log("SUCCES");
                }
                else
                {
                    GF.Log("NOT SUCCES");
                }
            }
            catch (Exception ex)
            {
                GF.Log("[HTTP] " + ex.Message);
            }

        });
    }

    // ћетод дл€ преобразовани€ заголовков в строку
    private static string GetHeadersAsString(HttpResponseMessage response)
    {
        HttpResponseHeaders headers = response.Headers;
        HttpContentHeaders contentHeaders = response.Content?.Headers;

        string headersString = "HTTP/" + response.Version + " " + (int)response.StatusCode + " " + response.ReasonPhrase + "\n";

        // ƒобавл€ем основные заголовки ответа
        foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>> header in headers)
        {
            headersString += $"{header.Key}: {string.Join(", ", header.Value)}\n";
        }

        // ƒобавл€ем заголовки содержимого (если есть)
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
