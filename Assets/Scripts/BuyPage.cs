using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToastMe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuyPage : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;
    public GameObject itemDetails;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TMP_InputField itemInput;
    public TextMeshProUGUI itemPrice;
    private int itemID;

    void Start()
    {
        itemDetails.SetActive(false);
        itemID = -1;
    }

    public void ChooseItem(int id)
    {
        itemDetails.SetActive(true);
        itemImage.sprite = itemsToPickup[id].image;
        itemName.text = itemsToPickup[id].name;
        itemInput.text = "1";
        itemPrice.text = "$" + itemsToPickup[id].price.ToString();
        itemID = id;
    }

    public void Minus()
    {
        int amount = int.Parse(itemInput.text);
        if (amount > 1)
        {
            amount--;
            itemInput.text = amount.ToString();
        }
        CalculatePrice();
    }

    public void Plus()
    {
        int amount = int.Parse(itemInput.text);
        amount++;
        itemInput.text = amount.ToString();
        CalculatePrice();
    }

    public void Buy()
    {
        if (itemID == -1) return;
        int money = PlayerController.Instance.money;
        int amount = int.Parse(itemInput.text);
        int totalPrice = itemsToPickup[itemID].price * amount;
        if (money < totalPrice)
        {
            ToastMessage.Instance.Show("Not enough money!");
            return;
        }
        int cnt = 0;
        for (int i = 0; i < amount; i++)
        {
            bool flag = inventoryManager.AddItem(itemsToPickup[itemID]);
            if (!flag)
            {
                UpdateMoney(cnt);
                if (cnt > 0)
                {
                    ToastItem.Instance.Show(itemsToPickup[itemID], cnt);
                    ToastMoney.Instance.Show("- " + itemsToPickup[itemID].price * cnt);
                }

                else
                    ToastMessage.Instance.Show("Inventory is full!");
                return;
            }
            cnt++;
        }
        UpdateMoney(amount);
        ToastItem.Instance.Show(itemsToPickup[itemID], amount);
        ToastMoney.Instance.Show("- " + totalPrice);
    }

    private void UpdateMoney(int cnt)
    {
        PlayerController.Instance.money -= itemsToPickup[itemID].price * cnt;
        PlayerController.Instance.moneyText.text = PlayerController.Instance.money.ToString();
    }

    public void CalculatePrice()
    {
        if (itemID == -1) return;
        int amount = int.Parse(itemInput.text);
        itemPrice.text = "$" + (itemsToPickup[itemID].price * amount).ToString();
    }
}
