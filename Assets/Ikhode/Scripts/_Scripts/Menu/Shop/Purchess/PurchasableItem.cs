using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PurchasableItem : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public float Price;
    public bool IsOwned;
    public bool IsReleased;
    public Texture2D thumbnail;

    public UnityEvent OnIsOwnedDataChange = new UnityEvent();

    public void SetIsOwned(bool isOwned)
    {
        if (IsOwned != isOwned)
        {
            IsOwned = isOwned;
            Debug.Log($"Item ID {ItemID} ownership changed to {isOwned}");
            OnIsOwnedDataChange.Invoke();
        }
    }
}
