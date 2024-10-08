using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Đối tượng mà camera xoay quanh
    public float rotationSpeed = 0.2f; // Tốc độ xoay camera
    public float zoomSpeed = 0.05f; // Tốc độ phóng to/thu nhỏ
    public float minZoom = 5.0f; // Giới hạn phóng to
    public float maxZoom = 20.0f; // Giới hạn thu nhỏ

    private Vector2 previousTouchPosition1;
    private Vector2 previousTouchPosition2;

    void Update()
    {
        // Kiểm tra nếu đang sử dụng thiết bị cảm ứng (điện thoại)
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1) // Nếu chỉ có 1 ngón tay
            {
                HandleMobileRotation();
            }
            else if (Input.touchCount == 2) // Nếu có 2 ngón tay
            {
                HandleMobileZoom();
            }
        }
        else
        {
            // Nếu không có cảm ứng thì kiểm tra điều khiển bằng chuột (dành cho máy tính)
            HandleMouseInput();
        }
    }

    // Điều khiển camera trên điện thoại bằng 1 ngón tay để xoay
    void HandleMobileRotation()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Moved)
        {
            float horizontalRotation = touch.deltaPosition.x * rotationSpeed;
            float verticalRotation = -touch.deltaPosition.y * rotationSpeed;

            // Xoay camera quanh đối tượng
            transform.RotateAround(target.position, Vector3.up, horizontalRotation);
            transform.RotateAround(target.position, transform.right, verticalRotation);
        }
    }

    // Điều khiển zoom trên điện thoại bằng pinch (2 ngón tay)
    void HandleMobileZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float currentTouchDeltaMag = (touch1.position - touch2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - currentTouchDeltaMag;

            Vector3 direction = transform.position - target.position;
            float currentDistance = direction.magnitude;

            float newDistance = Mathf.Clamp(currentDistance + deltaMagnitudeDiff * zoomSpeed, minZoom, maxZoom);
            transform.position = target.position + direction.normalized * newDistance;
        }
    }

    // Điều khiển bằng chuột cho máy tính
    void HandleMouseInput()
    {
        if (Input.GetMouseButton(0)) // Nếu chuột trái đang được giữ
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * 10f;
            float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed * 10f;

            // Xoay camera quanh đối tượng
            transform.RotateAround(target.position, Vector3.up, horizontalRotation);
            transform.RotateAround(target.position, transform.right, verticalRotation);
        }

        // Zoom bằng lăn chuột
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            Vector3 direction = transform.position - target.position;
            float currentDistance = direction.magnitude;

            float newDistance = Mathf.Clamp(currentDistance - scrollInput * 100 * zoomSpeed, minZoom, maxZoom);
            transform.position = target.position + direction.normalized * newDistance;
        }
    }
}
