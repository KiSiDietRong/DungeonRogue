using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float maxHealth = 100f;
    private float currentHealth;

    private Vector2 moveInput;
    public Vector2 lastMoveDirection { get; private set; } = Vector2.down;
    private bool isDashing = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.IsCanvasActive())
            return;

        HandleMovement();
        HandleActions();
    }

    void HandleMovement()
    {
        if (isDashing) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveInput != Vector2.zero)
            lastMoveDirection = moveInput;

        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            if (lastMoveDirection != Vector2.zero)
                StartCoroutine(Dash(lastMoveDirection));
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            transform.Translate(direction.normalized * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<bullet>().Initialize(shootDirection);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
        }
    }
}