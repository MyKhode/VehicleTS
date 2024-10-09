using com.example;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using EasyTransition;
using AHUI;
using System.Threading.Tasks;

public class PurchessManager : MonoBehaviour
{
    public VehicleData[] vehicleData;                // Array of all purchasable items
    public GameObject vehiclePrefab;                 // Prefab for vehicle GameObjects
    public Transform parentTransform;                // Parent Transform to spawn vehicles

    public Color ownedColor = Color.green;          // Color to indicate owned items
    public Color releasedColor = Color.blue;        // Color to indicate released items
    public Color lockedColor = Color.red;           // Color to indicate locked items

    public NotificationManager notificationManager;    // Reference to your AHUI NotificationManager
    public Transform parentNotificationTransform;      // Parent Transform to spawn notifications

    public GameObject notificationPrefab;              // Prefab for notification UI element
    public float notificationDuration = 3f;            // Duration to show notification (can be assigned in Inspector)

    private List<VehicleElement> vehicleElements = new List<VehicleElement>(); // List of UI elements representing each vehicle
    private decimal playerMoney = 1000m;               // Use decimal for player money
    private SupabaseModelManager supabaseModelManager;

    private UserDisplayInfoDemo userDisplayInfoDemo; 

    private void Awake()
    {
        LoadVehicleData();
        supabaseModelManager = FindObjectOfType<SupabaseModelManager>(); // Find SupabaseModelManager in the scene

        userDisplayInfoDemo = FindObjectOfType<UserDisplayInfoDemo>();
        if (userDisplayInfoDemo == null)
        {
            Debug.LogError("UserDisplayInfoDemo is not found in the scene.");
        }
    } 

    private async void Start()
    {
        await LoadPlayerData();  // Load player data on start
        InstantiateVehicles();
        UpdateItemUI();
    }

    // Load player's cash from Supabase
    private async Task LoadPlayerData()
    {
        playerMoney = await supabaseModelManager.GetPlayerCash(); // Remove playerUID
    }

    private void LoadVehicleData()
    {
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

    private void InstantiateVehicles()
    {
        if (parentTransform == null)
        {
            Debug.LogError("Parent Transform is not assigned.");
            return;
        }

        foreach (var item in vehicleData)
        {
            if (item == null) continue;

            // Instantiate a new vehicle GameObject
            GameObject vehicleGO = Instantiate(vehiclePrefab, parentTransform);

            // Add the VehicleElement component to the vehicle GameObject
            VehicleElement vehicleElement = vehicleGO.AddComponent<VehicleElement>();
            vehicleElement.vehicle = vehicleGO; // Set the vehicle GameObject
            vehicleElement.Initialize();
            vehicleElements.Add(vehicleElement);
        }
    }

    private async void BuyItem(int itemID)
    {
        var item = FindItemByID(itemID);
        if (item == null || item.IsOwned || !item.IsReleased)
            return;

        // Fetch updated player money before purchase
        playerMoney = await supabaseModelManager.GetPlayerCash(); // Remove playerUID
        Debug.Log($"Player money before buying {item.ItemName}: {playerMoney}");

        if (playerMoney >= (decimal)item.Price)
        {
            playerMoney -= (decimal)item.Price;  // Deduct price
            item.SetIsOwned(true);               // Mark item as owned

            // Sync with Supabase
            await supabaseModelManager.UpdatePlayerCash(playerMoney); // Remove playerUID
            UpdateItemUI();
            Debug.Log($"Player money after buying {item.ItemName}: {playerMoney}"); // Debug money
            
            // Refresh user cash UI
            if(userDisplayInfoDemo != null)
            {
                userDisplayInfoDemo.RefreshUI();
            }

            ShowNotification("Item Purchased", $"{item.ItemName} bought. Price = ${item.Price}.00");
        }
        else
        {
            ShowNotification("Insufficient Funds", "Not enough money to purchase this item.");
        }
    }


    public async void SellItem(int itemID)
    {
        var item = FindItemByID(itemID);
        if (item == null || !item.IsOwned)
            return;

        // Fetch updated player money before selling
        playerMoney = await supabaseModelManager.GetPlayerCash(); // Remove playerUID

        // Add 30% of item price to playerMoney
        decimal saleAmount = (decimal)item.Price * 0.3m;
        playerMoney += saleAmount;
        item.SetIsOwned(false); // Mark item as not owned

        // Sync with Supabase
        await supabaseModelManager.UpdatePlayerCash(playerMoney); // Remove playerUID
        UpdateItemUI();
        Debug.Log($"Player money after selling {item.ItemName}: {playerMoney}"); // Debug money
        
        // Refresh user cash UI
        if(userDisplayInfoDemo != null)
        {
            userDisplayInfoDemo.RefreshUI();
        }

        ShowNotification("Item Sold", $"{item.ItemName} sold for ${saleAmount}.00");
    }


    private PurchasableItem FindItemByID(int itemID)
    {
        foreach (var item in vehicleData)
        {
            if (item.ItemID == itemID)
                return item;
        }
        return null;
    }

    private void UpdateItemUI()
    {
        for (int i = 0; i < vehicleData.Length; i++)
        {
            var item = vehicleData[i];
            if (i >= vehicleElements.Count) continue; // Prevent out of bounds

            var vehicleElement = vehicleElements[i];
            vehicleElement.UpdateVisuals(item, ownedColor, releasedColor, lockedColor);

            // Set up the onClick events for the buy button
            vehicleElement.buyButton.onClick.RemoveAllListeners();
            vehicleElement.buyButton.onClick.AddListener(() => BuyItem(item.ItemID));

            // Set up the onClick events for the sell button
            vehicleElement.sellButton.onClick.RemoveAllListeners();
            vehicleElement.sellButton.onClick.AddListener(() => SellItem(item.ItemID));

            // Set up the onClick events for the view button
            vehicleElement.viewButton.onClick.RemoveAllListeners();
            vehicleElement.viewButton.onClick.AddListener(() => OnViewButtonClick(item.ItemID));
        }
    }

    private void OnViewButtonClick(int itemID)
    {
        var item = FindItemByID(itemID);
        if (item == null) return;

        // Set IsViewSelected to true for the selected item and false for others
        foreach (var vehicle in vehicleData)
        {
            vehicle.IsViewSelected = vehicle.ItemID == itemID;
        }

        // Load the scene for viewing the selected item
        LoadViewScene();
    }

    private void ShowNotification(string title, string message)
    {
        // Show notification using AHUI.NotificationManager
        notificationManager.ShowNotification(
            title,
            message,
            parentNotificationTransform, // Assign the parent transform for the notification
            notificationPrefab,           // Assign the notification prefab
            notificationDuration          // Duration for the notification
        );
    }

    private void LoadViewScene()
    {
        // Load the scene using a specific name or your scene management logic
        UnityEngine.SceneManagement.SceneManager.LoadScene("ViewScene");
    }
}

