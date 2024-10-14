using Fusion;
using Fusion.Sockets;


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum FusionConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected
}

public class GameFusionStarter : MonoBehaviour, INetworkRunnerCallbacks
{
    public GameObject playerPrefab;          // Prefab for player spawning
    public NetworkRunner runnerPrefab;       // NetworkRunner prefab to instantiate
    public TMP_InputField roomNameInput;     // Input field for room name
    public TMP_InputField playerNameInput;   // Input field for player name
    public TextMeshProUGUI statusText;       // UI Text to show connection status
    public Button hostButton;                // Button to host a game
    public Button joinButton;                // Button to join a game
    public Transform spawnPoint;             // Spawn point for players

    private NetworkRunner _runnerInstance;   // Instance of NetworkRunner

    private void Start()
    {
        // Add listeners for the Host and Join buttons
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);
    }

    // Method to host a game and spawn the player
    public async void HostGame()
    {
        if (_runnerInstance != null)
        {
            Debug.LogWarning("Game is already running!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerNameInput.text);

        // Instantiate and configure the NetworkRunner
        _runnerInstance = Instantiate(runnerPrefab);
        _runnerInstance.ProvideInput = true;
        _runnerInstance.AddCallbacks(this);

        // Set up StartGame arguments for hosting
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = roomNameInput.text,
            PlayerCount = 8
        };

        statusText.text = "Hosting...";

        // Start the network game session
        var result = await _runnerInstance.StartGame(startArgs);
        if (result.Ok)
        {
            statusText.text = "Hosting room successfully!";
            SpawnPlayer();
        }
        else
        {
            statusText.text = $"Failed to host: {result.ShutdownReason}";
        }
    }

    // Method to join a game
    public async void JoinGame()
    {
        if (_runnerInstance != null)
        {
            Debug.LogWarning("Game is already running!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerNameInput.text);

        // Instantiate and configure the NetworkRunner
        _runnerInstance = Instantiate(runnerPrefab);
        _runnerInstance.ProvideInput = true;
        _runnerInstance.AddCallbacks(this);

        // Set up StartGame arguments for joining
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = roomNameInput.text,
            PlayerCount = 8
        };

        statusText.text = "Joining...";

        // Join the network game session
        var result = await _runnerInstance.StartGame(startArgs);
        if (result.Ok)
        {
            statusText.text = "Joined room successfully!";
        }
        else
        {
            statusText.text = $"Failed to join: {result.ShutdownReason}";
        }
    }

    // Method to spawn the player at the predefined spawn point
    public void SpawnPlayer()
    {
        if (_runnerInstance == null)
        {
            Debug.LogWarning("NetworkRunner instance is null!");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // Spawn player at the specified position
        _runnerInstance.Spawn(playerPrefab, spawnPosition, spawnRotation, _runnerInstance.LocalPlayer);
        Debug.Log($"Player spawned at position {spawnPosition}.");
    }

    // Set connection status UI
    private void SetFusionConnectionStatus(FusionConnectionStatus status)
    {
        statusText.text = status.ToString();
    }

    // Leave session logic
    private void LeaveSession()
    {
        if (_runnerInstance != null)
        {
            Destroy(_runnerInstance.gameObject);
            _runnerInstance = null;
        }
    }

    #region INetworkRunnerCallbacks Implementation

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        SetFusionConnectionStatus(FusionConnectionStatus.Connected);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("Disconnected from server");
        LeaveSession();
        SetFusionConnectionStatus(FusionConnectionStatus.Disconnected);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        if (runner.TryGetSceneInfo(out var scene) && scene.SceneCount > 0)
        {
            Debug.LogWarning($"Connection refused from {request.RemoteAddress}");
            request.Refuse();
        }
        else
        {
            request.Accept();
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"Connection failed: {reason}");
        LeaveSession();
        SetFusionConnectionStatus(FusionConnectionStatus.Failed);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined!");
        SetFusionConnectionStatus(FusionConnectionStatus.Connected);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} left.");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Shutdown reason: {shutdownReason}");
        LeaveSession();
        SetFusionConnectionStatus(FusionConnectionStatus.Disconnected);
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    #endregion
}
