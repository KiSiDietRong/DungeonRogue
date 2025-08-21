using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<EnemyMau>())
        {
            EnemyMau enemyMau = other.gameObject.GetComponent<EnemyMau>();
            enemyMau.TakeDamage(damageAmount);
        }
    }
}
