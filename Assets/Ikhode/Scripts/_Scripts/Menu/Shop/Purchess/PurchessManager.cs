using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AHUI;

public class PurchaseManager : MonoBehaviour
{
    public VehicleData[] vehicleData;
    public GameObject vehiclePrefab;
    public Transform parentTransform;

    public Color ownedColor = Color.green;
    public Color releasedColor = Color.blue;
    public Color lockedColor = Color.red;

    public NotificationManager notificationManager;
    public Transform parentNotificationTransform;
    public GameObject notificationPrefab;
    public float notificationDuration = 3f;

    private List<VehicleElement> vehicleElements = new List<VehicleElement>();
    private decimal playerMoney = 1000m;
    private SupabaseModelManager supabaseModelManager;
    private VehicleDataSyncManager vehicleDataSyncManager;

    private async void Awake()
    {
        vehicleDataSyncManager = FindObjectOfType<VehicleDataSyncManager>();
        if (vehicleDataSyncManager == null)
        {
            Debug.LogError("VehicleDataSyncManager is not found in the scene.");
            return;
        }

        // Sync and load data
        await Task.WhenAll(vehicleDataSyncManager.SyncVehicleDataWithDatabase(), LoadPlayerData());

        // Load and instantiate vehicles
        LoadVehicleData();
        InstantiateVehicles();

        // Update UI
        await UpdateItemUIAsync();
    }

    private async Task LoadPlayerData()
    {
        supabaseModelManager = FindObjectOfType<SupabaseModelManager>();
        if (supabaseModelManager != null)
        {
            playerMoney = await supabaseModelManager.GetPlayerCash();
        }
        else
        {
            Debug.LogError("SupabaseModelManager is not found in the scene.");
        }
    }

    private void LoadVehicleData()
    {
        vehicleData = Resources.LoadAll<VehicleData>("");
        Debug.Log(vehicleData.Length > 0 ? "VehicleData loaded." : "No VehicleData found.");
    }

    private void InstantiateVehicles()
    {
        if (parentTransform == null)
        {
            Debug.LogError("Parent Transform is not assigned.");
            return;
        }

        foreach (var item in vehicleData.Where(v => v != null))
        {
            var vehicleGO = Instantiate(vehiclePrefab, parentTransform);
            var vehicleElement = vehicleGO.AddComponent<VehicleElement>();
            vehicleElement.vehicle = vehicleGO;
            vehicleElement.Initialize();
            vehicleElements.Add(vehicleElement);
        }
    }

    private async Task UpdateItemUIAsync()
    {
        while (vehicleData.Length > 0)
        {
            for (int i = 0; i < vehicleData.Length; i++)
            {
                if (i >= vehicleElements.Count) continue;

                var item = vehicleData[i];
                var vehicleElement = vehicleElements[i];
                vehicleElement.UpdateVisuals(item, ownedColor, releasedColor, lockedColor);
                SetUpButtonListeners(vehicleElement, item);
            }

            await Task.Delay(1000); // Optional delay
        }
    }

    private void SetUpButtonListeners(VehicleElement vehicleElement, PurchasableItem item)
    {
        vehicleElement.buyButton.onClick.RemoveAllListeners();
        vehicleElement.buyButton.onClick.AddListener(() => HandlePurchase(item.ItemID, true));

        vehicleElement.sellButton.onClick.RemoveAllListeners();
        vehicleElement.sellButton.onClick.AddListener(() => HandlePurchase(item.ItemID, false));

        vehicleElement.viewButton.onClick.RemoveAllListeners();
        vehicleElement.viewButton.onClick.AddListener(() => OnViewButtonClick(item.ItemID));
    }

    private async void HandlePurchase(int itemID, bool isBuying)
    {
        var item = FindItemByID(itemID);
        if (item == null || (isBuying && item.IsOwned) || (!isBuying && !item.IsOwned))
            return;

        playerMoney = await supabaseModelManager.GetPlayerCash();
        if (isBuying && playerMoney >= (decimal)item.Price)
        {
            playerMoney -= (decimal)item.Price;
            item.SetIsOwned(true);
            await supabaseModelManager.AddPurchase(itemID);
        }
        else if (!isBuying)
        {
            decimal saleAmount = (decimal)item.Price * 0.3m;
            playerMoney += saleAmount;
            item.SetIsOwned(false);
            await supabaseModelManager.RemovePurchase(itemID);
        }
        else
        {
            ShowNotification("Insufficient Funds", "Not enough money.");
            return;
        }

        await supabaseModelManager.UpdatePlayerCash(playerMoney);
        Debug.Log($"Player money: {playerMoney}");
        await vehicleDataSyncManager.SyncVehicleDataWithDatabase();
        ShowNotification(isBuying ? "Item Purchased" : "Item Sold with 30% Sale", $"{item.ItemName} for {item.Price:C}");
    }

    private PurchasableItem FindItemByID(int itemID) => vehicleData.FirstOrDefault(item => item.ItemID == itemID);

    private void ShowNotification(string title, string message)
    {
        notificationManager.ShowNotification(
            title,
            message,
            parentNotificationTransform,
            notificationPrefab,
            notificationDuration
        );
    }

    private void OnViewButtonClick(int itemID)
    {
        var item = FindItemByID(itemID);
        if (item == null) return;

        foreach (var vehicle in vehicleData)
        {
            vehicle.IsViewSelected = vehicle.ItemID == itemID;
        }

        LoadViewScene();
    }

    private void LoadViewScene() => UnityEngine.SceneManagement.SceneManager.LoadScene("ViewScene");
}
