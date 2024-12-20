using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MarketTabController : MonoBehaviour
{
    public UnityEngine.UI.Image[] tabs;
    public GameObject[] pages;
    public GameObject itemDetails;
    public UnityEngine.UI.Button actionBtn;
    public TextMeshProUGUI actionBtnText;
    public TextMeshProUGUI availableItems;
    public BuyPage buyPage;
    public SellPage sellPage;


    void Start()
    {
        OnTabClick(0);
    }

    public void OnTabClick(int index)
    {
        itemDetails.SetActive(false);
        if (index == 0)
        {
            actionBtn.onClick.RemoveAllListeners();
            actionBtn.onClick.AddListener(buyPage.Buy);
            actionBtnText.text = "Buy";
            availableItems.enabled = false;
        }
        else
        {
            actionBtn.onClick.RemoveAllListeners();
            actionBtn.onClick.AddListener(sellPage.Sell);
            actionBtnText.text = "Sell";
            availableItems.enabled = true;
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].color = Color.white;
            pages[i].SetActive(false);
        }

        Color tabColor;
        ColorUtility.TryParseHtmlString("#E6CBA4", out tabColor);
        tabs[index].color = tabColor;
        pages[index].SetActive(true);
    }
}
