using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;
    public float moveSpeed = 2f;
    private Transform target;

    void Start()
    {
        currentHP = maxHP;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
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
        Debug.Log(currentHP + "Mau Enemy");
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
