using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public ProjectionSettings projectionSettings;
    public RenderingSettings renderingSettings;
    public QualitySettingsObject qualitySettings;
    public GeneralSettings generalSettings;

    void Start()
    {
        ApplySettings();
    }

    void ApplySettings()
    {
        ApplyProjectionSettings(Camera.main);
        ApplyRenderingSettings(Camera.main);
        ApplyQualitySettings();
        ApplyGeneralSettings();
    }

    void ApplyProjectionSettings(Camera camera)
    {
        if (camera != null)
        {
            camera.fieldOfView = projectionSettings.fieldOfView;
            camera.nearClipPlane = projectionSettings.nearClipPlane;
            camera.farClipPlane = projectionSettings.farClipPlane;
            camera.aspect = projectionSettings.sensorSize.x / projectionSettings.sensorSize.y;

            // Handle physical camera settings, assuming you have a physical camera script or component
            // Custom scripts or components may be needed for certain settings like sensorType, focalLength, etc.
        }
    }

    void ApplyRenderingSettings(Camera camera)
    {
        if (camera != null)
        {
            // Implement rendering settings
            // Example: camera.renderingPath = RenderingPath.UsePlayerSettings;
            // Apply settings like post-processing, anti-aliasing, etc.
        }
    }

    void ApplyQualitySettings()
    {
        // Apply quality settings
        UnityEngine.QualitySettings.anisotropicFiltering = (AnisotropicFiltering)qualitySettings.anisotropicFiltering;
        // Continue applying quality settings as needed
    }

    void ApplyGeneralSettings()
    {
        // Apply general settings
        UnityEngine.QualitySettings.vSyncCount = generalSettings.vSync_Settings ? 1 : 0;
        UnityEngine.QualitySettings.realtimeReflectionProbes = generalSettings.realtimeReflectionProbes;
    }
}
