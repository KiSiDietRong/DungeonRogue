using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDemo : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool nearPortal = false;
    private Transform portalTransform;
    private bool isEnteringPortal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isEnteringPortal) return;

        // Nhận input từ bàn phím
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize(); // Đảm bảo tốc độ không tăng khi đi chéo

        // Nếu đang gần cổng và nhấn phím E
        if (nearPortal && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(MoveToPortalAndEnter());
        }
    }

    void FixedUpdate()
    {
        if (isEnteringPortal) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator MoveToPortalAndEnter()
    {
        isEnteringPortal = true;

        // Xác định vị trí trung tâm trước cổng (Z không cần vì 2D)
        Vector3 portalPosition = new Vector3(portalTransform.position.x, portalTransform.position.y - 1f, 0);

        // Di chuyển đến trước cổng (xấp xỉ)
        while (Vector2.Distance(transform.position, portalPosition) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, portalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Đợi 1 giây rồi vào cổng
        yield return new WaitForSeconds(1f);

        // Load scene mới (đảm bảo đã thêm scene trong Build Settings)
        SceneManager.LoadScene("Game");
    }

    // Gọi từ Portal khi chạm trigger
    public void SetNearPortal(bool value, Transform portal)
    {
        nearPortal = value;
        portalTransform = portal;
    }
}
