using System;

public static class GlobalConsts {
    /// <summary>
    /// URL API старт
    /// </summary>
    private const string URL_HEADER = "http://localhost:5141/";

    public const string URL_HEROES_GETALL = URL_HEADER + "heroes/getall";
    public static Uri Uri_login { get; private set; } = new(URL_HEROES_GETALL); // URL авторизации
}
