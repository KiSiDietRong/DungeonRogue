// PlayerDemo.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private INPCInteractable currentNPCs;

    public bool hasOrb = false;
    public int Gold = 500;
    public Text goldText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateGoldUI();
    }

    void Update()
    {
        if (isEnteringPortal) return;

        //moveInput = Vector2.zero;
        //if (Input.GetKey(KeyManager.Instance.GetKey("MoveUp"))) moveInput.y += 1;
        //if (Input.GetKey(KeyManager.Instance.GetKey("MoveDown"))) moveInput.y -= 1;
        //if (Input.GetKey(KeyManager.Instance.GetKey("MoveLeft"))) moveInput.x -= 1;
        //if (Input.GetKey(KeyManager.Instance.GetKey("MoveRight"))) moveInput.x += 1;
        //moveInput.Normalize();

        // Portal sử dụng phím Interact (F)
        if (nearPortal && Input.GetKeyDown(KeyManager.Instance.GetKey("Interact")))
        {
            StartCoroutine(MoveToPortalAndEnter());
        }

        if (nearNPC && Input.GetKeyDown(KeyManager.Instance.GetKey("Interact")))
        {
            currentNPCs.StartDialogue();
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Skill1")))
        {
            Debug.Log("Skill 1 dùng");
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Skill2")))
        {
            Debug.Log("Skill 2 dùng");
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Attack")))
        {
            Debug.Log("Tấn công");
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Dash")))
        {
            Debug.Log("Dash / né tránh");
        }

        if (Input.GetKeyDown(KeyManager.Instance.GetKey("Inventory")))
        {
            Debug.Log("Inventory mở (Tab)");
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
            goldText.text = $"Gold: {Gold}";
    }
}
