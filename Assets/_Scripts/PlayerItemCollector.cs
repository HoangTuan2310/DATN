using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collision.GetComponent<Collider2D>().enabled = false;

            ItemInventory item = collision.GetComponent<ItemInventory>();
            if (item != null)
            {
                bool itemAdded = InventoryController.Instance.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    item.PickUp();
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
