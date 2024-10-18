//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the photon demo scene by providing vehicle selection, spawn functionality, and connection handling.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Network/Photon/RCC Photon Demo Custom Manager")]
public class RCC_PhotonDemoCustom : MonoBehaviourPunCallbacks {

    [Header("Connection Settings")]
    public bool reconnectIfFails = true;
    private bool connected = false;

    [Header("Vehicle Settings")]
    [SerializeField] private int selectedCarIndex = 0;
    private int selectedBehaviorIndex = 0;
    public Transform[] spawnPoints;
    public GameObject menu;
    public VehicleData[] vehicleData;

    private List<RCC_CarControllerV3> spawnedVehicles = new List<RCC_CarControllerV3>();
    [SerializeField] private int selectedIndex = 0;

    public GameObject RCCCanvas;
    public RCC_Camera RCCCamera;        // Enabling / disabling camera selection script on RCC Camera if choosen.


    private void Awake() {
        LoadVehicleData();
    }

   private void Start() {
    if (reconnectIfFails && !PhotonNetwork.IsConnectedAndReady) {
        ConnectToPhoton();
    } else if (PhotonNetwork.IsConnectedAndReady) {
        menu.SetActive(true);
        if (RCC_LocalDemoVehicles.Instance == null) {
            Debug.LogError("RCC_LocalDemoVehicles.Instance is not set. Ensure it's part of the scene or initialized.");
            return;
        }
    }
     // Call CreateVehicles after the instance is checked
    CreateVehicles();
}


    /// <summary>
    /// Loads all VehicleData assets from the Resources folder.
    /// </summary>
    private void LoadVehicleData() {
        vehicleData = Resources.LoadAll<VehicleData>("");

        if (vehicleData.Length > 0) {
            Debug.Log("VehicleData assets successfully loaded.");
        } else {
            Debug.LogWarning("No VehicleData assets found in Resources.");
        }
    }

    /// <summary>
    /// Spawns vehicles based on the loaded VehicleData assets.
    /// </summary>
  private void CreateVehicles() {
    if (RCC_LocalDemoVehicles.Instance == null) {
        Debug.LogError("RCC_LocalDemoVehicles.Instance is null. Ensure it's loaded correctly.");
        return;
    }

    if (RCC_LocalDemoVehicles.Instance.vehicles.Length == 0) {
        Debug.LogError("RCC_LocalDemoVehicles has no vehicles. Check the vehicle setup.");
        return;
    }

    foreach (var data in vehicleData) {
        if (data.IsOwned) {
            int vehicleIndex = data.ItemID - 1;

            if (vehicleIndex >= 0 && vehicleIndex < RCC_LocalDemoVehicles.Instance.vehicles.Length) {
                var spawnedVehicle = RCC.SpawnRCC(
                    RCC_LocalDemoVehicles.Instance.vehicles[vehicleIndex],
                    spawnPoints[selectedIndex].position,
                    spawnPoints[selectedIndex].rotation,
                    false, false, false
                );

                if (spawnedVehicle != null) {
                    spawnedVehicle.gameObject.SetActive(false);
                    spawnedVehicles.Add(spawnedVehicle);
                    Debug.Log($"Vehicle {data.ItemName} spawned successfully.");
                } else {
                    Debug.LogError($"Failed to spawn vehicle at index {vehicleIndex}");
                }
            } else {
                Debug.LogError($"Vehicle index {vehicleIndex} is out of bounds.");
            }
        }
    }

    if (spawnedVehicles.Count == 0) {
        Debug.LogError("No vehicles were spawned. Check vehicle data.");
    } else {
        Debug.Log("Vehicles spawned successfully.");
        SpawnVehicle();
        SetCameraAndCanvas();
    }
}



   public void NextVehicle() {
    if (spawnedVehicles == null || spawnedVehicles.Count == 0) {
        Debug.LogError("No vehicles available to select.");
        return;
    }

    // Logic to cycle through vehicles
    selectedIndex++;
    if (selectedIndex >= spawnedVehicles.Count) {
        selectedIndex = 0;
    }

    SpawnVehicle();
    SetCameraAndCanvas();
}


    public void PreviousVehicle() {
        selectedIndex = (selectedIndex - 1 + spawnedVehicles.Count) % spawnedVehicles.Count;
        SpawnVehicle();
    }

    /// <summary>
    /// Spawns the selected vehicle and disables others.
    /// </summary>
    public void SpawnVehicle() {
        foreach (var vehicle in spawnedVehicles) {
            vehicle.gameObject.SetActive(false);
        }

        var newVehicle = spawnedVehicles[selectedIndex];
        newVehicle.gameObject.SetActive(true);
        RCC_SceneManager.Instance.activePlayerVehicle = newVehicle;

        if (RCC_SceneManager.Instance.activePlayerCamera) {
            RCC_SceneManager.Instance.activePlayerCamera.SetTarget(newVehicle);
        }
    }

    /// <summary>
    /// Deselects the current vehicle and enables the orbiting camera.
    /// </summary>
    public void DeSelectVehicle() {
        if (spawnedVehicles[selectedIndex] != null) {
            RCC.DeRegisterPlayerVehicle();
            Destroy(spawnedVehicles[selectedIndex].gameObject);
            EnableCamera();
        }
    }

    private void EnableCamera() {
        if (RCCCamera && RCCCamera.GetComponent<RCC_CameraCarSelection>()) {
            RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;
            RCCCamera.ChangeCamera(RCC_Camera.CameraMode.TPS);
        }

        if (RCCCanvas) {
            RCCCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Spawns the player vehicle based on their actor number.
    /// </summary>
    /// 
    public void Spawn() {
         int actorNo = PhotonNetwork.LocalPlayer.ActorNumber;

        if (actorNo > spawnPoints.Length) {

            while (actorNo > spawnPoints.Length)
                actorNo -= spawnPoints.Length;

        }

        foreach (var vehicle in spawnedVehicles) {
            vehicle.gameObject.SetActive(false);
        }

        SelectVehicle(selectedIndex);
        RCCCanvas.SetActive(true);

        Vector3 lastKnownPos = Vector3.zero;
        Quaternion lastKnownRot = Quaternion.identity;

        RCC_CarControllerV3 newVehicle;

        if (RCC_SceneManager.Instance.activePlayerVehicle) {

            lastKnownPos = RCC_SceneManager.Instance.activePlayerVehicle.transform.position;
            lastKnownRot = RCC_SceneManager.Instance.activePlayerVehicle.transform.rotation;

        }

        if (lastKnownPos == Vector3.zero) {

            lastKnownPos = spawnPoints[actorNo - 1].position;
            lastKnownRot = spawnPoints[actorNo - 1].rotation;

        }

        lastKnownRot.x = 0f;
        lastKnownRot.z = 0f;

        newVehicle = PhotonNetwork.Instantiate("Photon Vehicles/" + RCC_PhotonDemoVehicles.Instance.vehicles[selectedCarIndex].name, lastKnownPos + (Vector3.up), lastKnownRot, 0).GetComponent<RCC_CarControllerV3>();

        RCC.RegisterPlayerVehicle(newVehicle);
        RCC.SetControl(newVehicle, true);

        if (RCC_SceneManager.Instance.activePlayerCamera)
            RCC_SceneManager.Instance.activePlayerCamera.SetTarget(newVehicle);
    }

    public void SelectVehicle(int index) {
        selectedCarIndex = index;
    }

    public void SetBehavior(int index) {
        selectedBehaviorIndex = index;
        RCC.SetBehavior(selectedBehaviorIndex);
    }

    public void SetMobileController(int index) {
        var controllerType = index switch {
            0 => RCC_Settings.MobileController.TouchScreen,
            1 => RCC_Settings.MobileController.Gyro,
            2 => RCC_Settings.MobileController.SteeringWheel,
            _ => RCC_Settings.MobileController.Joystick
        };
        RCC.SetMobileController(controllerType);
    }

    public void SetQuality(int index) {
        QualitySettings.SetQualityLevel(index);
    }

    public void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() {
        Application.Quit();
    }

    private void ConnectToPhoton() {
        Debug.Log("Connecting to server...");
        connected = true;
        RCC_InfoLabel.Instance.ShowInfo("Entering lobby");
        PhotonNetwork.NickName = $"Player_{Random.Range(0, 99999)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        if (!connected) return;

        Debug.Log("Connected to server. Entering lobby...");
        RCC_InfoLabel.Instance.ShowInfo("Entering lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        if (!connected) return;

        Debug.Log("Entered lobby. Creating/Joining room...");
        RCC_InfoLabel.Instance.ShowInfo("Creating/Joining Random Room");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        if (!connected) return;

        RoomOptions roomOptions = new RoomOptions {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 4
        };

        PhotonNetwork.JoinOrCreateRoom($"Room_{Random.Range(0, 999)}", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        if (!connected) return;

        menu?.SetActive(true);
    }

    public override void OnCreatedRoom() {
        if (!connected) return;

        menu?.SetActive(true);
    }

    private void SetCameraAndCanvas() {
        if (RCCCamera && RCCCamera.GetComponent<RCC_CameraCarSelection>()) {
            RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;
        }

        if (RCCCanvas) {
            RCCCanvas.SetActive(false);
        }
    }
}
#endif
