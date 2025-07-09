using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    private bool isTyping;

    void Start()
    {
        StartCoroutine(ShowDialogueAfterDelay(1f));
    }

    void Update()
    {
        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextLine();
        }
    }

    IEnumerator ShowDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialoguePanel.SetActive(true);
        StartCoroutine(Typing());
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        isTyping = false;
    }

    public void NextLine()
    {
        contButton.SetActive(false);
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }
}
