using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

using com.example;
using Supabase;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;

public class UserDisplayInfoDemo : MonoBehaviour
{
    [Header("User Information UI")]
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private TMP_Text userEmailText;
    [SerializeField] private TMP_Text userCreatedAtText;
    [SerializeField] private TMP_Text userLastSignInText;
    [SerializeField] private TMP_Text userIDText;
    [SerializeField] private TMP_Text userCashText;
    [SerializeField] private Image userProfilePic;

    [Header("Supabase Settings")]
    public SupabaseSettings SupabaseSettings;
    private Supabase.Client client;

    private string oAuthUID;
    private string oAuthName;
    
    private const decimal InitializeCash = 25000m;

    private async void Awake()
    {
        await InitializeSupabaseClientAsync();
        LoadUserInfoFromPrefs();
        await DisplayUserInfoAsync();
        await RefreshUIAsync();
    }

    private async Task InitializeSupabaseClientAsync()
    {
        var options = new SupabaseOptions { AutoConnectRealtime = true };
        client = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
        await client.InitializeAsync();  // Ensure it's initialized before proceeding
    }

    private void LoadUserInfoFromPrefs()
    {
        oAuthUID = PlayerPrefs.GetString("OAuth_UID", string.Empty);
        oAuthName = PlayerPrefs.GetString("OAuth_Name", "Unknown Name");
    }

    private async Task DisplayUserInfoAsync()
    {
        var userInfo = GetUserInfoFromPrefs();
        UpdateUserInfoUI(userInfo);
        await DisplayUserCashAsync();
    }

    private UserInfo GetUserInfoFromPrefs()
    {
        return new UserInfo
        {
            Name = PlayerPrefs.GetString("OAuth_Name", "Unknown Name"),
            Email = PlayerPrefs.GetString("OAuth_Email", "Unknown Email"),
            ProfilePic = PlayerPrefs.GetString("OAuth_ProfilePic", null),
            UserID = PlayerPrefs.GetString("OAuth_UID", "Unknown User ID"),
            CreatedAt = PlayerPrefs.GetString("OAuth_CreatedAt", "Unknown Creation Date"),
            LastSignIn = PlayerPrefs.GetString("OAuth_LastSignIn", "Unknown Last Sign-In")
        };
    }

    private void UpdateUserInfoUI(UserInfo userInfo)
    {
        userNameText?.SetText(userInfo.Name);
        userEmailText?.SetText($"Email: {userInfo.Email}");
        userCreatedAtText?.SetText($"Created At: {userInfo.CreatedAt}");
        userLastSignInText?.SetText($"Last Sign In: {userInfo.LastSignIn}");
        userIDText?.SetText($"UID: {userInfo.UserID}");

        if (!string.IsNullOrEmpty(userInfo.ProfilePic) && userProfilePic != null)
        {
            StartCoroutine(LoadProfilePicAsync(userInfo.ProfilePic));
        }
    }

    private IEnumerator LoadProfilePicAsync(string url)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                userProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError($"Failed to load profile picture: {request.error}");
            }
        }
    }

    private async Task RefreshUIAsync()
    {
        await DisplayUserCashAsync();
    }

    private async Task DisplayUserCashAsync()
    {
        decimal playerCash = await GetPlayerCashAsync();

        if (userCashText != null)
        {
            userCashText.text = playerCash.ToString("N0");  // Format with comma separators
        }
    }

    public async Task<decimal> GetPlayerCashAsync()
    {
        if (string.IsNullOrEmpty(oAuthUID)) return 0;

        try
        {
            await InitializePlayerIfNotExistsAsync(oAuthUID, oAuthName);
            var result = await client.From<Users>().Filter("id", Operator.Equals, oAuthUID).Single();
            return result?.Cash ?? 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching player cash: {ex.Message}");
            return 0;
        }
    }

    public async Task InitializePlayerIfNotExistsAsync(string uid, string username)
    {
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("OAuth UID is null or empty.");
            return;
        }

        try
        {
            var existingPlayer = await client.From<Users>().Filter("id", Operator.Equals, uid).Single();

            if (existingPlayer == null)
            {
                var newPlayer = new Users
                {
                    ID = uid,
                    Cash = InitializeCash
                };

                await client.From<Users>().Insert(newPlayer);
                Debug.Log($"New player initialized with UID: {uid}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing player: {ex.Message}");
        }
    }

    public async Task<bool> IsVehicleOwnedAsync(string playerUID, int vehicleID)
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
        catch (Exception ex)
        {
            Debug.LogError($"Error checking vehicle ownership: {ex.Message}");
            return false;
        }
    }
}

[System.Serializable]
public class UserInfo
{
    public string Name;
    public string Email;
    public string ProfilePic;
    public string UserID;
    public string CreatedAt;
    public string LastSignIn;

    public override string ToString()
    {
        return $"Name: {Name}, Email: {Email}, ProfilePic: {ProfilePic}, UserID: {UserID}, CreatedAt: {CreatedAt}, LastSignIn: {LastSignIn}";
    }
}
