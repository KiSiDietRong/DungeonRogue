using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public float moveSpeed = 5f;
    private Vector2 movement;
    private bool facingRight = true;

    private string currentAni;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        AnimatePlayer();
        FlipSprite();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // Giữ vận tốc đều khi di chuyển chéo
    }

    private void MovePlayer()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    private void AnimatePlayer()
    {
        if (movement != Vector2.zero)
        {
            ChangeAnimation("run");
        }
        else
        {
            ChangeAnimation("idle");
        }
    }

    private void FlipSprite()
    {
        if (movement.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (movement.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void ChangeAnimation(string aniName)
    {
        if (currentAni == aniName) return;

        animator.ResetTrigger(currentAni);
        currentAni = aniName;
        animator.SetTrigger(currentAni);
    }
}
