using UnityEngine;

public class DanEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private float projectileRange = 10f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, startPosition) > projectileRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player)
        {
            player.TakeDamage(1, transform);
            Destroy(gameObject);
        }
    }
}
