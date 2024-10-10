using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VehicleDataSyncManager : MonoBehaviour
{
    [SerializeField] private VehicleData[] vehicleData;
    [SerializeField] private UserDisplayInfoDemo userDisplayInfoDemo;

    private async void Awake()
    {
        userDisplayInfoDemo = FindObjectOfType<UserDisplayInfoDemo>();
        if (userDisplayInfoDemo != null)
        {
            LoadVehicleData(); // Load vehicle data
            try
            {
                await SyncVehicleDataWithDatabase(); // Sync data with the database
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error syncing vehicle data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("UserDisplayInfoDemo is not found in the scene.");
        }
    }

    private void LoadVehicleData()
    {
        vehicleData = Resources.LoadAll<VehicleData>("");

        if (vehicleData.Length > 0)
            Debug.Log("VehicleData assets successfully loaded.");
        else
            Debug.LogWarning("No VehicleData assets found in Resources.");
    }

    public async Task SyncVehicleDataWithDatabase()
    {
        string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth_UID not found. Please authenticate first.");
            return;
        }

        try
        {
            foreach (var vehicle in vehicleData)
            {
                bool isOwned = await userDisplayInfoDemo.IsVehicleOwned(oAuthUID, vehicle.ItemID);
                vehicle.IsOwned = isOwned; // Sync ownership status

                Debug.Log($"Vehicle ID: {vehicle.ItemID}, IsOwned: {vehicle.IsOwned}"); // Log sync result
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error syncing vehicle data: {ex.Message}");
        }
    }
}
