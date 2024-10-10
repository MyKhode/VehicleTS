using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VehicleElement : MonoBehaviour
{
    public GameObject vehicle; // The main vehicle GameObject

    // UI components within the vehicle GameObject
    public TextMeshProUGUI itemID;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI price;
    public Image thumbnail;
    public Button buyButton;
    public Button sellButton;
    public Button viewButton;

    public void Initialize()
    {
        // Find and assign the necessary UI components within the vehicle GameObject
        itemID = vehicle.transform.Find("Info/ItemID").GetComponent<TextMeshProUGUI>();
        itemName = vehicle.transform.Find("Info/ItemName").GetComponent<TextMeshProUGUI>();
        price = vehicle.transform.Find("Info/Price").GetComponent<TextMeshProUGUI>();
        thumbnail = vehicle.transform.Find("Mask/Thumbnail").GetComponent<Image>();
        buyButton = vehicle.transform.Find("Button/Buy").GetComponent<Button>();
        sellButton = vehicle.transform.Find("Button/Sell").GetComponent<Button>();
        viewButton = vehicle.transform.Find("Button/View").GetComponent<Button>();
    }

    public void SetButtonActive(Button button, bool isActive)
    {
        if (button != null)
        {
            button.gameObject.SetActive(isActive);
        }
    }

    public void UpdateVisuals(PurchasableItem item, Color ownedColor, Color releasedColor, Color lockedColor)
    {
        // Set the UI components with data from PurchasableItem
        if (itemID != null)
            itemID.text = item.ItemID.ToString();

        if (itemName != null)
            itemName.text = item.ItemName;

        if (price != null)
            price.text = item.Price.ToString("C");

        if (thumbnail != null && item.thumbnail != null)
        {
            var sprite = Sprite.Create(item.thumbnail, new Rect(0, 0, item.thumbnail.width, item.thumbnail.height), new Vector2(0.5f, 0.5f));
            thumbnail.sprite = sprite;
        }

        if(vehicle != null)
        {
            // Set vehicle GameObject color based on item state
            vehicle.GetComponent<RawImage>().color = item.IsOwned ? ownedColor : item.IsReleased ? releasedColor : lockedColor;
        }
        // Hide the purchase and sell buttons if the vehicle ID is 1
        if (item.ItemID == 0)
        {
            SetButtonActive(buyButton, false);
            SetButtonActive(sellButton, false);
        }
        else
        {
            SetButtonActive(buyButton, !item.IsOwned && item.IsReleased);
            SetButtonActive(sellButton, item.IsOwned);
        }
    }
}
