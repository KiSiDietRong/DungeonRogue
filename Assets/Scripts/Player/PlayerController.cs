using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;          
    [SerializeField] private float dashSpeed = 10f;        
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.25f;
    [SerializeField] private TrailRenderer myTrailRenderer;

    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;
    private Vector2 lastMoveDir;

    private bool isDashing = false;
    private bool canDash = true;
    private float baseMoveSpeed;

    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    private bool facingLeft = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        baseMoveSpeed = moveSpeed;
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

        moveSpeed = dashSpeed;
        myTrailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        moveSpeed = baseMoveSpeed;
        myTrailRenderer.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
