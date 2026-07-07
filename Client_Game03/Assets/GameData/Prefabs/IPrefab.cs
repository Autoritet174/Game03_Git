internal interface IPrefab
{
    bool initialized { get; }
    float width { get; }
    float height { get; }
    void Initialize();
    void OnResized(float coefHeight, float top = 0, float buttom = 0, float left = 0, float right = 0);

}
