using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    void Start()
    {
        inventoryController = Object.FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collision.GetComponent<Collider2D>().enabled = false;

            ItemInventory item = collision.GetComponent<ItemInventory>();
            if (item != null)
            {
                bool itemAdded = inventoryController.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    item.PickUp();
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
