using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;
    public GameObject durabilityBar;
    public GameObject durabilityBarEmpty;

    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    public int durability;
    [HideInInspector] public Transform parentAfterDrag;

    public RectTransform durabilityBarRect;
    public bool isDragging = false;

    private int maxDurability;
    

    private void Start() {

        if (item.type == ItemType.Tool && item.hasDurability)
        {   
            if (item.actionType == ActionType.Fishing) {
                maxDurability = 20;
            } else if (item.actionType == ActionType.Farm) {
                maxDurability = 50;
            }

            durability = maxDurability;
            durabilityBar.SetActive(true);
            durabilityBarEmpty.SetActive(true);
            RefreshDurability();
        } else {
            durabilityBar.SetActive(false);
            durabilityBarEmpty.SetActive(false);
        }
    }

    public void InitialiseItem(Item newItem) {

        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();

        if (item.type == ItemType.Tool)
        {
            RefreshDurability();
        }
    }

    public void RefreshCount() {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void RefreshDurability()
    {
        float durabilityRatio = Mathf.Clamp01((float)durability / maxDurability);

         // Update color
        durabilityBar.GetComponent<Image>().color = GetDurabilityColor(durabilityRatio);

        // Update width
        float maxWidth = 30f;
        float minWidth = 0;

        // Tính toán chiều rộng mới dựa trên độ bền hiện tại của công cụ
        float newWidth = Mathf.Lerp(minWidth, maxWidth, durabilityRatio);

        Vector2 sizeDelta = durabilityBarRect.sizeDelta;
        sizeDelta.x = newWidth;
        durabilityBarRect.sizeDelta = sizeDelta;

    }

    public void ReduceCount() {
        count = Mathf.Max(0, count - 1);
        RefreshCount();
    }

    public void ReduceDurability()
    {
        durability = Mathf.Max(0, durability - 1);
        RefreshDurability();
    }

     private Color GetDurabilityColor(float durabilityRatio)
    {
        if (durabilityRatio >= 0.85f) return HexToColor("#04FB00");
        else if (durabilityRatio >= 0.70f) return HexToColor("#50FF00");
        else if (durabilityRatio >= 0.55f) return HexToColor("#8DFF00");
        else if (durabilityRatio >= 0.40f) return HexToColor("#F5FF00");
        else if (durabilityRatio >= 0.25f) return HexToColor("#FFA200");
        else if (durabilityRatio >= 0.10f) return HexToColor("#FF2E00");
        else return HexToColor("#FF0000");
    }

    private Color HexToColor(string hex)
    {
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return Color.white; // Default color if parsing fails
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        isDragging = false;
    }
}
