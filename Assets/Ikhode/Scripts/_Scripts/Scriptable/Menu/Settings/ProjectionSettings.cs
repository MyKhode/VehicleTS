using UnityEngine;

[CreateAssetMenu(fileName = "ProjectionSettings", menuName = "Settings/ProjectionSettings")]
public class ProjectionSettings : ScriptableObject
{
    [Header("Projection")]
    public float fieldOfView = 60f;
    public float nearClipPlane = 0.3f;
    public float farClipPlane = 2000f;
    public string fieldOfViewAxis = "Vertical"; // Vertical or Horizontal

    [Header("Physical Camera")]
    public string sensorType = "Custom";
    public Vector2 sensorSize = new Vector2(36f, 24f); // X and Y dimensions
    public float iso = 200f;
    public float shutterSpeed = 0.005f;
    public string gateFit = "Horizontal"; // Gate fit type (e.g., Horizontal, Vertical)
    public float focalLength = 20.78461f;
    public Vector2 shift = Vector2.zero;
    public float aperture = 16f;
    public float focusDistance = 10f;
    public int bladeCount = 5;
    public float curvature = 2f;
    public float barrelClipping = 0.25f;
    public bool anamorphism = false;
}
