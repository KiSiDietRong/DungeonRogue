using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHP = 100f;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public float chaseRange = 2f;
    public float attackCooldown = 1f;

    [Header("References")]
    public Animator animator;
    private GameObject player;
    private float currentHP;
    private bool isDead = false;
    private float lastAttackTime;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Die = Animator.StringToHash("Die");

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            animator.SetTrigger(Attack);
            FlipTowardsPlayer();
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            animator.SetTrigger(Walk);
            FlipTowardsPlayer();
            MoveTowardsPlayer();
        }
        else
        {
            animator.SetTrigger(Idle);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void FlipTowardsPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            direction.x > 0 ? scaleX : -scaleX,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void AttackPlayer()
    {
        var playerScript = player.GetComponent<test>();
        if (playerScript != null)
        {
            Debug.Log($"Enemy attacks player for {damage} damage!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger(Hurt);

        if (currentHP <= 0)
        {
            DieEnemy();
        }
    }

    void DieEnemy()
    {
        isDead = true;
        animator.SetTrigger(Die);
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        float dieAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, dieAnimationLength);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            float bulletDamage = other.GetComponent<bullet>().damage;
            TakeDamage(bulletDamage);
        }
    }
}