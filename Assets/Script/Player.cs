using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 moveInput;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private float lastDashTime;
    private bool isDashing;
    private float dashEndTime;

    private PlayerInfo stats;
    private Rigidbody2D rb;

    void Start()
    {
        stats = GetComponent<PlayerInfo>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            dashEndTime = Time.time + dashDuration;
            lastDashTime = Time.time;
            rb.linearVelocity = moveInput * dashForce;
        }
        if (isDashing && Time.time >= dashEndTime)
        {
            isDashing = false;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = moveInput * stats.speed;
        }
    }
}
