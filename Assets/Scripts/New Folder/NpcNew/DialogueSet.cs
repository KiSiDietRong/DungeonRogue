[System.Serializable]
public class DialogueSet
{
    public string npcName;
    public string[] lines;
}

[System.Serializable]
public class DialogueContainer
{
    public DialogueSet[] dialogues;
}
