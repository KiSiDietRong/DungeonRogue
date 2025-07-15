using UnityEngine;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour
{
    public string[] dialogueLines;  // Các dòng hội thoại
    private int currentLineIndex = 0;
    private bool isTalking = false;

    public GameObject dialogueUI; // Panel UI chứa Text
    public Text dialogueText;     // Text hiển thị câu thoại

    private PlayerDemo player;

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue()
    {
        if (dialogueLines.Length == 0) return;

        isTalking = true;
        currentLineIndex = 0;
        dialogueUI.SetActive(true);
        dialogueText.text = dialogueLines[currentLineIndex];
    }

    private void ShowNextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        dialogueUI.SetActive(false);
    }

    // Trigger khi Player đến gần
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerDemo>();
            if (player != null)
            {
                player.SetNearNPC(true, this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.SetNearNPC(false, null);
            }
        }
    }
}
