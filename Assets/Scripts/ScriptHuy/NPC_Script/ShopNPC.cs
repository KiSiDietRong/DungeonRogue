using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour, INPCInteractable
{
    public string[] greetingDialogue;
    public string[] aboutShopDialogue;
    public string[] rerollDialogue;

    public GameObject dialogueUI;
    public Text dialogueText;
    public GameObject pressFText;
    public GameObject choicePanel;
    public Button button1;
    public Button button2;

    public ShopRandom shopRandom; // Script spawn item
    private PlayerController player;
    private bool isTalking = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private string[] currentDialogue;
    private int currentLineIndex = 0;
    private int rerollCount = 0;
    public float typingSpeed = 0.05f;

    void Start()
    {

    }

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                SkipTyping();
            else
                ShowNextLine();
        }
    }

    public void StartDialogue()
    {
        isTalking = true;
        currentLineIndex = 0;
        currentDialogue = greetingDialogue;
        dialogueUI.SetActive(true);
        StartTyping(currentDialogue[currentLineIndex]);

        // Gán sự kiện ở đây nếu cần đảm bảo chắc chắn không trùng
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button1.onClick.AddListener(OnChooseOption1);
        button2.onClick.AddListener(OnChooseOption2);
    }


    void ShowNextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentDialogue.Length)
        {
            StartTyping(currentDialogue[currentLineIndex]);
        }
        else
        {
            if (currentDialogue == greetingDialogue)
                ShowChoices();
            else
                EndDialogue();
        }
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
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

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentDialogue[currentLineIndex];
        isTyping = false;
    }

    void ShowChoices()
    {
        choicePanel.SetActive(true);
        button1.GetComponentInChildren<Text>().text = "What do you selling in here ?";
        UpdateRerollButtonUI(); // ← GỌI ở đây
    }

    void UpdateRerollButtonUI()
    {
        Transform titleText = button2.transform.Find("Text_Title");
        Transform costText = button2.transform.Find("Text_Cost");

        if (titleText != null)
            titleText.GetComponent<Text>().text = "Reroll Shop For";

        if (costText != null)
            costText.GetComponent<Text>().text = $"{GetCurrentRerollPrice()}g";
    }
    public void OnChooseOption1()
    {
        choicePanel.SetActive(false);
        currentDialogue = aboutShopDialogue;
        currentLineIndex = 0;
        StartTyping(currentDialogue[currentLineIndex]);
    }

    public void OnChooseOption2()
    {
        choicePanel.SetActive(false);

        int cost = GetCurrentRerollPrice();

        Debug.Log($"[Before Reroll] Player Gold: {player.Gold}, Cost: {cost}");

        if (player.Gold >= cost)
        {
            player.Gold -= cost;
            player.UpdateGoldUI();

            rerollCount++;
            shopRandom.RerollItems();

            currentDialogue = rerollDialogue;
            UpdateRerollButtonUI();

            Debug.Log($"[After Reroll] Player Gold: {player.Gold}");
        }
        else
        {
            currentDialogue = new string[] { "You don't have enough gold, Adventure" };
        }

        currentLineIndex = 0;
        StartTyping(currentDialogue[currentLineIndex]);
    }


    int GetCurrentRerollPrice()
    {
        return 100 + rerollCount * 50;
    }

    void EndDialogue()
    {
        isTalking = false;
        dialogueUI.SetActive(false);
        choicePanel.SetActive(false);
        dialogueText.text = "";
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
