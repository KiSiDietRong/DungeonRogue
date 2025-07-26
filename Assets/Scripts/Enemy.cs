using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHP = 100f;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float chaseRange = 4f;
    public float attackCooldown = 1f;
    [SerializeField] private float popupOffsetRadius = 0.5f;

    [Header("References")]
    public GameObject damagePopupPrefab;
    public Animator animator;
    [SerializeField] private ParticleSystem stunEffect;
    private GameObject player;
    private float currentHP;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isStunned = false;
    private float lastAttackTime;
    private Knockback knockback;
    public string lastDamageSource = "";

    private bool isPatrolling = false;
    private Vector2 patrolDirection = Vector2.left;
    private float patrolSpeed = 1f;
    private float patrolInterval = 1f;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Die = Animator.StringToHash("Die");

    public delegate void EnemyDeathHandler(Enemy enemy);
    public static event EnemyDeathHandler OnEnemyDeath;

    // Getter để truy cập trạng thái isStunned
    public bool IsStunned => isStunned;

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player");
        if (animator == null) animator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();
        StartCoroutine(CheckForPlayerAndStartPatrolLoop());
    }

    void Update()
    {
        if (isDead || player == null || isAttacking || knockback.GettingKnockedBack || isPatrolling || isStunned) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            FlipTowardsPlayer();
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                StartCoroutine(AttackPlayer());
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isPatrolling = false;
            animator.ResetTrigger(Idle);
            animator.ResetTrigger(Hurt);
            animator.SetTrigger(Walk);
            FlipTowardsPlayer();
            MoveTowardsPlayer();
        }
        else
        {
            animator.ResetTrigger(Walk);
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

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger(Attack);

        AnimatorStateInfo stateInfo;
        float timeout = 1f;
        while (timeout > 0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack")) break;
            timeout -= Time.deltaTime;
            yield return null;
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float attackAnimLength = stateInfo.length;
        yield return new WaitForSeconds(attackAnimLength * 0.5f);

        if (!isStunned && !isDead)
        {
            var playerScript = player.GetComponent<PlayerHealth>();
            if (playerScript != null && !playerScript.GetComponent<Knockback>().GettingKnockedBack)
            {
                playerScript.TakeDamage((int)damage, transform);
            }
        }

        yield return new WaitForSeconds(attackAnimLength * 0.5f);
        animator.SetTrigger(Idle);

        float remainingCooldown = Mathf.Max(0f, attackCooldown - attackAnimLength);
        if (remainingCooldown > 0f) yield return new WaitForSeconds(remainingCooldown);

        isAttacking = false;
    }

    public void TakeDamage(float damage, Vector3 hitPosition, bool isCritical)
    {
        if (isDead || knockback.GettingKnockedBack) return;

        currentHP -= damage;

        if (damagePopupPrefab != null)
        {
            Vector2 offset = Random.insideUnitCircle * popupOffsetRadius;
            Vector3 popupPosition = transform.position + new Vector3(offset.x, offset.y + 0.3f, 0f);
            GameObject popup = Instantiate(damagePopupPrefab, popupPosition, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup((int)damage, isCritical);
            }
        }

        animator.SetTrigger(Hurt);

        if (isPatrolling)
        {
            isPatrolling = false;
            StopCoroutine(PatrolRoutine());
        }

        if (currentHP <= 0) DieEnemy();
        else StartCoroutine(RecoverFromHurt());
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, transform.position, false);
    }

    public void Stun(float duration)
    {
        if (!isDead && !isStunned)
        {
            // Kiểm tra Traumatic Blow trước khi làm choáng
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.TraumaticBlow) && currentHP <= maxHP * 0.2f)
            {
                Debug.Log($"Traumatic Blow triggered: Instantly defeated {gameObject.name} with HP {currentHP}/{maxHP} (<20%).");
                DieEnemy();
                return;
            }

            StartCoroutine(StunEnemy(duration));
        }
    }

    IEnumerator StunEnemy(float duration)
    {
        isStunned = true;
        animator.SetTrigger(Idle);
        Debug.Log($"{gameObject.name} is stunned for {duration} seconds.");

        // Kiểm tra ImpactCharm và gây sát thương diện rộng
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.ImpactCharm))
        {
            float aoeRadius = 2f;
            float aoeDamage = 15f;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            foreach (Collider2D hitEnemy in hitEnemies)
            {
                if (hitEnemy.CompareTag("Enemy"))
                {
                    Enemy enemy = hitEnemy.GetComponent<Enemy>();
                    if (enemy != null && enemy != this && !enemy.isDead)
                    {
                        enemy.TakeDamage(aoeDamage, transform.position, false);
                        Debug.Log($"ImpactCharm triggered: Dealt {aoeDamage} AOE damage to {enemy.gameObject.name} around stunned enemy {gameObject.name}.");
                    }
                }
            }
        }

        ParticleSystem stunInstance = null;
        if (stunEffect != null)
        {
            stunInstance = Instantiate(stunEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
            stunInstance.Play();
        }

        yield return new WaitForSeconds(duration);

        if (stunInstance != null)
        {
            stunInstance.Stop();
            Destroy(stunInstance.gameObject);
        }

        isStunned = false;
        if (!isDead)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= chaseRange)
            {
                animator.SetTrigger(Walk);
            }
            else
            {
                animator.SetTrigger(Idle);
            }
        }
    }

    IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.3f);

        if (isDead || isStunned) yield break;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= chaseRange)
        {
            animator.ResetTrigger(Hurt);
            animator.SetTrigger(Walk);
        }
        else
        {
            animator.ResetTrigger(Hurt);
            animator.SetTrigger(Idle);
        }
    }

    void DieEnemy()
    {
        isDead = true;
        animator.SetTrigger(Die);
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        float dieAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject, dieAnimationLength);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (knockback != null)
            {
                knockback.GetKnockedBack(other.transform, 0.2f);
            }
            lastDamageSource = "Projectile";
            Destroy(other.gameObject);
        }
    }

    IEnumerator CheckForPlayerAndStartPatrolLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(2f);

            if (player == null || isAttacking || knockback.GettingKnockedBack || isDead || isStunned) continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance > chaseRange && !isPatrolling)
            {
                StartCoroutine(PatrolRoutine());
            }
        }
    }

    IEnumerator PatrolRoutine()
    {
        isPatrolling = true;
        while (isPatrolling)
        {
            if (player == null || isDead || isStunned) yield break;

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= chaseRange)
            {
                isPatrolling = false;
                yield break;
            }

            animator.ResetTrigger(Hurt);
            animator.ResetTrigger(Idle);
            animator.SetTrigger(Walk);

            Flip(patrolDirection.x);

            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + (Vector3)patrolDirection;

            float elapsed = 0f;
            while (elapsed < patrolInterval)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / patrolInterval);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;

            patrolDirection *= -1;
            yield return null;
        }
    }

    void Flip(float directionX)
    {
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            directionX > 0 ? scaleX : -scaleX,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}