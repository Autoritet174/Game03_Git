using System;

/// <summary>
/// GlobalConsts
/// </summary>
public static class GC {
    /// <summary>
    /// URL API старт
    /// </summary>
    private const string URL_HEADER = "https://localhost:7227/api/";

    /// <summary>
    /// URL авторизации
    /// </summary>
    public static Uri Uri_login { get; private set; } = new(URL_HEADER + "Authentication");

    public static Uri Uri_test { get; private set; } = new(URL_HEADER + "Test");
}
