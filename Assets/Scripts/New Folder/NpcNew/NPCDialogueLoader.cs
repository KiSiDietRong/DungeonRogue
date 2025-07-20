using UnityEngine;
using TMPro;
using System.IO;

public class NPCDialogueLoader : MonoBehaviour
{
    public string npcName;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public float wordSpeed = 0.05f;

    private string[] currentLines;
    private int index = 0;

    void Start()
    {
        LoadDialogue();
        Invoke("StartDialogue", 2f); // Delay 2s trước khi bắt đầu
    }

    void LoadDialogue()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "npc_dialogue.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DialogueContainer container = JsonUtility.FromJson<DialogueContainer>(json);
            foreach (var d in container.dialogues)
            {
                if (d.npcName == npcName)
                {
                    currentLines = d.lines;
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy file JSON!");
        }
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    System.Collections.IEnumerator TypeLine()
    {
        dialogueText.text = "";
        foreach (char c in currentLines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }
}
