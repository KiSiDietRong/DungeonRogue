using UnityEngine;

public class FollowingBulletController : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private float damage;
    private float homingRange;
    private float lifeTime;
    private GameObject owner;

    private float lifeTimer = 0f;

    private Rigidbody2D rb;

    private GameObject currentTarget;

    public void Setup(Vector3 initialDirection, float spd, float dmg, float range, float lifetimeSec, GameObject ownerObj)
    {
        moveDirection = initialDirection.normalized;
        speed = spd;
        damage = dmg;
        homingRange = range;
        lifeTime = lifetimeSec;
        owner = ownerObj;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (currentTarget == null)
        {
            currentTarget = FindClosestTarget();
        }

        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;

            moveDirection = Vector3.Lerp(moveDirection, directionToTarget, Time.deltaTime * 5f).normalized;

            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle+90);
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private GameObject FindClosestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, homingRange);
        GameObject closestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = hit.gameObject;
                }
            }
        }
        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, homingRange);
    }
}
