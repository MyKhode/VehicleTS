using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VehicleDataSyncManager : MonoBehaviour
{
    public SupabaseModelManager supabaseModelManager; // Reference to your Supabase model manager
    private VehicleData[] vehicleData;

    private async void Start()
    {
        LoadVehicleData();
        await SyncVehicleDataWithDatabase();
    }

    private void LoadVehicleData()
    {
        // Load all VehicleData assets from the Resources folder
        vehicleData = Resources.LoadAll<VehicleData>("");

        // Check if vehicleData is populated
        if (vehicleData.Length > 0)
        {
            Debug.Log("VehicleData assets successfully loaded.");
        }
        else
        {
            Debug.LogWarning("No VehicleData assets found in Resources.");
        }
    }

    private async Task SyncVehicleDataWithDatabase()
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
                // Check if the vehicle is owned in the database
                bool isOwned = await supabaseModelManager.IsVehicleOwned(oAuthUID, vehicle.ItemID);

                // Sync the IsOwned property with the database
                vehicle.IsOwned = isOwned;

                // Log the result for debugging
                Debug.Log($"Vehicle ID: {vehicle.ItemID}, IsOwned: {vehicle.IsOwned}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error syncing vehicle data: {ex.Message}");
        }
    }
}
