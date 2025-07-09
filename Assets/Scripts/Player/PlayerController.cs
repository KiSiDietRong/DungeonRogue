using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;
    private Vector2 lastMoveDir;

    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    private bool facingLeft = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        HandleInput();
        UpdateFacingDirection();
        UpdateAnimator();
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
    private void UpdateFacingDirection()
    {
        if (movement.x < 0)
            facingLeft = true;
        else if (movement.x > 0)
            facingLeft = false;
    }
    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
            lastMoveDir = movement;
    }
    private void UpdateAnimator()
    {
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
        animator.SetFloat("lastMoveX", lastMoveDir.x);
        animator.SetFloat("lastMoveY", lastMoveDir.y);
        animator.SetBool("isMoving", movement != Vector2.zero);
    }
}
