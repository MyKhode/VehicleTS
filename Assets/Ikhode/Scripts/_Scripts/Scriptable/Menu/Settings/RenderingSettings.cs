using UnityEngine;

[CreateAssetMenu(fileName = "RenderingSettings", menuName = "Settings/RenderingSettings")]
public class RenderingSettings : ScriptableObject
{
    [Header("Rendering")]
    public string renderer = "Default Renderer";
    public bool postProcessing = false;
    public string antiAliasing = "No Anti-aliasing";
    public bool stopNaNs = false;
    public bool dithering = false;
    public bool renderShadows = true;
    public int priority = 0;
    public bool opaqueTexture = true;
    public bool depthTexture = true;
    public LayerMask cullingMask = ~0; // Everything
    public bool occlusionCulling = true;
}
