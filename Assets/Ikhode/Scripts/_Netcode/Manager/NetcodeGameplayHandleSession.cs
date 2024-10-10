using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class NetcodeGameplayHandleSession : NetworkBehaviour
{
    public GameObject PlayerUIElementObject; // The main vehicle GameObject
    public Transform ClientparentTransform;
    private Button KickBtn;
    private Button ViewBtn;
    private TextMeshProUGUI ClientID;
    private Image AvatarImage;
    public NetcodePlayerInfo PlayerInfo;

    // Dictionary to track client IDs and their corresponding UI elements
    private Dictionary<ulong, GameObject> clientUIElements = new Dictionary<ulong, GameObject>();

    // Camera reference for the server view
    private RCC_Camera serverCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to connection and disconnection events
        NetworkManager.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;

        serverCamera = GameObject.FindObjectOfType<RCC_Camera>();
        if (serverCamera == null)
        {
            Debug.LogError("NetcodeGameplayHandleSession: Could not find RCC_Camera component in the scene.");
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    // Called when a client connects to the server
    private void HandleClientConnected(ulong clientId)
    {
        PlayerInfo.GetPlayerInfo(clientId);
        Debug.Log("Client Connected: " + clientId);
        var clientElement = Instantiate(PlayerUIElementObject, ClientparentTransform);

        // Store the UI element in the dictionary with clientId as the key
        clientUIElements[clientId] = clientElement;

        KickBtn = clientElement.transform.Find("Button/Kick").GetComponent<Button>();
        KickBtn.onClick.AddListener(() => KickClient(clientId));
        ViewBtn = clientElement.transform.Find("Button/View").GetComponent<Button>();
        ViewBtn.onClick.AddListener(() => StartViewMode(clientId));

        ClientID = clientElement.transform.Find("Text/ClientID").GetComponent<TextMeshProUGUI>();
        AvatarImage = clientElement.transform.Find("Image/Image/avatar").GetComponent<Image>();

        ClientID.text = "#0 " + clientId.ToString();
    }

    // Called when a client disconnects from the server
    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log("Client Disconnected: " + clientId);

        // Destroy the UI element if it exists in the dictionary
        if (clientUIElements.ContainsKey(clientId))
        {
            Destroy(clientUIElements[clientId]);
            clientUIElements.Remove(clientId);
        }
    }

    // Kick a client from the server
    private void KickClient(ulong clientId)
    {
        Debug.Log("Kicking Client: " + clientId);
        NetworkManager.Singleton.DisconnectClient(clientId);

        // Destroy the UI element associated with the kicked client
        if (clientUIElements.ContainsKey(clientId))
        {
            Destroy(clientUIElements[clientId]);
            clientUIElements.Remove(clientId);
        }
    }

    // Start view mode for the server
    private void StartViewMode(ulong clientId)
    {
        ulong vehicleNetworkObjectId = GetClientVehicleNetworkObjectId(clientId);

        if (vehicleNetworkObjectId != 0)
        {
            // Hide the UI panel for the server
            PlayerUIElementObject.SetActive(false);

            // Update the server camera to follow the client vehicle
            UpdateClientCameraClientRpc(clientId, vehicleNetworkObjectId);
        }
    }

    // Method to set player info from the client
    public void SetPlayerInfo(ulong clientId, string username, string avatarUrl)
    {
        if (clientUIElements.ContainsKey(clientId))
        {
            var clientElement = clientUIElements[clientId];

            // Set username in the UI
            var usernameText = clientElement.transform.Find("Text/Username").GetComponent<TextMeshProUGUI>();
            if (usernameText != null)
            {
                usernameText.text = username;
            }

            // Set avatar image in the UI
            var avatarImage = clientElement.transform.Find("Image/Image/avatar").GetComponent<Image>();
            if (avatarImage != null)
            {
                StartCoroutine(LoadAvatarImage(avatarUrl, avatarImage));
            }
        }
    }

    // Coroutine to load the avatar image from a URL
    private IEnumerator LoadAvatarImage(string url, Image avatarImage)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = www.texture;
                avatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load avatar image: " + www.error);
            }
        }
    }

    // ClientRpc to update the camera for the server
    [ClientRpc]
    private void UpdateClientCameraClientRpc(ulong clientId, ulong vehicleNetworkObjectId)
    {
        if (IsOwner && NetworkManager.Singleton.LocalClientId == clientId)
        {
            var vehicle = NetworkManager.SpawnManager.SpawnedObjects[vehicleNetworkObjectId].gameObject;
            var carController = vehicle.GetComponent<RCC_CarControllerV3>();

            if (serverCamera != null && carController != null)
            {
                serverCamera.SetTarget(carController);
                serverCamera.cameraMode = RCC_Camera.CameraMode.TPS;
            }
        }
    }

    private ulong GetClientVehicleNetworkObjectId(ulong clientId)
    {
        return 1; // Replace with actual logic.
    }
}
