using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class UserDisplayInfoDemo : MonoBehaviour
{
    [Header("User Information UI")]
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private TMP_Text userEmailText;
    [SerializeField] private TMP_Text userCreatedAtText;
    [SerializeField] private TMP_Text userLastSignInText;
    [SerializeField] private TMP_Text userIDText;
    [SerializeField] private Image userProfilePic;

    private void Start()
    {
        DisplayUserInfo();
    }

    // Fetch and display user information
    private void DisplayUserInfo()
    {
        UserInfo userInfo = GetUserInfo();

        // Log user info for debugging purposes
        Debug.Log($"User Info:\nName: {userInfo.Name}\nEmail: {userInfo.Email}\nProfile Pic: {userInfo.ProfilePic}\nUser ID: {userInfo.UserID}\nCreated At: {userInfo.CreatedAt}\nLast Sign In: {userInfo.LastSignIn}");

        // Update UI elements with user info
        UpdateUserInfoUI(userInfo);
    }

    // Retrieve user information from PlayerPrefs
    private UserInfo GetUserInfo()
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

    // Update the UI elements with user information
    private void UpdateUserInfoUI(UserInfo userInfo)
    {
        if (userNameText != null) userNameText.text = userInfo.Name;
        if (userEmailText != null) userEmailText.text = $"Email: {userInfo.Email}";
        if (userCreatedAtText != null) userCreatedAtText.text = $"Created At: {userInfo.CreatedAt}";
        if (userLastSignInText != null) userLastSignInText.text = $"Last Sign In: {userInfo.LastSignIn}";
        if (userIDText != null) userIDText.text = $"UID: {userInfo.UserID}";

        if (userProfilePic != null)
        {
            StartCoroutine(LoadProfilePic(userInfo.ProfilePic));
        }
    }

    // Load a profile picture from a URL
    private IEnumerator LoadProfilePic(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Apply the downloaded texture to the Image component
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                userProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError($"Failed to load profile picture: {www.error}");
            }
        }
    }
}

// Serializable class to hold user information
[System.Serializable]
public class UserInfo
{
    public string Name;
    public string Email;
    public string ProfilePic;
    public string UserID;
    public string CreatedAt;
    public string LastSignIn;
}
