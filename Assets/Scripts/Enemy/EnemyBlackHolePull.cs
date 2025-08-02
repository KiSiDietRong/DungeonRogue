using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBlackHolePull : MonoBehaviour
{
    private bool beingPulled = false;
    private Vector2 pullTarget;
    private float pullSpeed = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (beingPulled)
        {
            Vector2 direction = (pullTarget - rb.position).normalized;
            Vector2 newPosition = rb.position + direction * pullSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
    }

    public void PullTowards(Vector2 center, float speed = 5f)
    {
        pullTarget = center;
        pullSpeed = speed;
        beingPulled = true;
    }

    public void StopPull()
    {
        beingPulled = false;
    }
}
