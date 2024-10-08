using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Vehicle Data", menuName = "Custom/Vehicle Data")]
public class VehicleData : PurchasableItem
{
    // Basic info
    public string Decal_Top_URL;
    public string Decal_Side_URL;
    public GameObject VehicleGamePlayPrefab;
    public float VehicleScore;

    // Vehicle specifications
    public string Manufacturer;
    public string ModelName;
    public string Year;
    public string Country;
    public float Speed;
    public float Handling;
    public float Acceleration;
    public float LunchTime;
    public float Braking;

    public float VehicleSpeed;

    // Vehicle customization
    public Material VehiclePaint;

    // Boolean stats
    public bool IsSelected;
    public bool IsViewSelected;

    // Decal visibility
    public bool DisplayDecalTop = true;
    public bool DisplayDecalLeft = true;
    public bool DisplayDecalRight = true;

    // Decal properties
    public Vector2 Decal_Offset_Top;
    public Vector2 Decal_Offset_Left;
    public Vector2 Decal_Offset_Right;
    public float Decal_Width_Top, Decal_Height_Top;
    public float Decal_Width_Left, Decal_Height_Left;
    public float Decal_Width_Right, Decal_Height_Right;
    public Texture2D DecalImage;

    // Events for decal data changes
    public UnityEvent OnDecalDataChanged = new UnityEvent();
    public UnityEvent OnDecalTopURLChanged = new UnityEvent();
    public UnityEvent OnDecalSideURLChanged = new UnityEvent();

    // Methods to update decal URLs
    public void UpdateDecalTopURL(string url)
    {
        Decal_Top_URL = url;
        Debug.Log("Decal Top URL updated: " + Decal_Top_URL);
        OnDecalTopURLChanged.Invoke();
        OnDecalDataChanged.Invoke();
    }

    public void UpdateDecalSideURL(string url)
    {
        Decal_Side_URL = url;
        OnDecalSideURLChanged.Invoke();
        OnDecalDataChanged.Invoke();
    }
}
