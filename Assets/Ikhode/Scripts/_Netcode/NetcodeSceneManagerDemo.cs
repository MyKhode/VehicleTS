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
        RCC.RegisterPlayerVehicle(newVehicle);
        RCC.SetControl(newVehicle, true);

        var networkObject = newVehicle.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("Player vehicle prefab is missing the NetworkObject component!");
            Destroy(newVehicle);
            return;
        }

        // Spawn the vehicle as the player object
        networkObject.SpawnAsPlayerObject(clientId, true);

        // Notify the client to set their camera to the new vehicle
        UpdateClientCameraClientRpc(clientId, networkObject.NetworkObjectId);
    }

    /// <summary>
    /// ClientRpc to update the camera on the client side to follow the newly spawned vehicle.
    /// </summary>
    [ClientRpc]
    private void UpdateClientCameraClientRpc(ulong clientId, ulong vehicleNetworkObjectId)
    {
        if (IsOwner && NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Retrieve the vehicle game object
            var vehicle = NetworkManager.SpawnManager.SpawnedObjects[vehicleNetworkObjectId].gameObject;

            // Get the RCC_CarControllerV3 component from the vehicle
            var carController = vehicle.GetComponent<RCC_CarControllerV3>();

            if (RCC_SceneManager.Instance.activePlayerCamera != null && carController != null)
            {
                // Set the target to the car controller, not the game object
                RCC_SceneManager.Instance.activePlayerCamera.SetTarget(carController);
            }
        }
    }

    public void SelectVehicle(int vehicleIndex)
    {
        selectedCarIndex = vehicleIndex;

        // Send the selection to the server
        SubmitVehicleSelectionServerRpc(selectedCarIndex, NetworkManager.Singleton.LocalClientId);
    }
}
