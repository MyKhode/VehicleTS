using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RenderVehicleInfoList : MonoBehaviour
{
    public GameObject[] VehicleElement; // Array of vehicle elements
    public List<VehicleData> VehicleDataList;  // List of VehicleData to populate

    void Start()
    {
        // Check if VehicleDataList and VehicleElement arrays are properly initialized
        if (VehicleDataList == null || VehicleElement == null)
        {
            Debug.LogError("VehicleDataList or VehicleElement array is not initialized.");
            return;
        }

        // Iterate through each VehicleElement
        for (int i = 0; i < VehicleElement.Length; i++)
        {
            GameObject vehicle = VehicleElement[i];
            
            if (vehicle != null)
            {
                // Find the UI components within the vehicle GameObject
                Transform itemID = vehicle.transform.Find("Info/ItemID");
                Transform itemName = vehicle.transform.Find("Info/ItemName");
                Transform price = vehicle.transform.Find("Info/Price");
                Transform thumbnail = vehicle.transform.Find("Mask/Thumbnail");

                // Check if the corresponding VehicleData exists
                if (i < VehicleDataList.Count)
                {
                    VehicleData data = VehicleDataList[i];

                    // Set the UI elements with data from VehicleData
                    if (itemID != null)
                    {
                        itemID.GetComponent<TextMeshProUGUI>().text = data.ItemID.ToString();
                        Debug.Log("Set ItemID to: " + data.ItemID);
                    }
                    else
                    {
                        Debug.LogWarning("ItemID not found in " + vehicle.name);
                    }

                    if (itemName != null)
                    {
                        itemName.GetComponent<TextMeshProUGUI>().text = data.ItemName;
                        Debug.Log("Set ItemName to: " + data.ItemName);
                    }
                    else
                    {
                        Debug.LogWarning("ItemName not found in " + vehicle.name);
                    }

                    if (price != null)
                    {
                        price.GetComponent<TextMeshProUGUI>().text = data.Price.ToString("C");
                        Debug.Log("Set Price to: " + data.Price);
                    }
                    else
                    {
                        Debug.LogWarning("Price not found in " + vehicle.name);
                    }

                    if (thumbnail != null)
                    {
                        // Convert Texture2D to Sprite
                        Texture2D texture = data.thumbnail;
                        if (texture != null)
                        {
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            thumbnail.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                            Debug.Log("Set Thumbnail for: " + vehicle.name);
                        }
                        else
                        {
                            Debug.LogWarning("Texture2D is null for " + vehicle.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Thumbnail not found in " + vehicle.name);
                    }
                }
                else
                {
                    Debug.LogWarning("No VehicleData available for index " + i);
                }
            }
            else
            {
                Debug.LogWarning("VehicleElement at index " + i + " is null.");
            }
        }
    }
}
