using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellPage : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToSell;
    public GameObject itemDetails;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TMP_InputField itemInput;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI availableItems;
    private int itemID;

    void Start()
    {
        itemDetails.SetActive(false);
        itemID = -1;
    }

    public void ChooseItem(int id)
    {
        itemDetails.SetActive(true);
        itemImage.sprite = itemsToSell[id].image;
        itemName.text = itemsToSell[id].name;
        itemInput.text = "1";
        itemPrice.text = "$" + itemsToSell[id].price.ToString();
        UpdateAvailable(id);
        itemID = id;
    }

    public void UpdateAvailable(int id) {
        availableItems.text = "Available: " + inventoryManager.GetItemCount(itemsToSell[id]).ToString();
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

    public void Sell()
    {
        if (itemID == -1) return;
        int amount = int.Parse(itemInput.text);
        int totalPrice = itemsToSell[itemID].price * amount;
        if (amount > inventoryManager.GetItemCount(itemsToSell[itemID]))
        {
            ToastMessage.Instance.Show("Not enough items to sell");
            return;
        }
        
        inventoryManager.RemoveItem(itemsToSell[itemID], amount);
        UpdateMoney(amount);
        ToastMoney.Instance.Show("+ " + totalPrice);
        UpdateAvailable(itemID);
    }

    private void UpdateMoney(int cnt)
    {
        PlayerController.Instance.money += itemsToSell[itemID].price * cnt;
        PlayerController.Instance.moneyText.text = PlayerController.Instance.money.ToString();
    }

    public void CalculatePrice()
    {
        if (itemID == -1) return;
        int amount = int.Parse(itemInput.text);
        itemPrice.text = "$" + (itemsToSell[itemID].price * amount).ToString();
    }

}
