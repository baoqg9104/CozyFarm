using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public BoxCollider2D movementArea; // Vùng cụ thể để di chuyển (BoxCollider2D)
    public float speed = 1f; // Tốc độ di chuyển
    public float waitTime = 2f; // Thời gian dừng trước khi chọn điểm mới
    public Animator animator;

    private Vector3 targetPosition;
    private float waitTimer;
    private Vector3 lastPosition; // Lưu vị trí cuối cùng
    private enum Direction { Horizontal, Vertical }
    private Direction moveDirection;

    void Start()
    {
        // Chọn vị trí ngẫu nhiên ban đầu trong vùng cụ thể
        ChooseRandomPosition();
    }

    void Update()
    {
        // Di chuyển bot tới vị trí mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Tính toán vector di chuyển
        Vector3 movement = transform.position - lastPosition;

        // Cập nhật hoạt ảnh
        if (movement.x != 0)
        {
            animator.SetBool("IsMoveLeftOrRight", true);
            animator.SetBool("IsMoveFront", false);
            animator.SetBool("IsMoveBack", false);

            // Lật hướng bot
            if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); // Đi trái
            }
            else if (movement.x > 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); // Đi phải
            }
        }
        else if (movement.y != 0)
        {
            animator.SetBool("IsMoveLeftOrRight", false);

            if (movement.y > 0)
            {
                animator.SetBool("IsMoveFront", false);
                animator.SetBool("IsMoveBack", true); // Đi lên
            }
            else
            {
                animator.SetBool("IsMoveFront", true); // Đi xuống
                animator.SetBool("IsMoveBack", false);
            }
        }
        else
        {
            // Dừng hoạt ảnh
            animator.SetBool("IsMoveLeftOrRight", false);
            animator.SetBool("IsMoveFront", false);
            animator.SetBool("IsMoveBack", false);
        }

        // Cập nhật vị trí cuối cùng
        lastPosition = transform.position;

        // Nếu đến gần mục tiêu, chọn vị trí mới sau thời gian chờ
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                ChooseRandomPosition();
                waitTimer = 0;
            }
        }
    }

    void ChooseRandomPosition()
    {
        // Nếu vùng di chuyển chưa được thiết lập, thoát
        if (movementArea == null)
        {
            Debug.LogWarning("Movement area is not set.");
            return;
        }

        // Lấy giới hạn của BoxCollider2D
        Bounds bounds = movementArea.bounds;

        // Chọn chiều di chuyển ngẫu nhiên (ngang hoặc dọc)
        moveDirection = (Random.value > 0.5f) ? Direction.Horizontal : Direction.Vertical;

        if (moveDirection == Direction.Horizontal)
        {
            // Di chuyển theo chiều ngang
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            targetPosition = new Vector3(randomX, transform.position.y, transform.position.z);
        }
        else
        {
            // Di chuyển theo chiều dọc
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            targetPosition = new Vector3(transform.position.x, randomY, transform.position.z);
        }
    }
}
