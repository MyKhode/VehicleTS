using Unity.Netcode;
using UnityEngine;

public class NetcodeSceneManagerDemo : NetworkBehaviour
{
    [SerializeField]
    private Transform SpawnPoint;

    private int selectedCarIndex = 0;  // Default value, client will set their own

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        else
        {
            Debug.LogError("NetworkManager Singleton is null! Ensure NetworkManager is present in the scene.");
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer)
        {
            // The server doesn't choose the vehicle, it just waits for the client to send the vehicle selection
            Debug.Log($"Client {clientId} connected, waiting for vehicle selection...");
        }
    }

    /// <summary>
    /// This is called by the client to send their selected vehicle index to the server.
    /// </summary>
    [ServerRpc (RequireOwnership = false)]
    public void SubmitVehicleSelectionServerRpc(int selectedVehicleIndex, ulong clientId)
    {
        SpawnPlayer(selectedVehicleIndex, clientId);
    }

    /// <summary>
    /// This spawns the player with the selected vehicle.
    /// </summary>
    private void SpawnPlayer(int selectedVehicleIndex, ulong clientId)
    {
        if (!IsServer) return;



        // Instantiate the selected vehicle for the client
        var selectedVehicle = RCC_NetcodeDemoVehicles.Instance.vehicles[selectedVehicleIndex];
        if (selectedVehicle == null)
        {
            Debug.LogError("Selected vehicle prefab is null!");
            return;
        }

        var newVehicle = Instantiate(selectedVehicle, SpawnPoint.position, SpawnPoint.rotation);

    
        var networkObject = newVehicle.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("Player vehicle prefab is missing the NetworkObject component!");
            Destroy(newVehicle);
            return;
        }

        // Spawn the vehicle as the player object for the correct client
        networkObject.SpawnAsPlayerObject(clientId, true);

    }



    public void SelectVehicle(int vehicleIndex)
    {
        selectedCarIndex = vehicleIndex;

        // Send the selection to the server, which will handle the actual vehicle spawning
        SubmitVehicleSelectionServerRpc(selectedCarIndex, NetworkManager.Singleton.LocalClientId);
    }

}
