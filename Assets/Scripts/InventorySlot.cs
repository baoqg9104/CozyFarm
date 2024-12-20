using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake() {
        Deselect();
    }

    public void Select() {
        image.color = selectedColor;
    }

    public void Deselect() {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (draggedItem != null)
        {
            if (transform.childCount == 0)
            {
                // Không có item nào trong slot, di chuyển item vào đây
                draggedItem.parentAfterDrag = transform;
            }
            else
            {
                // Đã có item trong slot, hoán đổi vị trí với item đang kéo
                Transform existingItemTransform = transform.GetChild(0);
                InventoryItem existingItem = existingItemTransform.GetComponent<InventoryItem>();

                if (existingItem != null)
                {
                    // Đặt lại parent của item cũ về slot gốc của item mới
                    existingItem.parentAfterDrag = draggedItem.parentAfterDrag;
                    existingItemTransform.SetParent(existingItem.parentAfterDrag);

                    // Đặt lại parent của item mới về slot hiện tại
                    draggedItem.parentAfterDrag = transform;
                }
            }
        }
    }

}
