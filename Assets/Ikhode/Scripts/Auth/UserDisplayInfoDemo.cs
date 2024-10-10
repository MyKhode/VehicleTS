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

    [SerializeField] private string oAuthUID;
    [SerializeField] private string oAuthName;

    
    private decimal InitializeCash = 25000m;


    private void Awake()
    {
        InitializeSupabaseClient();
        LoadUserInfoFromPrefs();
    }

    private void InitializeSupabaseClient()
    {
        var options = new SupabaseOptions { AutoConnectRealtime = true };
        client = new Supabase.Client(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);
    }

    private void LoadUserInfoFromPrefs()
    {
        oAuthUID = PlayerPrefs.GetString("OAuth_UID", null);
        oAuthName = PlayerPrefs.GetString("OAuth_Name", null);
    }

    private void Start()
    {
        DisplayUserInfo();
        RefreshUI();
    }

    private async void DisplayUserInfo()
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
            ProfilePic = PlayerPrefs.GetString("OAuth_ProfilePic", "No Profile Picture"),
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

        if (userProfilePic != null)
        {
            StartCoroutine(LoadProfilePic(userInfo.ProfilePic));
        }
    }

    private IEnumerator LoadProfilePic(string url)
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

    private void RefreshUI()
    {
        DisplayUserCashAsync();
    }

    private async Task DisplayUserCashAsync()
    {
        decimal playerCash = await GetPlayerCash();

        if (userCashText != null)
        {
            userCashText.text = playerCash.ToString();
        }
    }

    public async Task<decimal> GetPlayerCash()
    {
        if (string.IsNullOrEmpty(oAuthUID)) return 0;

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

    public async Task InitializePlayerIfNotExists(string uid, string username)
    {
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("OAuth UID is null or empty.");
            return;
        }

        try
        {
            var existingPlayer = await client.From<Users>()
                                             .Filter("id", Operator.Equals, uid)
                                             .Single();

            if (existingPlayer == null)
            {
                var newPlayer = new Users
                {
                    ID = uid,
                    Cash = InitializeCash // You may replace this with a method or value
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
