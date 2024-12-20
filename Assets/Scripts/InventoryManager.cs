using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Serializable]
    private class InventoryItemData
    {
        public string itemName;
        public int count;
        public int durability;
        public int slotIndex;
    }

    public List<Item> allItems = new List<Item>(); // Assuming you have a list of all items

    private Item FindItemByName(string itemName)
    {
        // Implement the logic to find and return the item by its name
        // This is a placeholder implementation
        foreach (Item item in allItems)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }

    [Serializable]
    private class InventoryData
    {
        public List<InventoryItemData> items = new List<InventoryItemData>();
    }

    public int maxStackedItems = 64;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public Button inventoryButton;
    public GameObject inventoryUI;
    private bool isInventoryOpen = false;
    public FishingController fishingController;
    public Canvas marketCanvas;

    int selectedSlot = -1;
    private string saveFilePath;


    private void Start()
    {
        ChangeSelectedSlot(0);
        saveFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
        LoadInventory();
    }

    private void OnApplicationQuit()
    {
        SaveInventory();
    }

    private void Update()
    {
        if (marketCanvas.isActiveAndEnabled) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSelectedSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSelectedSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSelectedSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSelectedSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeSelectedSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeSelectedSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChangeSelectedSlot(6);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
    }

    public void ChangeSelectedSlot(int newValue)
    {
        bool isFishing = fishingController.getIsFishing();

        if (isFishing) return;

        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item)
    {
        // Kiểm tra inventory đầy
        if (IsInventoryFull(item))
        {
            Debug.Log("Inventory is full! Cannot add item: " + item.name);
            return false;
        }

        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)
            {

                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    public bool IsInventoryFull(Item itemToAdd)
    {
        // Kiểm tra các slot hiện tại
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            // Nếu slot trống => inventory chưa đầy
            if (itemInSlot == null)
            {
                return false;
            }

            // Nếu item trong slot có thể stack và chưa đạt max stack => inventory chưa đầy
            if (itemInSlot.item == itemToAdd &&
                itemInSlot.item.stackable &&
                itemInSlot.count < maxStackedItems)
            {
                return false;
            }
        }

        // Nếu không tìm thấy slot nào khả dụng
        return true;
    }


    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    public InventoryItem GetSelectedInventoryItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        return itemInSlot;
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }

        return null;
    }

    public void RemoveItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
        {
            Debug.Log("Item to remove is null.");
            return;
        }

        // Xóa item khỏi danh sách các item trong inventory
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == inventoryItem)
            {
                if (itemInSlot.count <= 1)
                {
                    itemInSlot.count = 0;
                    Destroy(itemInSlot.gameObject); // Xóa gameObject liên quan đến item
                }
                else
                {
                    itemInSlot.count--;
                    itemInSlot.RefreshCount();
                }
                return;
            }
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            InventoryItem itemInSlot = FindInventoryItem(item);
            if (itemInSlot != null)
                RemoveItem(itemInSlot);
        }
    }

    public InventoryItem FindInventoryItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item)
            {
                return itemInSlot;
            }
        }
        return null;
    }

    public int GetItemCount(Item item)
    {
        int count = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item)
            {
                count += itemInSlot.count;
            }
        }
        return count;
    }

    public void SaveInventory()
    {
        InventoryData inventoryData = new InventoryData();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot != null)
            {
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null)
                {
                    InventoryItemData itemData = new InventoryItemData
                    {
                        itemName = itemInSlot.item.name,
                        count = itemInSlot.count,
                        durability = itemInSlot.durability,
                        slotIndex = i // Lưu vị trí của item
                    };
                    inventoryData.items.Add(itemData);
                }
            }
        }

        string json = JsonUtility.ToJson(inventoryData, true);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

            foreach (InventoryItemData itemData in inventoryData.items)
            {
                Item item = FindItemByName(itemData.itemName);
                if (item != null)
                {
                    InventorySlot slot = inventorySlots[itemData.slotIndex];
                    if (slot != null)
                    {
                        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
                        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
                        inventoryItem.InitialiseItem(item);
                        inventoryItem.count = itemData.count;
                        inventoryItem.durability = itemData.durability;
                        inventoryItem.RefreshCount();
                        inventoryItem.RefreshDurability();
                    }
                }
            }
        }
    }
}
