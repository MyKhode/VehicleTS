//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple example manager for how the car selection scene works. 
/// </summary>
public class RCC_NetcodeCarSelectionExample : MonoBehaviour {

    private List<RCC_CarControllerV3> _spawnedVehicles = new List<RCC_CarControllerV3>();       // Our spawned vehicle list. No need to instantiate same vehicles over and over again. 

    public Transform spawnPosition;     // Spawn transform.
    public int selectedIndex = 0;           // Selected vehicle index. Next and previous buttons are affecting this value.

    public GameObject RCCCanvas;
    public RCC_Camera RCCCamera;        // Enabling / disabling camera selection script on RCC Camera if choosen.

    public string nextScene;        //  Name of the target scene when we select the vehicle.
    public NetcodeSceneManagerDemo _netcodeSelect;

    public VehicleData[] vehicleData;
    void Awake() {
        LoadVehicleData();
    }

    private void Start() {

           CreateVehicles();

        // Add this check to avoid out-of-range access
        if (_spawnedVehicles.Count == 0) {
            Debug.LogError("No vehicles spawned. Please ensure that vehicle data is correctly set.");
            return; // Prevent further execution if no vehicles are available
        }

        // Clamp selectedIndex to be within the bounds of the list
        selectedIndex = Mathf.Clamp(selectedIndex, 0, _spawnedVehicles.Count - 1);
        SpawnVehicle(); // Only call this if there are vehicles

    }
    private void LoadVehicleData() {
        // Load all VehicleData assets from the Resources folder
        vehicleData = Resources.LoadAll<VehicleData>("");

        // Check if vehicleData is populated
        if (vehicleData.Length > 0)
        {
            Debug.Log("VehicleData assets successfully loaded.");
        }
        else
        {
            Debug.LogWarning("No VehicleData assets found in Resources.");
        }
    }

    /// <summary>
    /// Creating all vehicles at once.
    /// </summary>
    private void CreateVehicles()
    {
        for (int i = 0; i < vehicleData.Length; i++)
        {
            // Check if the vehicle is owned
            if (vehicleData[i].IsOwned)
            {
                // Adjust the vehicle ID by subtracting 1 since arrays are 0-based, but your IDs start from 1.
                int vehicleIndex = vehicleData[i].ItemID - 1;

                // Ensure that the vehicleIndex is within the bounds of the array to avoid errors
                if (vehicleIndex >= 0 && vehicleIndex < RCC_LocalDemoVehicles.Instance.vehicles.Length)
                {
                    // Spawning the vehicle with no controllable, no player, and engine off.
                    RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC(
                        RCC_LocalDemoVehicles.Instance.vehicles[vehicleIndex], // Use the adjusted vehicle ID
                        spawnPosition.position,
                        spawnPosition.rotation,
                        false, // controllable
                        false, // player
                        false  // engine
                    );

                    // Disabling spawned vehicle.
                    spawnedVehicle.gameObject.SetActive(false);

                    // Adding and storing it in _spawnedVehicles list.
                    _spawnedVehicles.Add(spawnedVehicle);
                }
                else
                {
                    Debug.LogWarning("Vehicle ID out of bounds for vehicle: " + vehicleData[i].ItemName);
                }
            }
        }

        // Optionally, you can call SpawnVehicle here if you want to spawn a default vehicle
        SpawnVehicle(); 

        // If RCC Camera is chosen, it will enable RCC_CameraCarSelection script. 
        if (RCCCamera)
        {
            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;
        }

        // if (RCCCanvas)
        //     RCCCanvas.SetActive(false);
    }


    /// <summary>
    /// Increasing selected index, disabling all other vehicles, enabling current selected vehicle.
    /// </summary>
    public void NextVehicle() {

        selectedIndex++;

        // If index exceeds maximum, return to 0.
        if (selectedIndex > _spawnedVehicles.Count - 1)
            selectedIndex = 0;

        SpawnVehicle();

    }

    /// <summary>
    /// Decreasing selected index, disabling all other vehicles, enabling current selected vehicle.
    /// </summary>
    public void PreviousVehicle() {

        selectedIndex--;

        // If index is below 0, return to maximum.
        if (selectedIndex < 0)
            selectedIndex = _spawnedVehicles.Count - 1;

        SpawnVehicle();

    }

    /// <summary>
    /// Spawns the current selected vehicle.
    /// </summary>
    public void SpawnVehicle() {

        // Disabling all vehicles.
        for (int i = 0; i < _spawnedVehicles.Count; i++)
            _spawnedVehicles[i].gameObject.SetActive(false);

        // And enabling only selected vehicle.
        _spawnedVehicles[selectedIndex].gameObject.SetActive(true);

        RCC_SceneManager.Instance.activePlayerVehicle = _spawnedVehicles[selectedIndex];

    }

    /// <summary>
    /// Registering the selected vehicle for multiplayer, handled by netcode.
    /// </summary>
    public void SelectVehicle() {

        // First, deactivate or destroy the current netcode vehicle
        if (RCC_SceneManager.Instance.activePlayerVehicle != null) {
            DeSelectVehicle();  // Clean up before selecting a new vehicle
        }

        // Save the selected vehicle for later use (e.g., in another scene or session)
        PlayerPrefs.SetInt("SelectedRCCVehicle", selectedIndex);

        // Use NetcodeSceneManagerDemo to handle vehicle spawning
        _netcodeSelect.SelectVehicle(selectedIndex);

        // Enable UI canvas if necessary (optional)
        if (RCCCanvas)
            RCCCanvas.SetActive(true);

        // Load the next scene if necessary
        if (!string.IsNullOrEmpty(nextScene))
            OpenScene();
    }

    /// <summary>
    /// Deactivates selected vehicle and returns to the car selection.
    /// </summary>
    public void DeSelectVehicle() {

        if (_spawnedVehicles[selectedIndex] != null) {
            // De-registers the vehicle from the scene.
            RCC.DeRegisterPlayerVehicle();

            // If it's a netcode vehicle, ensure it's properly destroyed
            Destroy(_spawnedVehicles[selectedIndex].gameObject);

            // If RCC Camera is selected, enable the orbiting camera.
            if (RCCCamera && RCCCamera.GetComponent<RCC_CameraCarSelection>()) {
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;
                RCCCamera.ChangeCamera(RCC_Camera.CameraMode.TPS);
            }

            if (RCCCanvas)
                RCCCanvas.SetActive(false);
        }
    }


    /// <summary>
    /// Loads the target scene.
    /// </summary>
    public void OpenScene() {

        //	Loads next scene.
        SceneManager.LoadScene(nextScene);

    }

}
