using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Prefab to spawn as the player's avatar

    // Start is called before the first frame update
    private void Start()
    {
        // Ensure this object persists between scene loads
        DontDestroyOnLoad(gameObject);

        // Subscribe to the client connection event
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        else
        {
            Debug.LogError("NetworkManager Singleton is null! Ensure NetworkManager is present in the scene.");
        }
    }

    // Unsubscribe from events to avoid memory leaks when the object is destroyed
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }
    }

    /// <summary>
    /// Callback method triggered when a client connects to the server.
    /// </summary>
    /// <param name="clientId">The unique client ID of the connected client</param>
    private void OnClientConnectedCallback(ulong clientId)
    {
        // Spawn the player for the connected client
        SpawnPlayer(clientId);
    }

    /// <summary>
    /// Spawns the player object for the connected client.
    /// </summary>
    /// <param name="clientId">The unique client ID to assign the spawned player to</param>
    private void SpawnPlayer(ulong clientId)
    {
        if (!IsServer)
        {
            return; // Ensure that only the server can spawn players
        }

        // Ensure the playerPrefab is assigned
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the inspector!");
            return;
        }

        // Instantiate the player object from the prefab
        GameObject player = Instantiate(playerPrefab);

        // Ensure the instantiated player has a NetworkObject component
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("Player prefab is missing the NetworkObject component!");
            Destroy(player); // Cleanup the instantiated object if no NetworkObject component is found
            return;
        }

        // Spawn the player object for the connected client
        networkObject.SpawnAsPlayerObject(clientId, true);

        Debug.Log($"Player spawned for client {clientId}");
    }
}
