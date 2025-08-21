using System.Collections.Generic;
using UnityEngine;

public class BounceSkillController : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private int maxBounces;
    private int currentBounces = 0;
    private float bounceRange;
    private GameObject owner;

    private Rigidbody2D rb;

    private readonly HashSet<GameObject> hitTargets = new();

    private float lifetime = 3f; // thời gian tự hủy

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Setup(Vector3 dir, float spd, float dmg, int maxBounceCount, float range, GameObject ownerObj)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        maxBounces = maxBounceCount;
        bounceRange = range;
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
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;
        if (!other.CompareTag("Enemy")) return;
        if (hitTargets.Contains(other.gameObject)) return;

        hitTargets.Add(other.gameObject);

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        currentBounces++;

        if (currentBounces >= maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        GameObject nextTarget = FindNextTarget(other.transform.position);

        if (nextTarget != null)
        {
            Vector3 nextDir = (nextTarget.transform.position - transform.position).normalized;
            direction = nextDir;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private GameObject FindNextTarget(Vector3 fromPosition)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(fromPosition, bounceRange);

        GameObject closestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && !hitTargets.Contains(hit.gameObject))
            {
                float dist = Vector3.Distance(fromPosition, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = hit.gameObject;
                }
            }
        }

        return closestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, bounceRange);
    }
}
