using UnityEngine;

public class FlameController : MonoBehaviour
{
    public float duration = 3f;
    public float tickRate = 0.2f;
    public int damage = 5;

    private float timeAlive;
    private float nextTick;
    private Transform user;

    public void Initialize(Transform owner, Vector3 target)
    {
        user = owner;
        transform.SetParent(user);
        transform.position = user.position;

        Vector3 dir = (target - user.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time >= nextTick && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            nextTick = Time.time + tickRate;
        }
    }
}