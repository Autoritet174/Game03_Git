using Assets.GameData.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Button_Reg : MonoBehaviour
{
    public async void OnClick() {
        await Task.Run(async () => {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GV.Jwt_token);

            string json = JsonConvert.SerializeObject("");
            StringContent content = new(json, Encoding.UTF8, "application/json");

            client.Timeout = TimeSpan.FromSeconds(60);
            //_ = log.AppendLine($"{i} попытка авторизоваться");
            try {
                HttpResponseMessage response = await client.PostAsync(GC.Uri_test, content);

                if (response.IsSuccessStatusCode) {
                    GF.Log("SUCCES");
                }
                else {
                    GF.Log("NOT SUCCES");
                }
            }
            catch (Exception ex) {
                Debug.LogError("[HTTP] " + ex.Message);
            }

        });
    }
}
