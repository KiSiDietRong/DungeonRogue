using UnityEngine;

public class BulletEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    private Transform playerTransform;
    private void Awake()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    public void Attack()
    {
        Vector2 targetDirection = playerTransform.position - transform.position;

        GameObject newBullet = Instantiate(bulletPrefab, transform.position,Quaternion.identity);
        newBullet.transform.right = targetDirection;
    }
}
