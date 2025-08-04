using Assets.GameData.Scripts;
using UnityEngine;

public class Button_Reg : MonoBehaviour
{
    public void OnClick()
    {
        GameMessage.Show("тут сделать ссылку на регистрацию", true);

        //try
        //{
        //    await Task.Run(async () =>
        //    {
        //        using HttpClient client = new();
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GV.Jwt_token);

        //        string json = JsonConvert.SerializeObject("");
        //        StringContent content = new(json, Encoding.UTF8, "application/json");

        //        client.Timeout = TimeSpan.FromSeconds(60);
        //        //_ = log.AppendLine($"{i} попытка авторизоваться");

        //        string s = client.ToString();
        //        Assets.GameData.Scripts.Logger2.Log("Bearer: " + GV.Jwt_token);
        //        HttpResponseMessage response = await client.PostAsync(General.URLs.Uri_test, content);
        //        // Получаем заголовки ответа в виде строки
        //        string headersString = HttpHelper.GetHeadersAsString(response);

        //        // Получаем тело ответа в виде строки
        //        string bodyString = await response.Content.ReadAsStringAsync();

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Assets.GameData.Scripts.Logger2.Log("SUCCES");
        //        }
        //        else
        //        {
        //            Assets.GameData.Scripts.Logger2.Log("NOT SUCCES");
        //        }
        //    });
        //}
        //catch (Exception ex)
        //{
        //    Assets.GameData.Scripts.Logger2.Log("[HTTP] " + ex.Message);
        //}
    }


}
