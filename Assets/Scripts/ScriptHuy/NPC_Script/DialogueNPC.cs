using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour, INPCInteractable
{
    public string[] initialDialogue;      // Ví dụ: ["Xin chào người chơi"]
    public string[] option1Dialogue;      // "Bạn là ai?"
    public string[] orbHaveDialogue;      // Khi có orb
    public string[] orbNotHaveDialogue;   // Khi không có orb
    public string[] nothingDialogue;

    private string[] currentDialogue;
    private int currentLineIndex = 0;

    public GameObject dialogueUI;
    public Text dialogueText;

    public GameObject pressFText;
    public GameObject choicePanel;
    public Button button1;
    public Button button2;
    public Button button3;

    private PlayerController player;
    private bool isTalking = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public float typingSpeed = 0.05f;

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue()
    {
        if (initialDialogue.Length == 0) return;

        isTalking = true;
        currentLineIndex = 0;
        currentDialogue = initialDialogue;

        if (player != null)
            player.isInDialogue = true;

        dialogueUI.SetActive(true);
        StartTyping(currentDialogue[currentLineIndex]);
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentDialogue.Length)
        {
            StartTyping(currentDialogue[currentLineIndex]);
        }
        else
        {
            if (currentDialogue == initialDialogue)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void StartTyping(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentDialogue[currentLineIndex];
        isTyping = false;
    }

    private void EndDialogue()
    {
        isTalking = false;
        dialogueUI.SetActive(false);
        choicePanel.SetActive(false);
        dialogueText.text = "";

        if (player != null)
            player.isInDialogue = false;
    }

    private void ShowChoices()
    {
        choicePanel.SetActive(true);

        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button3.gameObject.SetActive(true);
    }

    public void OnChooseOption1()
    {
        choicePanel.SetActive(false);
        currentDialogue = option1Dialogue;
        currentLineIndex = 0;
        StartTyping(currentDialogue[currentLineIndex]);
    }

    public void OnChooseOption2()
    {
        choicePanel.SetActive(false);

        if (player != null && player.HasOrb())
        {
            currentDialogue = orbHaveDialogue;
        }
        else
        {
            currentDialogue = orbNotHaveDialogue;
        }

        currentLineIndex = 0;
        StartTyping(currentDialogue[currentLineIndex]);
    }
    public void OnChooseOption3()
    {
        choicePanel.SetActive(false);
        currentDialogue = nothingDialogue;
        currentLineIndex = 0;
        StartTyping(currentDialogue[currentLineIndex]);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetNearNPC(true, this);
                pressFText.SetActive(true);
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
                pressFText.SetActive(false);
            }
        }
    }
}
