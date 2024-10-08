using com.example;
using Supabase;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;  // Add this line

public class SupabaseModelManager : MonoBehaviour
{
    public SupabaseSettings SupabaseSettings;
    private Supabase.Client client;
    private User currentUser;

    private async void Awake()
    {
        var options = new SupabaseOptions { AutoConnectRealtime = true };
        client = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
        await client.InitializeAsync();

        var oAuthSession = PlayerPrefs.GetString("OAuth_UID", null);
        Debug.Log(oAuthSession != null ? "Authenticated" : "Not authenticated");

        await InitializePlayerWithOAuth();
    }

    public async Task<decimal> GetPlayerCash()
    {
        string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth_UID not found. Please authenticate first.");
            return 0;
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID);

            var result = await client.From<Player>().Filter("uid", Operator.Equals, oAuthUID).Single();
            return result?.Cash ?? 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch player cash: {ex.Message}");
            return 0;
        }
    }

    public async Task UpdatePlayerCash(decimal newCash)
    {
        string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth_UID not found. Please authenticate first.");
            return;
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID);

            var existingPlayer = await client.From<Player>().Filter("uid", Operator.Equals, oAuthUID).Single();
            if (existingPlayer != null)
            {
                var player = new Player
                {
                    UID = oAuthUID,
                    Username = existingPlayer.Username,
                    Cash = newCash
                };

                var result = await client.From<Player>().Upsert(player);
                if (result == null)
                {
                    Debug.LogError("Failed to update player cash: Upsert returned null.");
                }
            }
            else
            {
                Debug.LogError("Player not found for the provided OAuth UID.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating player cash: {ex.Message}");
        }
    }
   public async Task AddPurchase(int vehicleID)
{
    string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
    if (string.IsNullOrEmpty(oAuthUID))
    {
        Debug.LogError("OAuth_UID not found. Please authenticate first.");
        return;
    }

    try
    {
        await InitializePlayerIfNotExists(oAuthUID);

        var existingPurchases = await client.From<Purchases>().Filter("player_uid", Operator.Equals, oAuthUID).Single();

        if (existingPurchases == null)
        {
            var purchase = new Purchases
            {
                PlayerUID = oAuthUID,
                VehicleID = vehicleID.ToString(), // Changed to direct assignment
                PurchaseDate = DateTime.UtcNow
            };

            var resultInsert = await client.From<Purchases>().Insert(purchase);
            if (resultInsert == null)
            {
                Debug.LogError("Failed to add purchase: Insert returned null.");
            }
            else
            {
                Debug.Log($"Successfully added purchase for Player UID: {oAuthUID}, Vehicle ID: {vehicleID}");
            }
        }
        else
        {
            var existingVehicleIDs = existingPurchases.VehicleID.Trim('(', ')').Split(',')
                                    .Where(id => !string.IsNullOrWhiteSpace(id))
                                    .Select(int.Parse).ToList();

            if (!existingVehicleIDs.Contains(vehicleID))
            {
                existingVehicleIDs.Add(vehicleID);
                existingPurchases.VehicleID = $"({string.Join(",", existingVehicleIDs)})";

                var resultUpdate = await client.From<Purchases>().Update(existingPurchases);
                if (resultUpdate == null)
                {
                    Debug.LogError("Failed to update purchase: Update returned null.");
                }
                else
                {
                    Debug.Log($"Successfully added vehicle ID {vehicleID} to existing purchases for Player UID: {oAuthUID}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error adding purchase: {ex.Message}");
    }
}

    public async Task RemovePurchase(int vehicleID)
    {
        string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth_UID not found. Please authenticate first.");
            return;
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID);  // Ensure player exists before removing purchase

            // Fetch existing purchases
            var existingPurchases = await client.From<Purchases>().Filter("player_uid", Operator.Equals, oAuthUID).Single();

            if (existingPurchases != null)
            {
                var existingVehicleIDs = existingPurchases.VehicleID.Trim('(', ')').Split(',').Select(int.Parse).ToList();
                if (existingVehicleIDs.Contains(vehicleID))
                {
                    existingVehicleIDs.Remove(vehicleID);
                    existingPurchases.VehicleID = $"({string.Join(",", existingVehicleIDs)})";

                    var resultUpdate = await client.From<Purchases>().Update(existingPurchases);
                    if (resultUpdate == null)
                    {
                        Debug.LogError("Failed to remove purchase: Update returned null.");
                    }
                    else
                    {
                        Debug.Log($"Successfully removed vehicle ID {vehicleID} from purchases for Player UID: {oAuthUID}");
                    }
                }
                else
                {
                    Debug.LogError($"Vehicle ID {vehicleID} not found in purchases.");
                }
            }
            else
            {
                Debug.LogError("No purchases found for the provided OAuth UID.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error removing purchase: {ex.Message}");
        }
    }


    private async Task InitializePlayerWithOAuth()
    {
        try
        {
            string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
            if (!string.IsNullOrEmpty(oAuthUID))
            {
                string accessToken = PlayerPrefs.GetString("OAuth_AccessToken", null);
                string refreshToken = PlayerPrefs.GetString("OAuth_RefreshToken", null);

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    await InitializePlayerIfNotExists(oAuthUID);
                }
                else
                {
                    Debug.LogError("Session not found. Please log in first.");
                }
            }
            else
            {
                Debug.LogError("OAuth_UID not found. Please authenticate first.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing player: {ex.Message}");
        }
    }

    private async Task InitializePlayerIfNotExists(string oAuthUID)
    {
        var existingPlayer = await client.From<Player>().Filter("uid", Operator.Equals, oAuthUID).Single();
        if (existingPlayer == null)
        {
            string oAuthName = PlayerPrefs.GetString("OAuth_Name", "New Player");

            var newPlayer = new Player
            {
                UID = oAuthUID,
                Username = oAuthName,
                Cash = 0m
            };

            var result = await client.From<Player>().Insert(newPlayer);
            if (result == null)
            {
                Debug.LogError("Failed to initialize player data.");
            }
            else
            {
                Debug.Log($"Initialized player data for UID: {oAuthUID} with username: {oAuthName}");
            }
        }
    }

    private string Decrypt(string encryptedData)
    {
        // Implement your decryption logic here
        return encryptedData; // Placeholder
    }
}
