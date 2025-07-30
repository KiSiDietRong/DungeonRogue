using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats & Dash")]
    public CharacterStatSO characterStat;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.25f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private float doomShellRadius = 2f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    private Knockback Knockback;
    private InventoryManager inventoryManager;

    [Header("Movement")]
    private Vector2 movement;
    private Vector2 lastMoveDir;
    private bool facingLeft = false;
    public bool FacingLeft { get => facingLeft; set => facingLeft = value; }
    private float baseMoveSpeed;
    private float moveSpeed = 5f;

    private bool isDashing = false;
    private bool canDash = true;

    [Header("Portal & NPC")]
    private bool nearPortal = false;
    private Transform portalTransform;
    private bool isEnteringPortal = false;

    private bool nearNPC = false;
    private INPCInteractable currentNPCs;

    [Header("Currency & Items")]
    public bool hasOrb = false;
    public int Gold = 500;
    public Text goldText;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Knockback = GetComponent<Knockback>();
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (characterStat == null)
        {
            baseMoveSpeed = 5f;
        }
        else
        {
            baseMoveSpeed = characterStat.moveSpeed;
        }

        moveSpeed = baseMoveSpeed;
        UpdateGoldUI();
    }

    void Update()
    {
        if (isEnteringPortal) return;

        HandleInput();
        UpdateFacingDirection();
        UpdateAnimator();

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Dash")) && canDash && movement != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (nearPortal && Input.GetKeyDown(KeyManager.Instance.GetKey("Interact")))
        {
            StartCoroutine(MoveToPortalAndEnter());
        }

        if (nearNPC && Input.GetKeyDown(KeyManager.Instance.GetKey("Interact")))
        {
            currentNPCs.StartDialogue();
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Skill1")))
            Debug.Log("Skill 1 dùng");

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Skill2")))
            Debug.Log("Skill 2 dùng");

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Attack")))
            Debug.Log("Tấn công");

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Inventory")))
            Debug.Log("Inventory mở (Tab)");
    }

    void FixedUpdate()
    {
        if (isEnteringPortal) return;
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

        if (inventoryManager != null && inventoryManager.playerInventory.Exists(r => r.type == RelicType.DoomShell))
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

    private IEnumerator MoveToPortalAndEnter()
    {
        isEnteringPortal = true;
        Vector3 portalPosition = new Vector3(portalTransform.position.x, portalTransform.position.y - 1f, 0);

        while (Vector2.Distance(transform.position, portalPosition) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, portalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }

    public void SetNearPortal(bool value, Transform portal)
    {
        nearPortal = value;
        portalTransform = portal;
    }

    public void SetNearNPC(bool value, INPCInteractable npc)
    {
        nearNPC = value;
        currentNPCs = value ? npc : null;
    }

    public void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {Gold}";
    }

    public bool HasOrb()
    {
        return hasOrb;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, doomShellRadius);
    }
}
