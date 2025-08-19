using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SkillSelectorManager : MonoBehaviour
{
    public static SkillSelectorManager Instance { get; private set; }

    [Header("Skill Data")]
    public List<Skill> allSkills;
    public Button[] skillButtons; // Buttons for skill choices, similar to relicButtons
    private Skill[] currentSkillChoices = new Skill[4];

    [Header("Skill Inventory")]
    public Image[] skillSlots; // Slots in the main inventory UI to display skill icons (max 2)
    public Image[] skillCanvasSlots; // Slots in the skill selector canvas (if separate)
    public List<Skill> playerSkills = new List<Skill>(); // Max 2 skills

    [Header("UI Panel")]
    public GameObject skillCanvasUI; // New canvas for Skill Selector, similar to canvasUI

    [Header("Skill Tooltip")]
    public GameObject skillTooltipPanel;
    public TextMeshProUGUI tooltipNameText;
    public Image tooltipIcon;
    public TextMeshProUGUI tooltipEffectText; // If skills have descriptions
    // Add rarity if skills have it, otherwise omit

    //[Header("Rarity Spawn Chances (0-100)")] // Assuming skills have rarity like relics; adjust if not
    //[Range(0, 100)] public int chanceCommon = 50;
    //[Range(0, 100)] public int chanceEpic = 30;
    //[Range(0, 100)] public int chanceLegendary = 20;

    private int selectedSkillIndex = 0;
    private int activeSkillCount = 0;
    private bool skillCanvasActive = false;
    private bool skillSelected = false;
    private bool skillConfirming = false;
    private bool skillLocked = false;
    private bool pendingSkillChoose = false;
    private HashSet<Skill> usedSkills = new HashSet<Skill>();

    private bool waitingForReplace = false;
    private int selectedReplaceIndex = 0;
    private bool clickOnce = false;
    private Skill skillToReplace;

    private SkillManager skillManager; // Reference to SkillManager to assign skills

    void Awake()
    {
        Instance = this;
        skillManager = FindObjectOfType<SkillManager>(); // Get SkillManager reference
    }

    void Start()
    {
        if (skillCanvasUI != null)
        {
            skillCanvasUI.SetActive(false);
            skillCanvasActive = false;
        }
        UpdateSkillInventoryUI();
    }

    void Update()
    {
        if (waitingForReplace)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                waitingForReplace = false;
                skillSelected = false;
                skillConfirming = false;
                pendingSkillChoose = false;
                skillLocked = false;
                RestoreAllSkillsAlpha();
                SelectSkill(selectedSkillIndex);

                for (int i = 0; i < skillSlots.Length; i++)
                    skillSlots[i].transform.DOScale(Vector3.one, 0.2f);
                return;
            }
            HandleReplaceInput();
            return;
        }

        if (!skillCanvasActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (skillConfirming || pendingSkillChoose)
            {
                skillConfirming = false;
                skillLocked = false;
                pendingSkillChoose = false;
                skillSelected = false;
                RestoreAllSkillsAlpha();
                SelectSkill(selectedSkillIndex);
                return;
            }
        }

        if (!skillConfirming && !skillLocked)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                SelectSkill((selectedSkillIndex - 1 + activeSkillCount) % activeSkillCount);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                SelectSkill((selectedSkillIndex + 1) % activeSkillCount);
        }

        if (!skillConfirming && !skillLocked && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < activeSkillCount; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(skillButtons[i].GetComponent<RectTransform>(), mousePos))
                {
                    SelectSkill(i);
                    skillConfirming = true;
                    skillLocked = true;
                    FadeOtherSkills(i);
                    pendingSkillChoose = true;
                    return;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (skillConfirming && pendingSkillChoose)
            {
                HandleSkillSelection(currentSkillChoices[selectedSkillIndex]);
                pendingSkillChoose = false;
            }
            else if (!skillConfirming)
            {
                skillConfirming = true;
                skillLocked = true;
                FadeOtherSkills(selectedSkillIndex);
                pendingSkillChoose = true;
            }
        }

        if (skillConfirming && pendingSkillChoose && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < activeSkillCount; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(skillButtons[i].GetComponent<RectTransform>(), mousePos))
                {
                    if (i == selectedSkillIndex)
                    {
                        HandleSkillSelection(currentSkillChoices[i]);
                        pendingSkillChoose = false;
                        return;
                    }
                }
            }
        }
    }

    public void ShowSkillTooltip(int index, Vector3 position)
    {
        if (index >= 0 && index < playerSkills.Count)
        {
            Skill skill = playerSkills[index];

            tooltipNameText.text = skill.skillName;
            tooltipIcon.sprite = skill.icon;
            tooltipEffectText.text = "Cooldown: " + skill.cooldown + "s"; // Adjust if skills have descriptions

            skillTooltipPanel.SetActive(true);
            skillTooltipPanel.transform.position = position + new Vector3(310f, 50);
        }
    }

    public void HideSkillTooltip()
    {
        skillTooltipPanel.SetActive(false);
    }

    void HandleSkillSelection(Skill skill)
    {
        if (playerSkills.Count < 2)
        {
            playerSkills.Add(skill);
            usedSkills.Add(skill);
            AssignSkillToSlot(playerSkills.Count - 1);
            UpdateSkillInventoryUI();
            skillSelected = true;
            StartCoroutine(CloseCanvasAfterDelay(1f));
        }
        else
        {
            skillToReplace = skill;
            skillSelected = true;
            skillConfirming = false;
            pendingSkillChoose = false;
            skillLocked = false;
            waitingForReplace = true;
            selectedReplaceIndex = 0;
            ChangeReplaceSelection(0);
        }
    }

    void HandleReplaceInput()
    {
        // Assuming 2 slots, adjust navigation for 1 row x 2 columns or as per UI
        int totalSlots = 2;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex - 1 + totalSlots) % totalSlots);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex + 1) % totalSlots);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < skillSlots.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(skillSlots[i].rectTransform, mousePos))
                {
                    if (i == selectedReplaceIndex && (clickOnce || skillConfirming))
                    {
                        ReplaceSlotConfirmed();
                    }
                    else
                    {
                        ChangeReplaceSelection(i);
                        clickOnce = true;
                    }
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            ReplaceSlotConfirmed();
        }
    }

    void ChangeReplaceSelection(int index)
    {
        skillSlots[selectedReplaceIndex].transform.DOScale(Vector3.one, 0.2f);
        selectedReplaceIndex = index;
        skillSlots[selectedReplaceIndex].transform.DOScale(Vector3.one * 1.2f, 0.2f);
        clickOnce = false;
    }

    void ReplaceSlotConfirmed()
    {
        if (selectedReplaceIndex >= 0 && selectedReplaceIndex < playerSkills.Count)
        {
            Skill oldSkill = playerSkills[selectedReplaceIndex];
            playerSkills[selectedReplaceIndex] = skillToReplace;
            usedSkills.Add(skillToReplace);
            AssignSkillToSlot(selectedReplaceIndex);
            UpdateSkillInventoryUI();
        }
        waitingForReplace = false;
        clickOnce = false;
        skillSelected = false;
        skillConfirming = false;
        pendingSkillChoose = false;
        StartCoroutine(CloseCanvasAfterDelay(1f));
    }

    public void OpenSkillCanvas() // Call this when picking up a skill item
    {
        skillCanvasUI.SetActive(true);
        skillCanvasActive = true;
        skillSelected = false;
        skillConfirming = false;
        skillLocked = false;
        pendingSkillChoose = false;
        ShowRandomSkills();
        UpdateSkillInventoryUI();
        SelectSkill(0);
    }

    public bool IsSkillCanvasActive()
    {
        return skillCanvasActive;
    }

    void ShowRandomSkills()
    {
        List<Skill> availableSkills = new List<Skill>();

        // Lấy tất cả skill chưa được sử dụng
        foreach (Skill s in allSkills)
        {
            if (!usedSkills.Contains(s))
            {
                availableSkills.Add(s);
            }
        }

        // Chọn ngẫu nhiên 3 skill (hoặc ít hơn nếu không đủ)
        int targetSlots = Mathf.Min(3, availableSkills.Count);
        List<Skill> selected = new List<Skill>();

        for (int i = 0; i < targetSlots; i++)
        {
            if (availableSkills.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSkills.Count);
                selected.Add(availableSkills[randomIndex]);
                availableSkills.RemoveAt(randomIndex);
            }
        }

        activeSkillCount = selected.Count;

        // Cập nhật UI cho các button skill
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < activeSkillCount)
            {
                var skill = selected[i];
                currentSkillChoices[i] = skill;

                skillButtons[i].transform.Find("Icon1").GetComponent<Image>().sprite = skill.icon;
                skillButtons[i].transform.Find("NameText1").GetComponent<TextMeshProUGUI>().text = skill.skillName;
                skillButtons[i].transform.Find("EffectText1").GetComponent<TextMeshProUGUI>().text = "Cooldown: " + skill.cooldown + "s";

                skillButtons[i].gameObject.SetActive(true);
                skillButtons[i].interactable = true;
                skillButtons[i].transform.localScale = Vector3.one;

                CanvasGroup cg = skillButtons[i].GetComponent<CanvasGroup>();
                if (cg == null) cg = skillButtons[i].gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 1f;
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
                currentSkillChoices[i] = null;
            }
        }
    }

    void SelectSkill(int index)
    {
        // Similar to SelectRelic
        selectedSkillIndex = index;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].transform.DOScale(Vector3.one, 0.2f);
        }
        skillButtons[selectedSkillIndex].transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    void FadeOtherSkills(int exceptIndex)
    {
        // Similar to FadeOtherRelics
    }

    void RestoreAllSkillsAlpha()
    {
        // Similar to RestoreAllRelicsAlpha
    }

    IEnumerator CloseCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseCanvasImmediate();
    }

    void CloseCanvasImmediate()
    {
        skillCanvasUI.SetActive(false);
        skillTooltipPanel.SetActive(false);
        skillCanvasActive = false;
        skillSelected = false;
        skillConfirming = false;
        pendingSkillChoose = false;
    }

    public void UpdateSkillInventoryUI()
    {
        // Similar to UpdateInventoryUI, but for skills
        if (skillSlots != null)
        {
            for (int i = 0; i < skillSlots.Length; i++)
            {
                if (i < playerSkills.Count)
                {
                    skillSlots[i].sprite = playerSkills[i].icon;
                    skillSlots[i].color = Color.white;
                }
                else
                {
                    skillSlots[i].sprite = null;
                    skillSlots[i].color = new Color(0, 0, 0, 0);
                }
            }
        }

        // Update skillCanvasSlots if separate
    }

    private void AssignSkillToSlot(int index)
    {
        if (index == 0 && skillManager != null)
        {
            // Assuming SkillState needs to be updated with the Skill
            // You may need to modify SkillState to hold/reference the Skill ScriptableObject
            // e.g., skillManager.skillSlot1.SetSkill(playerSkills[0]);
        }
        else if (index == 1 && skillManager != null)
        {
            // skillManager.skillSlot2.SetSkill(playerSkills[1]);
        }
    }

    // Add methods like RemoveSkill, etc., if needed
}