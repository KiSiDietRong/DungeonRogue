using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public bool isDashing = false;
    private bool canDash = true;

    private bool nearPortal = false;
    private Transform portalTransform;
    private bool isEnteringPortal = false;
    private bool isSlowed = false;

    private bool nearNPC = false;
    private DialogueNPC currentNPC;
    private INPCInteractable currentNPCs;
    public bool isInDialogue = false;

    public bool hasOrb = false;
    public int Gold = 500;
    public int Souls = 0;
    public Text soulText;
    public Text goldText;

    private float baseMoveSpeed;
    private float moveSpeed = 5f;

    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    private bool facingLeft = false;
    internal Vector2 lastMoveDirection;

    public static PlayerController Instance;

    void Start()
    {
        Souls = UserDataSave.Instance.GetSouls();
        UpdateSoulUI();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        if (isInDialogue)
        {
            return;
        }

        HandleInput();
        UpdateFacingDirection();
        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.Space) && canDash && movement != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (nearPortal && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(MoveToPortalAndEnter());
        }

        if (nearNPC && Input.GetKeyDown(KeyCode.F))
        {
            currentNPCs.StartDialogue();
        }
    }

    void FixedUpdate()
    {
        if (!isInDialogue)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
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

    public bool HasOrb()
    {
        return hasOrb;
    }
    public void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"{Gold}";
    }

    public void UpdateSoulUI()
    {
        if (soulText != null)
            soulText.text = $"{Souls}";
    }

    public void AddSouls(int amount)
    {
        Souls += amount;
        UpdateSoulUI();
        UserDataSave.Instance.AddSouls(amount); 
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
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    int damage = Random.Range(5, 11);
                    enemyScript.TakeDamage(damage, enemy.transform.position, false);
                    if (playerHealth != null)
                    {
                        playerHealth.AddDamageDealt(damage);
                    }
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

    public void ApplySlow(float duration)
    {
        if (isSlowed) return;
        StartCoroutine(SlowCoroutine(duration));
    }

    private IEnumerator SlowCoroutine(float duration)
    {
        isSlowed = true;
        float originalSpeed = moveSpeed;
        moveSpeed = 0.3f;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;
        isSlowed = false;
    }

    public void ResetCurrency()
    {
        Gold = 0;
        UpdateGoldUI();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, doomShellRadius);
    }
}