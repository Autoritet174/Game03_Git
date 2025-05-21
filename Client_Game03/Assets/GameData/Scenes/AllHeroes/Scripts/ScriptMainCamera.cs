using UnityEngine;

public class ScriptMainCamera : MonoBehaviour
{
    Camera cam;
    void OnEnable() {
        cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }
}
