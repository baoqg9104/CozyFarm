using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float zoomSpeed = 10f;  // Tốc độ zoom
    private float minZoom = 3f;     // Mức zoom tối thiểu
    private float maxZoom = 10f;    // Mức zoom tối đa
    private float targetZoom;       // Giá trị zoom đích
    private float smoothSpeed = 5f; // Độ mượt

    void Start()
    {
        // Đặt giá trị zoom ban đầu bằng giá trị hiện tại của camera
        targetZoom = Camera.main.orthographicSize;
    }

    void Update()
    {
        // Lấy giá trị lăn chuột
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        // Cập nhật giá trị zoom đích
        targetZoom -= scrollData * zoomSpeed;

        // Đảm bảo giá trị zoom đích nằm trong khoảng cho phép
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Thay đổi giá trị zoom hiện tại dần dần tới giá trị đích
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
    }
}
