using UnityEngine;
using UnityEngine.UI;

public class TrashCan : MonoBehaviour
{
    private bool isItemInside = false; // Theo dõi xem vật phẩm có nằm trong vùng thùng rác không
    private GameObject itemInside; // Lưu trữ vật phẩm trong vùng
    private Image trashCanImage; // Đối tượng Image của thùng rác
    private Color originalColor; // Màu gốc của Image

    private void Start()
    {
        // Lấy component Image từ đối tượng thùng rác
        trashCanImage = GetComponent<Image>();
        if (trashCanImage != null)
        {
            originalColor = trashCanImage.color; // Lưu lại màu gốc
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            isItemInside = true;
            itemInside = other.gameObject;

            // Giảm alpha của Image để tạo hiệu ứng
            if (trashCanImage != null)
            {
                Color newColor = trashCanImage.color;
                newColor.a = 1f; // Đặt alpha mới (0.5 = 50% trong suốt)
                trashCanImage.color = newColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            isItemInside = false;
            itemInside = null;

            // Khôi phục alpha của Image về trạng thái ban đầu
            if (trashCanImage != null)
            {
                trashCanImage.color = originalColor;
            }
        }
    }

    private void Update()
    {
        // Nếu chuột thả ra và vật phẩm đang ở trong vùng thùng rác
        if (isItemInside && Input.GetMouseButtonUp(0))
        {
            // Kiểm tra trạng thái kéo
            InventoryItem draggable = itemInside.GetComponent<InventoryItem>();
            if (draggable != null && !draggable.isDragging)
            {
                Debug.Log("Item destroyed");
                Destroy(itemInside); // Xóa vật phẩm
                isItemInside = false; // Đặt lại trạng thái
                itemInside = null;
            }
        }
    }
}
