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
using System.Linq;

public class SupabaseModelManager : MonoBehaviour
{
    public SupabaseSettings SupabaseSettings;
    private Supabase.Client client;
    private User currentUser;

    [SerializeField] private string vehicleID = "(1)"; // This is the ID of the vehicle being purchased
    [SerializeField] private string oAuthUID;
    [SerializeField] private string oAuthName;

    private decimal InitializeCash = 25000m;


    private async void Awake()
    {
        var options = new SupabaseOptions { AutoConnectRealtime = true };
        client = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
        await client.InitializeAsync();

        // Fetch oAuthUID and oAuthName from PlayerPrefs
        oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        oAuthName = PlayerPrefs.GetString("OAuth_Name", null);

        if (!string.IsNullOrEmpty(oAuthUID))
        {
            // Proceed with player initialization only if UID is valid
            await InitializePlayerWithOAuth();
            await InitializePurchasesWithOAuth(oAuthUID, vehicleID);
        }
    }

    public async Task<decimal> GetPlayerCash()
    {
        if (string.IsNullOrEmpty(oAuthUID))
        {
            return 0; // Return 0 if UID is not available
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID, oAuthName);

            var result = await client.From<Users>().Filter("id", Operator.Equals, oAuthUID).Single();
            return result?.Cash ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task UpdatePlayerCash(decimal newCash)
    {
        if (string.IsNullOrEmpty(oAuthUID))
        {
            return; // Exit if UID is not available
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID, oAuthName);

            var existingPlayer = await client.From<Users>().Filter("id", Operator.Equals, oAuthUID).Single();
            if (existingPlayer != null)
            {
                existingPlayer.Cash = newCash; // Update the cash value
                await client.From<Users>().Upsert(existingPlayer);
            }
        }
        catch { }
    }

    public async Task AddPurchase(int vehicleID)
    {
        if (string.IsNullOrEmpty(oAuthUID))
        {
            return; // Exit if UID is not available
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID, oAuthName);

            var existingPurchases = await client.From<Purchases>().Filter("player_uid", Operator.Equals, oAuthUID).Single();

    
        
                var existingVehicleIDs = existingPurchases.VehicleID.Trim('(', ')').Split(',')
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(int.Parse).ToList();

                if (!existingVehicleIDs.Contains(vehicleID))
                {
                    existingVehicleIDs.Add(vehicleID);
                    existingPurchases.VehicleID = $"({string.Join(",", existingVehicleIDs)})";

                    await client.From<Purchases>().Update(existingPurchases);
                }
            
        }
        catch { }
    }

    public async Task RemovePurchase(int vehicleID)
    {
        if (string.IsNullOrEmpty(oAuthUID))
        {
            return; // Exit if UID is not available
        }

        try
        {
            await InitializePlayerIfNotExists(oAuthUID, oAuthName);

            var existingPurchases = await client.From<Purchases>().Filter("player_uid", Operator.Equals, oAuthUID).Single();

            if (existingPurchases != null)
            {
                var existingVehicleIDs = existingPurchases.VehicleID.Trim('(', ')').Split(',')
                    .Select(int.Parse).ToList();

                if (existingVehicleIDs.Contains(vehicleID))
                {
                    existingVehicleIDs.Remove(vehicleID);
                    existingPurchases.VehicleID = $"({string.Join(",", existingVehicleIDs)})";

                    await client.From<Purchases>().Update(existingPurchases);
                }
            }
        }
        catch { }
    }

    private async Task InitializePlayerWithOAuth()
    {
        if (!string.IsNullOrEmpty(oAuthUID))
        {
            await InitializePlayerIfNotExists(oAuthUID, oAuthName);
        }
    }
    public async Task InitializePlayerIfNotExists(string oAuthUID, string username)
    {
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth UID is null or empty.");
            return; // Exit if UID is not available
        }

        try
        {
            var existingPlayer = await client.From<Users>()
                                            .Filter("id", Operator.Equals, oAuthUID) // Ensure it's treated as a string
                                            .Single();

            if (existingPlayer == null)
            {
                var newPlayer = new Users
                {
                    ID = oAuthUID,  // Ensure this value is not null
                    Cash = InitializeCash
                };

                await client.From<Users>().Insert(newPlayer);
                Debug.Log($"New player initialized with UID: {oAuthUID}");
            }
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Invalid format for oAuthUID: {oAuthUID} - {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing player: {ex.Message}");
        }
    }


public async Task InitializePurchasesWithOAuth(string oAuthUID, string vehicleID)
{
    try
    {
        // Fetch the existing player from the database
        var existingPlayer = await client.From<Users>()
                                         .Filter("id", Operator.Equals, oAuthUID)
                                         .Single();

        if (existingPlayer == null)
        {
            Debug.LogError($"Player with UID {oAuthUID} not found.");
            return;
        }

        // Check if the player has any existing purchases
        var existingPurchases = await client.From<Purchases>()
                                            .Filter("player_uid", Operator.Equals, oAuthUID)
                                            .Single();

        if (existingPurchases == null)
        {
            // If no previous purchases exist, add a new purchase
            var newPurchase = new Purchases
            {
                PlayerUID = oAuthUID,
                VehicleID = vehicleID,
                PurchaseDate = DateTime.UtcNow
            };

            await client.From<Purchases>().Insert(newPurchase);
            Debug.Log($"Purchase initialized for vehicle ID: {vehicleID}");
        }
        else
        {
            Debug.Log($"Player {oAuthUID} already has existing purchases.");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error initializing purchases: {ex.Message}");
    }
}



    public async Task<bool> IsVehicleOwned(string playerUID, int vehicleID)
    {
        try
        {
            var existingPurchases = await client.From<Purchases>()
                                                .Filter("player_uid", Operator.Equals, playerUID)
                                                .Single();

            if (existingPurchases != null)
            {
                var existingVehicleIDs = existingPurchases.VehicleID
                    .Trim('(', ')')
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                return existingVehicleIDs.Contains(vehicleID);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
