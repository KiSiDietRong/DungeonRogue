using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDemo : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private bool nearPortal = false;
    private Transform portalTransform;
    private bool isEnteringPortal = false;

    private bool nearNPC = false;
    private DialogueNPC currentNPC;

    public bool hasOrb = false; // <--- THÊM BIẾN NÀY

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isEnteringPortal) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (nearPortal && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(MoveToPortalAndEnter());
        }

        if (nearNPC && Input.GetKeyDown(KeyCode.F))
        {
            currentNPC.StartDialogue();
        }
    }

    void FixedUpdate()
    {
        if (isEnteringPortal) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
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

    public void SetNearNPC(bool value, DialogueNPC npc)
    {
        nearNPC = value;
        currentNPC = value ? npc : null;
    }

    // Getter cho DialogueNPC gọi
    public bool HasOrb()
    {
        return hasOrb;
    }
}
