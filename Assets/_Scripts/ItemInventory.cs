using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    public int ID;
    public string Name;
    public int quantity = 1;

    private TMP_Text quantityText;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDispay();
    }

    public void UpdateQuantityDispay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDispay();
    }
    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDispay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        ItemInventory cloneItem = clone.GetComponent<ItemInventory>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDispay();
        return clone;
    }

    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }
}

