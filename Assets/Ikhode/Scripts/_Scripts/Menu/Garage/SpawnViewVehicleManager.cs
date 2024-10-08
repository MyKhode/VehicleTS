using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EasyTransition;

public class SpawnViewVehicleManager : MonoBehaviour
{
    public GameStatsSO gameStats;
    public Button BackButton;
    public DemoLoadScene _demoLoadScene;
    public VehicleData[] vehicleDataList; // List of VehicleData assets
    public TMP_Text manufacturerText; // TextMeshPro component for displaying vehicle manufacturer
    public TMP_Text modelNameText; // TextMeshPro component for displaying vehicle model name
    public TMP_Text yearText; // TextMeshPro component for displaying vehicle year
    public TMP_Text countryText; // TextMeshPro component for displaying vehicle country
    public TMP_Text speedText; // TextMeshPro component for displaying vehicle speed
    public TMP_Text PriceText; // TextMeshPro component for displaying vehicle speed
    public TMP_Text handlingText; // TextMeshPro component for displaying vehicle handling
    public TMP_Text accelerationText; // TextMeshPro component for displaying vehicle acceleration
    public TMP_Text launchTimeText; // TextMeshPro component for displaying vehicle launch time
    public TMP_Text brakingText; // TextMeshPro component for displaying vehicle braking
    public RawImage thumbnailVehicleImage; // RawImage component for displaying vehicle thumbnail

    void Awake()
    {
        // Automatically find and assign all VehicleData assets
        FindAndAssignAllVehicleData();

        // Display specific vehicle data on load
        DisplayVehicleData();
        BackButton.onClick.AddListener(LoadScene);
    }
    void LoadScene()
    {
        _demoLoadScene.LoadScene(gameStats.previousState.ToString());
    }

    public void FindAndAssignAllVehicleData()
    {
        // Load all VehicleData assets from the Resources folder
        vehicleDataList = Resources.LoadAll<VehicleData>("");

        // Check if vehicleDataList is populated
        if (vehicleDataList.Length > 0)
        {
            Debug.Log("VehicleData assets successfully loaded.");
        }
        else
        {
            Debug.LogWarning("No VehicleData assets found in Resources.");
        }
    }

    public void DisplayVehicleData()
    {
        foreach (var vehicleData in vehicleDataList)
        {
            if (vehicleData.IsViewSelected) // Only display the selected vehicle
            {
                UpdateVehicleDisplay(vehicleData);
                break; // Exit after displaying the selected vehicle data
            }
        }
    }

    private void UpdateVehicleDisplay(VehicleData vehicleData)
    {
        // Update the UI with the vehicle specifications
        manufacturerText.text = vehicleData.Manufacturer;
        modelNameText.text = vehicleData.ModelName;
        yearText.text = vehicleData.Year;
        countryText.text = vehicleData.Country;
        PriceText.text = vehicleData.Price.ToString() + ".00$";

        speedText.text = vehicleData.Speed.ToString() + " km/h";
        handlingText.text = vehicleData.Handling.ToString();
        accelerationText.text = vehicleData.Acceleration.ToString();
        launchTimeText.text = vehicleData.LunchTime.ToString() + " sec";
        brakingText.text = vehicleData.Braking.ToString();

        thumbnailVehicleImage.texture = vehicleData.thumbnail;
    }
}
