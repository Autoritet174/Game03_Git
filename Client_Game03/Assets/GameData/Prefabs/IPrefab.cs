internal interface IPrefab
{
    bool Initialized { get; }
    float Width { get; }
    float Height { get; }
    void Initialize();
    void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0);

}
