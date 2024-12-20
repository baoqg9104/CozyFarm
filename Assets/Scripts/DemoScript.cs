using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;
    
    public void PickupItem(int id) {
        bool result = inventoryManager.AddItem(itemsToPickup[id]);
    }

    public void GetSelectedItem() {
        Item receivedItem = inventoryManager.GetSelectedItem(false);
    }

    public void UseSelectedItem() {
        Item receivedItem = inventoryManager.GetSelectedItem(true);
    }
}
