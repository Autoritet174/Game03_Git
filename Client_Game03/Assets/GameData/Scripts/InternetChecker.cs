using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class InternetChecker
{
    /// <summary>
    /// Комбинированная проверка несколькими методами
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> CheckInternetConnectionAsync()
    {
        try
        {
            // Быстрая проверка доступности сети
            bool reachable = await CheckReachabilityAsync();
            if (!reachable)
            {
                return false;
            }

            //// Проверка через пинг
            //bool pingSuccess = await PingGoogleAsync();

            //// Проверка через HTTP запрос (если пинг неудачный)
            //if (!pingSuccess)
            //{
            //    bool httpSuccess = await CheckHttpConnectionAsync();
            //    return httpSuccess;
            //}

            bool httpSuccess = await CheckHttpConnectionAsync();

            return httpSuccess;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Проверка NetworkReachability
    /// </summary>
    /// <returns></returns>
    private static Task<bool> CheckReachabilityAsync()
    {
        return Task.FromResult(Application.internetReachability != NetworkReachability.NotReachable);
    }

    /// <summary>
    /// Проверка через пинг Google DNS
    /// </summary>
    /// <returns></returns>
    private static async Task<bool> PingGoogleAsync()
    {
        try
        {
            System.Net.NetworkInformation.Ping ping = new();
            PingReply reply = await ping.SendPingAsync("8.8.8.8", 1000);
            return reply != null && reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Проверка через HTTP запрос https://www.google.com
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> CheckHttpConnectionAsync(string url = "https://www.google.com")
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 2;
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        for (int i = 0; i < 20; i++)
        {
            if (operation.isDone)
            {
                break;
            }
            await Task.Delay(100);
        }

        return request.result != UnityWebRequest.Result.ConnectionError;
    }
}