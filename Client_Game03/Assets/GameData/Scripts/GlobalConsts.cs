using System;

public static class GlobalConsts {
    /// <summary>
    /// URL API старт
    /// </summary>
    private const string URL_HEADER = "http://localhost:5141/";

    private const string URL_LOGIN = URL_HEADER + "Authentication";

    /// <summary>
    /// URL авторизации
    /// </summary>
    public static Uri Uri_login { get; private set; } = new(URL_LOGIN);
}
