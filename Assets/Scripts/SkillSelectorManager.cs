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

    [Header("Shared UI Elements")]
    public Image[] relicSlots; // 12 ô trắng để chứa relic (shared with relic selector/inventory)
    public Image[] skillSlots; // 2 ô để chứa skill (shared)
    public TextMeshProUGUI hpText; // Text để hiển thị HP
    public TextMeshProUGUI dmgText; // Text để hiển thị DMG
    public TextMeshProUGUI critText; // Text để hiển thị CRIT
    public Image weaponIcon; // Ô trắng icon weapon

    [Header("UI Panel")]
    public GameObject skillCanvasUI; // Canvas for Skill Selector

    [Header("Skill Tooltip")]
    public GameObject skillTooltipPanel;
    public TextMeshProUGUI tooltipNameText;
    public Image tooltipIcon;
    public TextMeshProUGUI tooltipEffectText; // If skills have descriptions

    private int selectedSkillIndex = 0;
    private int activeSkillCount = 0;
    private bool skillCanvasActive = false;
    private bool skillSelected = false;
    private bool skillConfirming = false;
    private bool skillLocked = false;
    private bool pendingSkillChoose = false;
    private HashSet<Skill> usedSkills = new HashSet<Skill>();
    public List<Skill> playerSkills = new List<Skill>(); // Thêm lại danh sách kỹ năng của người chơi

    private bool waitingForReplace = false;
    private int selectedReplaceIndex = 0;
    private bool clickOnce = false;
    private Skill skillToReplace;

    private SkillManager skillManager; // Reference to SkillManager to assign skills
    private PlayerHealth playerHealth; // Tham chiếu đến PlayerHealth
    private ActiveWeapon activeWeapon; // Tham chiếu đến ActiveWeapon

    void Awake()
    {
        Instance = this;
        skillManager = FindObjectOfType<SkillManager>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        activeWeapon = FindObjectOfType<ActiveWeapon>();
        if (skillManager == null)
        {
            Debug.LogError("SkillManager not found in scene!");
        }
    }

    void Start()
    {
        if (skillCanvasUI != null)
        {
            skillCanvasUI.SetActive(false);
            skillCanvasActive = false;
        }
        UpdateUI(); // Cập nhật UI ban đầu
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
            tooltipEffectText.text = "Cooldown: " + skill.cooldown + "s";

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
            AssignSkillToManager(playerSkills.Count - 1); // Gán skill vào SkillManager
            UpdateUI();
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
            AssignSkillToManager(selectedReplaceIndex); // Gán skill mới vào SkillManager
            UpdateUI();
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
        UpdateUI();
        SelectSkill(0);
    }

    public bool IsSkillCanvasActive()
    {
        return skillCanvasActive;
    }

    void ShowRandomSkills()
    {
        List<Skill> availableSkills = new List<Skill>();

        foreach (Skill s in allSkills)
        {
            if (!usedSkills.Contains(s))
            {
                availableSkills.Add(s);
            }
        }

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
        selectedSkillIndex = index;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].transform.DOScale(Vector3.one, 0.2f);
        }
        skillButtons[selectedSkillIndex].transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    void FadeOtherSkills(int exceptIndex)
    {
        for (int i = 0; i < activeSkillCount; i++)
        {
            if (i != exceptIndex)
            {
                CanvasGroup cg = skillButtons[i].GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = skillButtons[i].gameObject.AddComponent<CanvasGroup>();
                }
                cg.alpha = 0.3f;
            }
        }
    }

    void RestoreAllSkillsAlpha()
    {
        for (int i = 0; i < activeSkillCount; i++)
        {
            CanvasGroup cg = skillButtons[i].GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1f;
            }
        }
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

    public void UpdateUI() // Cập nhật toàn bộ UI chung (relics, skills, stats, weapon)
    {
        // Cập nhật relics từ InventoryManager (đồng bộ)
        if (InventoryManager.Instance != null && relicSlots != null)
        {
            List<Relic> playerInventory = InventoryManager.Instance.playerInventory;
            for (int i = 0; i < relicSlots.Length; i++)
            {
                relicSlots[i].transform.DOScale(Vector3.one, 0.1f);

                if (i < playerInventory.Count)
                {
                    relicSlots[i].sprite = playerInventory[i].icon;
                    relicSlots[i].color = Color.white;
                }
                else
                {
                    relicSlots[i].sprite = null;
                    relicSlots[i].color = new Color(0, 0, 0, 0);
                }
            }
        }

        // Cập nhật skills
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

        // Cập nhật icon vũ khí
        if (activeWeapon != null && activeWeapon.CurrentActiveWeapon != null)
        {
            WeaponInfo weaponInfo = activeWeapon.CurrentActiveWeapon.GetWeaponInfo();
            if (weaponInfo != null && weaponInfo.weaponSprite != null)
            {
                if (weaponIcon != null)
                {
                    weaponIcon.sprite = weaponInfo.weaponSprite;
                    weaponIcon.color = Color.white;
                }
            }
            else
            {
                if (weaponIcon != null)
                {
                    weaponIcon.sprite = null;
                    weaponIcon.color = new Color(0, 0, 0, 0);
                }
            }
        }
        else
        {
            if (weaponIcon != null)
            {
                weaponIcon.sprite = null;
                weaponIcon.color = new Color(0, 0, 0, 0);
            }
        }

        // Cập nhật chỉ số player
        if (playerHealth != null)
        {
            if (hpText != null)
                hpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
        else
        {
            if (hpText != null)
                hpText.text = "N/A";
        }

        if (activeWeapon != null && activeWeapon.CurrentActiveWeapon != null)
        {
            WeaponInfo weaponInfo = activeWeapon.CurrentActiveWeapon.GetWeaponInfo();
            if (weaponInfo != null)
            {
                if (dmgText != null)
                    dmgText.text = $"{weaponInfo.weaponDamage}";
                if (critText != null)
                    critText.text = $"{(weaponInfo.criticalChance * 100)}%";
            }
            else
            {
                if (dmgText != null)
                    dmgText.text = "N/A";
                if (critText != null)
                    critText.text = "N/A";
            }
        }
        else
        {
            if (dmgText != null)
                dmgText.text = "N/A";
            if (critText != null)
                critText.text = "N/A";
        }
    }

    private void AssignSkillToManager(int index)
    {
        if (skillManager != null)
        {
            if (index >= 0 && index < playerSkills.Count)
            {
                Skill selectedSkill = playerSkills[index];
                SkillState newSkillState = new SkillState();
                newSkillState.skill = selectedSkill; // Gán skill vào SkillState
                skillManager.SetSkill(newSkillState, index + 1); // Gán vào slot 1 hoặc 2
                Debug.Log($"Assigned skill {selectedSkill.skillName} to slot {index + 1} in SkillManager");
            }
            else
            {
                Debug.LogError($"Invalid skill index {index} in AssignSkillToManager");
            }
        }
        else
        {
            Debug.LogError("SkillManager is null in SkillSelectorManager!");
        }
    }

    // Thêm phương thức để đồng bộ skill với InventoryManager
    public void SyncSkillsWithInventory()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UpdateInventoryUI(); // Cập nhật UI inventory để hiển thị skill
        }
    }
}