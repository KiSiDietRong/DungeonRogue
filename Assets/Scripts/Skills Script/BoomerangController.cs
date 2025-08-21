using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    private Transform player;
    private Vector2 moveDir;
    private float speed;
    private float maxDistance;
    private float returnSpeed;
    private int damage;
    private string targetTag;

    private Vector3 startPos;
    private bool returning = false;

    public void Init(Transform player, Vector2 dir, float speed, float maxDistance, float returnSpeed, int damage, string targetTag)
    {
        this.player = player;
        this.moveDir = dir;
        this.speed = speed;
        this.maxDistance = maxDistance;
        this.returnSpeed = returnSpeed;
        this.damage = damage;
        this.targetTag = targetTag;
        startPos = transform.position;
    }

    void Update()
    {
        if (!returning)
        {
            transform.position += (Vector3)moveDir * speed * Time.deltaTime;
            if (Vector3.Distance(startPos, transform.position) >= maxDistance)
                returning = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, returnSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Enemy e = collision.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
        }
    }
}
