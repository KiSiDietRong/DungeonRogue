using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float magnetRadius = 5f;
    public float moveSpeed = 5f;

    private Transform playerTransform;

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance <= magnetRadius)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Gold += value;
                player.UpdateGoldUI();
            }

            Destroy(gameObject);
        }
    }
}
