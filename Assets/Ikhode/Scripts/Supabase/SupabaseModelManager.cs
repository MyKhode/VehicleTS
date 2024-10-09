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

public class SupabaseModelManager : MonoBehaviour
{
    public SupabaseSettings SupabaseSettings = null!;
    private Supabase.Client client;
    private User currentUser;

    private async void Awake()
    {
        var options = new SupabaseOptions { AutoConnectRealtime = true };
        client = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
        await client.InitializeAsync();

        var _OAuthSession = PlayerPrefs.GetString("OAuth_UID", null);
        if (_OAuthSession != null)
        {
            Debug.Log($"Authenticated");
        }
        else
        {
            Debug.Log("Not authenticated");
        }

        await InitializePlayerWithOAuth();
          await SubscribeToCashUpdates();
    }
  // Subscribe to real-time cash updates for the player
    private async Task SubscribeToCashUpdates()
    {
        string oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);

<<<<<<< HEAD
        if (string.IsNullOrEmpty(oAuthUID))
        {
            Debug.LogError("OAuth_UID not found. Cannot subscribe to real-time cash updates.");
            return;
        }

        // Subscribe to changes in the "players" table where the UID matches
        client.From<Player>()
              .Filter("uid", Operator.Equals, oAuthUID)
              .On(Supabase.Realtime.Constants.EventType.Update, (payload) =>
              {
                  var updatedPlayer = payload.Record as Player;
                  Debug.Log($"Real-time cash update: {updatedPlayer.Cash}");

                  // Trigger the event to update the cash in the UI
                  OnCashUpdated?.Invoke(updatedPlayer.Cash);
              })
              .Subscribe();
    }
    // Fetch the player's cash value initially
=======

>>>>>>> parent of 839d981 (purchess buy & sell)
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
            var purchase = new Purchases
            {
                PlayerUID = oAuthUID,
                VehicleID = vehicleID,
                PurchaseDate = DateTime.UtcNow
            };

            var result = await client.From<Purchases>().Insert(purchase);
            if (result == null)
            {
                Debug.LogError("Failed to add purchase: Insert returned null.");
            }
            else
            {
                Debug.Log($"Successfully added purchase for Player UID: {oAuthUID}, Vehicle ID: {vehicleID}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error adding purchase: {ex.Message}");
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
                    string oAuthName = PlayerPrefs.GetString("OAuth_Name", "New Player");

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
