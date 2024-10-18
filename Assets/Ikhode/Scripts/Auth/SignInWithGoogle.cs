using com.example;
using System;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using UnityEngine;
using static Supabase.Gotrue.Constants;
using System.Net;
using AHUI;
using UnityEngine.SceneManagement;  // Add for scene management


    
public class SignInWithGoogle : MonoBehaviour
{
    [SerializeField] private string RedirectUrl = "http://localhost:3700/";
    [SerializeField] private string loginSceneName = "GoogleAuth";
    [SerializeField] private string homeSceneName = "Account";

    // Public Unity References
    public TMP_Text ErrorText = null;
    public SupabaseManager SupabaseManager = null;
    public NotificationManager NotificationManager = null; // Add reference to NotificationManager
    public Transform notificationParent = null;
    public GameObject notificationPrefab = null;

    // Private implementation
    private bool _doSignIn;
    private bool _doSignOut;
    private bool _doExchangeCode;

    private HttpListener httpListener = null;

    // Transactional data
    private string _pkce;
    private string _token;

    public void SignIn()
    {
        _doSignIn = true;
    }

    public void SignOut()
    {
        _doSignOut = true;
    }
    // void Awake()
    // {
    //     string PlayerAuth = PlayerPrefs.GetString("OAuth_Name", null);
    //     if (PlayerAuth != null)
    //     {
    //         SceneManager.LoadScene(homeSceneName);
    //     }
    // }

    private async void Update()
    {
        // Removed auto sign-out on start

        if (_doSignOut)
        {
            _doSignOut = false;
            await SupabaseManager.Supabase()!.Auth.SignOut();

            // Check if NotificationManager is assigned before using it
            if (NotificationManager != null && notificationParent != null && notificationPrefab != null)
            {
                NotificationManager.ShowNotification("Signed Out", "You have been signed out successfully.", notificationParent, notificationPrefab);
            }

            // Clear all saved user data from PlayerPrefs
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // Change scene after sign-out
            SceneManager.LoadScene(loginSceneName);
        }

        if (_doExchangeCode)
        {
            _doExchangeCode = false;
            await PerformExchangeCode();
        }

        if (_doSignIn)
        {
            _doSignIn = false;
            await PerformSignIn();
        }
    }

    private void StartLocalWebserver()
    {
        if (httpListener == null)
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(RedirectUrl);
            httpListener.Start();
            httpListener.BeginGetContext(new AsyncCallback(IncomingHttpRequest), httpListener);
        }
    }

    private void IncomingHttpRequest(IAsyncResult result)
    {
        Debug.Log("IncomingHttpRequest");

        HttpListener httpListener = (HttpListener)result.AsyncState;
        HttpListenerContext httpContext = httpListener.EndGetContext(result);

        HttpListenerRequest httpRequest = httpContext.Request;
        _token = httpRequest.QueryString.Get("code");

        HttpListenerResponse httpResponse = httpContext.Response;
        string responseString = @"
<html>
    <head>
        <style>
            body {
                font-family: sans-serif;
                text-align: center;
            }
            h1 {
                font-size: 2em;
                margin-bottom: 0.5em;
            }
        </style>
    </head>
    <body>
        <h1>You've been signed in with Google!</h1>
        <p>You can close this tab/window now.</p>
    </body>
</html>
";        
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        httpListener.Stop();
        httpListener = null;
        _doExchangeCode = true;
    }

    private async Task PerformSignIn()
    {
        try
        {
            if (SupabaseManager.Supabase() == null)
            {
                if (ErrorText != null)
                {
                    ErrorText.text = "SupabaseManager is not set.";
                }

                // Check if NotificationManager is assigned before using it
                if (NotificationManager != null && notificationParent != null && notificationPrefab != null)
                {
                    NotificationManager.ShowNotification("Login Error", "SupabaseManager is not set.", notificationParent, notificationPrefab);
                }
                return;
            }

            var providerAuth = (await SupabaseManager.Supabase()!.Auth.SignIn(Provider.Google, new SignInOptions
            {
                FlowType = OAuthFlowType.PKCE,
            }))!;
            _pkce = providerAuth.PKCEVerifier;

            StartLocalWebserver();
            Application.OpenURL(providerAuth.Uri.ToString());
        }
        catch (GotrueException goTrueException)
        {
            if (ErrorText != null)
            {
                ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
            }

            // Check if NotificationManager is assigned before using it
            if (NotificationManager != null && notificationParent != null && notificationPrefab != null)
            {
                NotificationManager.ShowNotification("Login Error", $"{goTrueException.Reason}: {goTrueException.Message}", notificationParent, notificationPrefab);
            }
            Debug.Log(goTrueException.Message, gameObject);
            Debug.LogException(goTrueException, gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message, gameObject);

            // Check if NotificationManager is assigned before using it
            if (NotificationManager != null && notificationParent != null && notificationPrefab != null)
            {
                NotificationManager.ShowNotification("Login Error", $"An error occurred: {e.Message}", notificationParent, notificationPrefab);
            }
            Debug.Log(e, gameObject);
        }
    }

    private async Task PerformExchangeCode()
{
    try
    {
        // Exchange authorization code for a session
        Session session = (await SupabaseManager.Supabase()!.Auth.ExchangeCodeForSession(_pkce, _token)!);

        var name = session.User?.UserMetadata.ContainsKey("name") == true 
            ? session.User.UserMetadata["name"].ToString() 
            : "Unknown Name";

        // Store encrypted session details in PlayerPrefs
        PlayerPrefs.SetString("OAuth_Name", name);
        PlayerPrefs.SetString("OAuth_Email", session.User?.Email ?? "Unknown Email");

        // Encrypt and store tokens securely
        string encryptedAccessToken = EncryptionHelper.Encrypt(session.AccessToken);
        string encryptedRefreshToken = EncryptionHelper.Encrypt(session.RefreshToken);

        PlayerPrefs.SetString("OAuth_AccessToken", encryptedAccessToken);
        PlayerPrefs.SetString("OAuth_RefreshToken", encryptedRefreshToken);
        PlayerPrefs.SetString("OAuth_UID", session.User?.Id ?? "Unknown User ID");
        PlayerPrefs.SetString("OAuth_CreatedAt", session.User?.CreatedAt.ToString() ?? "Unknown Creation Date");
        PlayerPrefs.SetString("OAuth_LastSignIn", DateTime.Now.ToString());
        PlayerPrefs.SetString("OAuth_ProfilePic", session.User?.UserMetadata.ContainsKey("avatar_url") == true ? session.User.UserMetadata["avatar_url"].ToString() : "No Profile Picture");
        PlayerPrefs.Save();

        if (ErrorText != null)
        {
            ErrorText.text = $"Success! Signed Up as {name} {session.User?.Email}";
        }

        // Change scene after sign-in
        SceneManager.LoadScene(homeSceneName);
    }
    catch (GotrueException goTrueException)
    {
        Debug.LogError($"Error during sign-in: {goTrueException.Message}");
        if (ErrorText != null)
        {
            ErrorText.text = "Error: Sign-in failed!";
        }
    }
}

    }

