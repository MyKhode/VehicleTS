using Unity.Netcode;
using UnityEngine;

public class NetcodeSceneManagerDemo : NetworkBehaviour
{

    [SerializeField]
    private int selectedCarIndex = 0;

    [SerializeField]
    private Transform SpawnPoint;

    private void Start()
    {

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

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!IsServer)
        {
            return;
        }

        var newVehicle = Instantiate(RCC_NetcodeDemoVehicles.Instance.vehicles[selectedCarIndex], SpawnPoint.position, SpawnPoint.rotation);
        RCC.RegisterPlayerVehicle(newVehicle);
        RCC.SetControl(newVehicle, true);

        var networkObject = newVehicle.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("Player prefab is missing the NetworkObject component!");
            Destroy(newVehicle);
            return;
        }

        if (RCC_SceneManager.Instance.activePlayerCamera)
        {
            RCC_SceneManager.Instance.activePlayerCamera.SetTarget(newVehicle);
        }

        networkObject.SpawnAsPlayerObject(clientId, true);

        Debug.Log($"Player spawned for client {clientId}");
    }
}

