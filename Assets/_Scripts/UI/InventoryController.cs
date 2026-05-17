using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        RebuildItemCounts();
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();
        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                ItemInventory item = slot.currentItem.GetComponent<ItemInventory>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }
        OnInventoryChanged?.Invoke();
    }
    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    public bool AddItem(GameObject itemPrefab)
    {
        ItemInventory itemToAdd = itemPrefab.GetComponent<ItemInventory>();
        if (itemToAdd == null) return false;

        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                ItemInventory slotItem = slot.currentItem.GetComponent<ItemInventory>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack();
                    RebuildItemCounts();
                    return true;
                }
            }
        }

        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTranform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                RebuildItemCounts();
                return true;
            }
        }

        Debug.Log("Inventory is full");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTranform in inventoryPanel.transform)
        {
            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                ItemInventory item = slot.currentItem.GetComponent<ItemInventory>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTranform.GetSiblingIndex(),
                    quantity = item.quantity
                });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    ItemInventory itemComponent = item.GetComponent<ItemInventory>();
                    if (itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDispay();
                    }
                    slot.currentItem = item;
                }
            }
        }
        RebuildItemCounts();
    }

    public void RemoveItemsFromInventory(int itemID, int amountToRemove)
    {
        foreach(Transform slotTranform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            Slot slot = slotTranform.GetComponent<Slot>();
            if (slot?.currentItem?.GetComponent<ItemInventory>() is ItemInventory item && item.ID == itemID)
            {
                int removed = Mathf.Min(amountToRemove, item.quantity);
                item.RemoveFromStack(removed);
                amountToRemove -= removed;

                if(item.quantity == 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }

        RebuildItemCounts();
    }
}
