using UnityEngine;

[CreateAssetMenu(fileName = "GeneralSettings", menuName = "Settings/GeneralSettings")]
public class GeneralSettings : ScriptableObject
{
    [Header("General Settings")]
    public bool vSync_Settings = true;
    public bool realtimeReflectionProbes = true;

    // Add more general settings as needed
}
