using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;

    public GameObject itemPrefab;
    private GameObject player;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, 5f);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

            if (player != null && itemPrefab != null)
            {
                Vector2 offset = Vector2.down;

                var playerScript = player.GetComponent<test>();
                if (playerScript != null && playerScript.lastMoveDirection != Vector2.zero)
                    offset = playerScript.lastMoveDirection.normalized;

                Vector3 spawnPos = player.transform.position + (Vector3)(offset * 2f);
                Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
