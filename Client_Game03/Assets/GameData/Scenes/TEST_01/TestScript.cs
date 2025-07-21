using Assets.GameData.Scripts;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string hwid = HardwareIdentifier.GetRawDeviceIdentifier();
        Debug.Log("HWID устройства: " + hwid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
