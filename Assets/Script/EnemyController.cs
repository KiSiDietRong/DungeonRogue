using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;

    public float moveSpeed = 2f;
    private Transform target;

    public bool isPulledByBlackHole = false; // ✅ Bị hút bởi black hole

    void Start()
    {
        currentHP = maxHP;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isPulledByBlackHole)
            return; // ✅ Không di chuyển nếu đang bị hút

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.z = 0;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log(currentHP + " Máu Enemy");
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
