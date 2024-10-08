using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalVehicleManager : MonoBehaviour
{
    public DecalProjector decalTop, decalLeft, decalRight;
    public VehicleData vehicleData;
    public GameStatsSO gameStats;

    private Texture2D decalTopTexture;
    private Texture2D decalSideTexture;
    private Material decalMaterial;

    void Start()
    {
        if (vehicleData == null)
        {
            Debug.LogError("vehicleData is not assigned.");
            return;
        }

        if (decalTop == null || decalLeft == null || decalRight == null)
        {
            Debug.LogError("One or more decal projectors are not assigned.");
            return;
        }

        LoadDecalTextures();

        if (gameStats.currentState == GameStatsSO.GameState.Garage)
        {
            vehicleData.OnDecalDataChanged.AddListener(LoadDecalTextures);
            LoadDecalTextures(); // Initial setup
        }
    }


    void OnDestroy()
    {
        if (vehicleData != null)
        {
            vehicleData.OnDecalDataChanged.RemoveListener(LoadDecalTextures);
        }
    }

    void UpdateDecals()
    {
        ApplyDecal(decalTop, decalTopTexture, vehicleData.Decal_Offset_Top, vehicleData.Decal_Width_Top, vehicleData.Decal_Height_Top, vehicleData.DisplayDecalTop);
        ApplyDecal(decalLeft, decalSideTexture, vehicleData.Decal_Offset_Left, vehicleData.Decal_Width_Left, vehicleData.Decal_Height_Left, vehicleData.DisplayDecalLeft);
        ApplyDecal(decalRight, decalSideTexture, vehicleData.Decal_Offset_Right, vehicleData.Decal_Width_Right, vehicleData.Decal_Height_Right, vehicleData.DisplayDecalRight);
    }

    void LoadDecalTextures()
    {
        decalTopTexture = LoadTextureFromPath(vehicleData.Decal_Top_URL);
        decalSideTexture = LoadTextureFromPath(vehicleData.Decal_Side_URL);

        if (decalTopTexture == null && decalSideTexture == null)
        {
            Debug.LogWarning("No decal textures could be loaded.");
            return; // Early exit
        }

        UpdateDecals(); // Only update once both textures are loaded
    }


    Texture2D LoadTextureFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Texture path is null or empty.");
            return null;
        }

        return Resources.Load<Texture2D>(path);
    }

    void ApplyDecal(DecalProjector decalProjector, Texture2D decalTexture, Vector2 decalOffset, float decalWidth, float decalHeight, bool isVisible)
    {
        if (decalProjector == null)
        {
            Debug.LogWarning("DecalProjector is null, cannot apply decal.");
            return;
        }

        if (isVisible && decalTexture != null)
        {
            // Create a new material every time the decal is applied to ensure the material changes.
            Material newDecalMaterial = new Material(Shader.Find("Shader Graphs/Decal"));
            if (newDecalMaterial.shader == null)
            {
                Debug.LogError("Shader 'Shader Graphs/Decal' not found.");
                return;
            }

            // Set the new decal texture
            newDecalMaterial.SetTexture("Base_Map", decalTexture);

            // Apply the new material to the projector
            decalProjector.material = newDecalMaterial;

            // Adjust position, size, and other properties
            decalProjector.transform.localPosition = new Vector3(decalProjector.transform.localPosition.x, decalOffset.x, decalOffset.y);
            decalProjector.size = new Vector3(decalWidth, decalHeight, decalProjector.size.z);
            decalProjector.gameObject.SetActive(true);

            Debug.Log("Decal applied with new material and texture.");
        }
        else
        {
            // Hide the decal if it's not visible or if the texture is null
            decalProjector?.gameObject.SetActive(false);
            Debug.Log("Decal projector is now hidden.");
        }
    }


}
