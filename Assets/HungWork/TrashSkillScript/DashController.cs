using UnityEngine;

public class DashController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDashing = false;
    private float dashTime;
    private Vector2 dashDirection;
    private float dashSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                rb.linearVelocity = Vector2.zero;
                isDashing = false;
            }
        }
    }

    public void PerformDash(Vector2 direction, float distance, float duration)
    {
        if (!isDashing)
        {
            isDashing = true;
            dashDirection = direction;
            dashSpeed = distance / duration;
            dashTime = duration;
        }
    }
}