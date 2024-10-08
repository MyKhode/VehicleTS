using UnityEngine;

[CreateAssetMenu(fileName = "QualitySettings", menuName = "Settings/QualitySettings")]
public class QualitySettingsObject : ScriptableObject
{
    [Header("Quality Settings")]
    public int anisotropicFiltering = 2; // Example values: 0 = Disabled, 1 = PerTexture, 2 = ForcedOn
    public bool textureStreaming = true;
    public int shadowCascades = 2; // 1 = No Cascades, 2 = Two Cascades, 3 = Three Cascades, 4 = Four Cascades
    public float shadowDistance = 100f;
    public int shadowResolution = 2; // Example values: 0 = Low, 1 = Medium, 2 = High
    public int shadowmaskMode = 0; // Example values: 0 = Distance Shadowmask, 1 = Shadowmask

    // Add more quality settings as needed
}
