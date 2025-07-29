using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterStatSO characterStat;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.25f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private float doomShellRadius = 2f;

    public Rigidbody2D rb;
    public Animator animator;

    private Knockback Knockback;
    private Vector2 movement;
    private Vector2 lastMoveDir;
    private InventoryManager inventoryManager;

    private bool isDashing = false;
    private bool canDash = true;

    private float baseMoveSpeed;
    private float moveSpeed = 5f;

    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    private bool facingLeft = false;
    internal Vector2 lastMoveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Knockback = GetComponent<Knockback>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (characterStat == null)
        {
            baseMoveSpeed = 5f;
            moveSpeed = baseMoveSpeed;
        }
    }

    void Update()
    {
        HandleInput();
        UpdateFacingDirection();
        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.Space) && canDash && movement != Vector2.zero)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
            lastMoveDir = movement;
    }

    private void UpdateFacingDirection()
    {
        if (movement.x < 0)
            facingLeft = true;
        else if (movement.x > 0)
            facingLeft = false;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
        animator.SetFloat("lastMoveX", lastMoveDir.x);
        animator.SetFloat("lastMoveY", lastMoveDir.y);
        animator.SetBool("isMoving", movement != Vector2.zero);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalMoveSpeed = moveSpeed;
        moveSpeed = dashSpeed;
        myTrailRenderer.emitting = true;

        if (inventoryManager != null && inventoryManager.playerInventory.Exists(relic => relic.type == RelicType.DoomShell))
        {
            ApplyDoomShellEffect();
        }

        yield return new WaitForSeconds(dashDuration);

        moveSpeed = baseMoveSpeed;
        myTrailRenderer.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void ApplyDoomShellEffect()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, doomShellRadius);
        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    int damage = Random.Range(5, 11);
                    enemyScript.TakeDamage(damage, enemy.transform.position, false);
                }
            }
        }
    }

    public void InitFromStats(CharacterStatSO stats)
    {
        characterStat = stats;
        baseMoveSpeed = characterStat.moveSpeed;
        moveSpeed = baseMoveSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, doomShellRadius);
    }
}