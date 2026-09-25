using Assets.GameData.Scripts;
using UnityEngine;

/// <summary>Служит заготовкой компонента для тестовой сцены.</summary>
public class TestScript : MonoBehaviour
{
    private void Start()
    {
        string hwid = HardwareIdentifier.GetRawDeviceIdentifier();
        Debug.Log("HWID ����������: " + hwid);
    }
}
