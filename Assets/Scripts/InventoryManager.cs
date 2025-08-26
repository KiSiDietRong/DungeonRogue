using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Relic Data")]
    public List<Relic> allRelics;
    public Button[] relicButtons;
    private Relic[] currentRelicChoices = new Relic[4];

    [Header("Inventory")]
    public Image[] inventorySlotsSelect;
    public Image[] inventoryCanvasSlots;
    public List<Relic> playerInventory = new List<Relic>();
    public Image[] skillSlots;
    public Image[] skillSlotsSelect;

    [Header("UI Panel")]
    public GameObject relicSelect;
    public GameObject inventory;

    [Header("Relic Infor Select")]
    public GameObject relicTooltipPanel;
    public TextMeshProUGUI tooltipNameText;
    public Image tooltipIcon;
    public TextMeshProUGUI tooltipEffectText;
    public TextMeshProUGUI tooltipRarityText;

    [Header("Relic Infor Inventory")]
    public GameObject inforRelic;
    public TextMeshProUGUI NameText;
    public Image Icon;
    public TextMeshProUGUI EffectText;
    public TextMeshProUGUI RarityText;

    [Header("Player Stats UI Select")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI critText;
    public Image weaponIcon;

    [Header("Player Stats UI Inventory")]
    public TextMeshProUGUI inventoryHpText;
    public TextMeshProUGUI inventoryDmgText;
    public TextMeshProUGUI inventoryCritText;
    public Image inventoryWeaponIcon;

    [Header("Rarity Spawn Chances (0-100)")]
    [Range(0, 100)] public int chanceCommon = 50;
    [Range(0, 100)] public int chanceEpic = 30;
    [Range(0, 100)] public int chanceLegendary = 20;

    private int selectedRelicIndex = 0;
    private int activeRelicCount = 0;
    private bool canvasActive = false;
    private bool inventoryCanvasActive = false;
    private bool relicSelected = false;
    private bool inventoryOnlyView = false;
    private bool isPickingRelic = false;
    public HashSet<Relic> usedRelics = new HashSet<Relic>();

    private bool waitingForReplace = false;
    private int selectedReplaceIndex = 0;
    private bool clickOnce = false;
    private bool relicConfirming = false;
    private bool relicLocked = false;
    private bool pendingRelicChoose = false;
    private Relic relicToReplace;

    private int titansWargearKillCount = 0;
    private PlayerHealth playerHealth;
    private ActiveWeapon activeWeapon;

    void Awake()
    {
        Instance = this;
        Enemy.OnEnemyDeath += HandleEnemyDeath;
        playerHealth = FindObjectOfType<PlayerHealth>();
        activeWeapon = FindObjectOfType<ActiveWeapon>();
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    void Start()
    {
        if (inventory != null)
        {
            inventory.SetActive(false);
            inventoryCanvasActive = false;
        }
        if (relicSelect != null)
        {
            relicSelect.SetActive(false);
            canvasActive = false;
        }
        UpdateShopPrices();
    }

    void Update()
    {
        if (waitingForReplace)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                waitingForReplace = false;
                relicSelected = false;
                relicConfirming = false;
                pendingRelicChoose = false;
                relicLocked = false;
                RestoreAllRelicsAlpha();
                SelectRelic(selectedRelicIndex);

                for (int i = 0; i < inventorySlotsSelect.Length; i++)
                    inventorySlotsSelect[i].transform.DOScale(Vector3.one, 0.2f);
                return;
            }
            HandleReplaceInput();
            return;
        }

        if (!isPickingRelic && !relicConfirming && !pendingRelicChoose && !waitingForReplace && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryOnlyUI();
            return;
        }

        if (!canvasActive) return;
        if (inventoryOnlyView) return;
        if (relicSelected) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryOnlyView)
            {
                inventoryOnlyView = false;
                relicSelect.SetActive(true);
                relicSelected = false;
                relicConfirming = true;
                pendingRelicChoose = true;
                relicLocked = true;
                RestoreAllRelicsAlpha();
                SelectRelic(selectedRelicIndex);
                return;
            }

            if (relicConfirming || pendingRelicChoose)
            {
                relicConfirming = false;
                relicLocked = false;
                pendingRelicChoose = false;
                relicSelected = false;
                RestoreAllRelicsAlpha();
                SelectRelic(selectedRelicIndex);
                return;
            }
        }

        if (!relicConfirming && !relicLocked)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                SelectRelic((selectedRelicIndex - 1 + activeRelicCount) % activeRelicCount);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                SelectRelic((selectedRelicIndex + 1) % activeRelicCount);
        }

        if (!relicConfirming && !relicLocked && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < activeRelicCount; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(relicButtons[i].GetComponent<RectTransform>(), mousePos))
                {
                    SelectRelic(i);
                    relicConfirming = true;
                    relicLocked = true;
                    FadeOtherRelics(i);
                    pendingRelicChoose = true;
                    return;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (relicConfirming && pendingRelicChoose)
            {
                HandleRelicSelection(currentRelicChoices[selectedRelicIndex]);
                pendingRelicChoose = false;
            }
            else if (!relicConfirming)
            {
                relicConfirming = true;
                relicLocked = true;
                FadeOtherRelics(selectedRelicIndex);
                pendingRelicChoose = true;
            }
        }

        if (relicConfirming && pendingRelicChoose && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < activeRelicCount; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(relicButtons[i].GetComponent<RectTransform>(), mousePos))
                {
                    if (i == selectedRelicIndex)
                    {
                        HandleRelicSelection(currentRelicChoices[i]);
                        pendingRelicChoose = false;
                        return;
                    }
                }
            }
        }
    }

    public void ShowRelicTooltip(int index, Vector3 position)
    {
        if (index >= 0 && index < playerInventory.Count)
        {
            Relic relic = playerInventory[index];

            tooltipNameText.text = relic.relicName;
            tooltipIcon.sprite = relic.icon;
            tooltipEffectText.text = relic.effectDescription;
            tooltipRarityText.text = relic.rarity;

            switch (relic.rarityType)
            {
                case Rarity.Common: tooltipRarityText.color = Color.white; break;
                case Rarity.Epic: tooltipRarityText.color = new Color(0.6f, 0.2f, 1f); break;
                case Rarity.Legendary: tooltipRarityText.color = Color.yellow; break;
            }

            relicTooltipPanel.SetActive(true);
            relicTooltipPanel.transform.position = position + new Vector3(310f, 50);
        }

        if (index >= 0 && index < playerInventory.Count)
        {
            Relic relic = playerInventory[index];

            NameText.text = relic.relicName;
            Icon.sprite = relic.icon;
            EffectText.text = relic.effectDescription;
            RarityText.text = relic.rarity;

            switch (relic.rarityType)
            {
                case Rarity.Common: RarityText.color = Color.white; break;
                case Rarity.Epic: RarityText.color = new Color(0.6f, 0.2f, 1f); break;
                case Rarity.Legendary: RarityText.color = Color.yellow; break;
            }

            inforRelic.SetActive(true);
            inforRelic.transform.position = position + new Vector3(310f, 50);
        }
    }

    public void HideRelicTooltip()
    {
        relicTooltipPanel.SetActive(false);
        inforRelic.SetActive(false);
    }

    void HandleRelicSelection(Relic relic)
    {
        if (playerInventory.Count < inventorySlotsSelect.Length)
        {
            playerInventory.Add(relic);
            usedRelics.Add(relic);
            UpdateInventoryUI();
            ApplyRelicEffect(relic);
            relicSelected = true;
            StartCoroutine(CloseCanvasAfterDelay(1f));
        }
        else
        {
            relicToReplace = relic;
            relicSelected = true;
            relicConfirming = false;
            pendingRelicChoose = false;
            relicLocked = false;
            waitingForReplace = true;
            selectedReplaceIndex = 0;
            ChangeReplaceSelection(0);
        }
        if (SkillSelectorManager.Instance != null)
        {
            SkillSelectorManager.Instance.SyncSkillsWithInventory();
        }
    }

    void HandleReplaceInput()
    {
        int columns = 4;
        int rows = 3;
        int totalSlots = columns * rows;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex % columns == 0) ? selectedReplaceIndex + (columns - 1) : selectedReplaceIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex % columns == columns - 1) ? selectedReplaceIndex - (columns - 1) : selectedReplaceIndex + 1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex - columns + totalSlots) % totalSlots);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeReplaceSelection((selectedReplaceIndex + columns) % totalSlots);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < inventorySlotsSelect.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(inventorySlotsSelect[i].rectTransform, mousePos))
                {
                    if (i == selectedReplaceIndex && (clickOnce || relicConfirming))
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
        inventorySlotsSelect[selectedReplaceIndex].transform.DOScale(Vector3.one, 0.2f);
        selectedReplaceIndex = index;
        inventorySlotsSelect[selectedReplaceIndex].transform.DOScale(Vector3.one * 1.2f, 0.2f);
        clickOnce = false;
    }

    void ReplaceSlotConfirmed()
    {
        if (selectedReplaceIndex >= 0 && selectedReplaceIndex < playerInventory.Count)
        {
            Relic oldRelic = playerInventory[selectedReplaceIndex];
            playerInventory[selectedReplaceIndex] = relicToReplace;
            usedRelics.Add(relicToReplace);
            UpdateInventoryUI();
            ApplyRelicEffect(relicToReplace);
            UpdateArmorTextVisibility();
            if (oldRelic.type == RelicType.DiscountCard)
            {
                UpdateShopPrices();
            }
        }
        else
        {
            playerInventory.Add(relicToReplace);
            usedRelics.Add(relicToReplace);
            UpdateInventoryUI();
            ApplyRelicEffect(relicToReplace);
            UpdateArmorTextVisibility();
        }
        waitingForReplace = false;
        clickOnce = false;
        isPickingRelic = false;
        relicSelected = false;
        relicConfirming = false;
        pendingRelicChoose = false;
        StartCoroutine(CloseCanvasAfterDelay(1f));
        if (SkillSelectorManager.Instance != null)
        {
            SkillSelectorManager.Instance.SyncSkillsWithInventory();
        }
    }

    public void OpenCanvas()
    {
        if (inventory != null)
        {
            inventory.SetActive(false);
            inventoryCanvasActive = false;
        }

        relicSelect.SetActive(true);
        canvasActive = true;
        isPickingRelic = true;
        relicSelected = false;
        inventoryOnlyView = false;
        relicConfirming = false;
        relicLocked = false;
        pendingRelicChoose = false;
        ShowRandomRelics();
        UpdateInventoryUI();
        SelectRelic(0);
        if (SkillSelectorManager.Instance != null)
        {
            SkillSelectorManager.Instance.SyncSkillsWithInventory();
        }
    }

    public void ToggleInventoryOnlyUI()
    {
        if (relicSelect != null)
        {
            relicSelect.SetActive(false);
            canvasActive = false;
        }

        if (inventory != null)
        {
            bool active = !inventory.activeSelf;
            inventory.SetActive(active);
            inventoryCanvasActive = active;
            inventoryOnlyView = active;
            relicSelected = false;
            UpdateInventoryUI();
            UpdateArmorTextVisibility();

            foreach (var btn in relicButtons)
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    public bool IsCanvasActive()
    {
        return canvasActive || inventoryCanvasActive;
    }

    void ShowRandomRelics()
    {
        List<Relic> commons = new List<Relic>();
        List<Relic> epics = new List<Relic>();
        List<Relic> legendaries = new List<Relic>();

        foreach (Relic r in allRelics)
        {
            if (usedRelics.Contains(r)) continue;

            switch (r.rarityType)
            {
                case Rarity.Common: commons.Add(r); break;
                case Rarity.Epic: epics.Add(r); break;
                case Rarity.Legendary: legendaries.Add(r); break;
            }
        }

        List<Relic> selected = new List<Relic>();
        Relic highRarityRelic = null;

        int totalAvailableRelics = commons.Count + epics.Count + legendaries.Count;

        int roll = Random.Range(0, 100);

        if (roll < chanceLegendary && legendaries.Count > 0)
        {
            highRarityRelic = legendaries[Random.Range(0, legendaries.Count)];
            legendaries.Remove(highRarityRelic);
        }
        else if (roll < (chanceLegendary + chanceEpic) && epics.Count > 0)
        {
            highRarityRelic = epics[Random.Range(0, epics.Count)];
            epics.Remove(highRarityRelic);
        }

        int targetSlots = totalAvailableRelics >= 3 ? 3 : totalAvailableRelics;
        if (highRarityRelic != null)
            targetSlots = Mathf.Min(4, totalAvailableRelics);

        int commonSlots = Mathf.Min(3, commons.Count);
        while (selected.Count < commonSlots && commons.Count > 0)
        {
            Relic r = commons[Random.Range(0, commons.Count)];
            selected.Add(r);
            commons.Remove(r);
        }

        while (selected.Count < targetSlots && (epics.Count > 0 || legendaries.Count > 0))
        {
            int subRoll = Random.Range(0, 100);
            if (subRoll < chanceLegendary && legendaries.Count > 0)
            {
                Relic r = legendaries[Random.Range(0, legendaries.Count)];
                selected.Add(r);
                legendaries.Remove(r);
            }
            else if (epics.Count > 0)
            {
                Relic r = epics[Random.Range(0, epics.Count)];
                selected.Add(r);
                epics.Remove(r);
            }
            else if (legendaries.Count > 0)
            {
                Relic r = legendaries[Random.Range(0, legendaries.Count)];
                selected.Add(r);
                legendaries.Remove(r);
            }
        }

        if (highRarityRelic != null && selected.Count >= 3)
        {
            selected.Add(highRarityRelic);
        }
        else if (highRarityRelic != null && selected.Count < 3)
        {
            selected.Add(highRarityRelic);
        }

        int buttonsToShow = selected.Count >= 3 ? (highRarityRelic != null ? 4 : 3) : selected.Count;
        activeRelicCount = buttonsToShow;

        for (int i = 0; i < relicButtons.Length; i++)
        {
            if (i < activeRelicCount && i < selected.Count)
            {
                var relic = selected[i];
                currentRelicChoices[i] = relic;

                relicButtons[i].transform.Find("Icon").GetComponent<Image>().sprite = relic.icon;
                relicButtons[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = relic.relicName;
                relicButtons[i].transform.Find("EffectText").GetComponent<TextMeshProUGUI>().text = relic.effectDescription;

                TextMeshProUGUI rarityText = relicButtons[i].transform.Find("RarityText").GetComponent<TextMeshProUGUI>();
                rarityText.text = relic.rarity;

                switch (relic.rarityType)
                {
                    case Rarity.Common: rarityText.color = Color.white; break;
                    case Rarity.Epic: rarityText.color = new Color(0.6f, 0.2f, 1f); break;
                    case Rarity.Legendary: rarityText.color = Color.yellow; break;
                }

                relicButtons[i].gameObject.SetActive(true);
                relicButtons[i].interactable = true;
                relicButtons[i].transform.localScale = Vector3.one;

                CanvasGroup cg = relicButtons[i].GetComponent<CanvasGroup>();
                if (cg == null) cg = relicButtons[i].gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 1f;
            }
            else
            {
                relicButtons[i].gameObject.SetActive(false);
                currentRelicChoices[i] = null;
            }
        }
    }

    void SelectRelic(int index)
    {
        if (index < 0 || index >= activeRelicCount)
            return;

        selectedRelicIndex = index;

        for (int i = 0; i < relicButtons.Length; i++)
        {
            relicButtons[i].transform.DOScale(Vector3.one, 0.2f);
        }

        relicButtons[selectedRelicIndex].transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    void FadeOtherRelics(int exceptIndex)
    {
        for (int i = 0; i < activeRelicCount; i++)
        {
            if (i != exceptIndex)
            {
                CanvasGroup cg = relicButtons[i].GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = relicButtons[i].gameObject.AddComponent<CanvasGroup>();
                }
                cg.alpha = 0.3f;
            }
        }
    }

    void RestoreAllRelicsAlpha()
    {
        for (int i = 0; i < activeRelicCount; i++)
        {
            CanvasGroup cg = relicButtons[i].GetComponent<CanvasGroup>();
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
        relicSelect.SetActive(false);
        relicTooltipPanel.SetActive(false);
        inforRelic.SetActive(false);
        canvasActive = false;
        isPickingRelic = false;
        relicSelected = false;
        relicConfirming = false;
        pendingRelicChoose = false;
    }

    public void UpdateInventoryUI()
    {
        if (inventorySlotsSelect != null)
        {
            for (int i = 0; i < inventorySlotsSelect.Length; i++)
            {
                inventorySlotsSelect[i].transform.DOScale(Vector3.one, 0.1f);

                if (i < playerInventory.Count)
                {
                    inventorySlotsSelect[i].sprite = playerInventory[i].icon;
                    inventorySlotsSelect[i].color = Color.white;
                }
                else
                {
                    inventorySlotsSelect[i].sprite = null;
                    inventorySlotsSelect[i].color = new Color(0, 0, 0, 0);
                }
            }
        }

        if (inventoryCanvasSlots != null)
        {
            for (int i = 0; i < inventoryCanvasSlots.Length; i++)
            {
                inventoryCanvasSlots[i].transform.DOScale(Vector3.one, 0.1f);

                if (i < playerInventory.Count)
                {
                    inventoryCanvasSlots[i].sprite = playerInventory[i].icon;
                    inventoryCanvasSlots[i].color = Color.white;
                }
                else
                {
                    inventoryCanvasSlots[i].sprite = null;
                    inventoryCanvasSlots[i].color = new Color(0, 0, 0, 0);
                }
            }
        }

        if (skillSlots != null)
        {
            var playerSkills = (SkillSelectorManager.Instance != null)
         ? SkillSelectorManager.Instance.playerSkills
         : new List<Skill>();
            for (int i = 0; i < skillSlots.Length; i++)
            {
                skillSlots[i].transform.DOScale(Vector3.one, 0.1f);

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

        if (skillSlotsSelect != null)
        {
            var playerSkills = (SkillSelectorManager.Instance != null)
        ? SkillSelectorManager.Instance.playerSkills
        : new List<Skill>();
            for (int i = 0; i < skillSlotsSelect.Length; i++)
            {
                skillSlotsSelect[i].transform.DOScale(Vector3.one, 0.1f);

                if (i < playerSkills.Count)
                {
                    skillSlotsSelect[i].sprite = playerSkills[i].icon;
                    skillSlotsSelect[i].color = Color.white;
                }
                else
                {
                    skillSlotsSelect[i].sprite = null;
                    skillSlotsSelect[i].color = new Color(0, 0, 0, 0);
                }
            }
        }

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
                if (inventoryWeaponIcon != null)
                {
                    inventoryWeaponIcon.sprite = weaponInfo.weaponSprite;
                    inventoryWeaponIcon.color = Color.white;
                }
            }
            else
            {
                if (weaponIcon != null)
                {
                    weaponIcon.sprite = null;
                    weaponIcon.color = new Color(0, 0, 0, 0);
                }
                if (inventoryWeaponIcon != null)
                {
                    inventoryWeaponIcon.sprite = null;
                    inventoryWeaponIcon.color = new Color(0, 0, 0, 0);
                }
                Debug.LogWarning("WeaponInfo or weaponSprite is null in ActiveWeapon!");
            }
        }
        else
        {
            if (weaponIcon != null)
            {
                weaponIcon.sprite = null;
                weaponIcon.color = new Color(0, 0, 0, 0);
            }
            if (inventoryWeaponIcon != null)
            {
                inventoryWeaponIcon.sprite = null;
                inventoryWeaponIcon.color = new Color(0, 0, 0, 0);
            }
            Debug.LogWarning("ActiveWeapon or CurrentActiveWeapon is null!");
        }
        UpdatePlayerStatsUI();
    }

    void UpdatePlayerStatsUI()
    {
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

        if (playerHealth != null)
        {
            if (inventoryHpText != null)
                inventoryHpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
        else
        {
            if (inventoryHpText != null)
                inventoryHpText.text = "N/A";
        }

        if (activeWeapon != null && activeWeapon.CurrentActiveWeapon != null)
        {
            WeaponInfo weaponInfo = activeWeapon.CurrentActiveWeapon.GetWeaponInfo();
            if (weaponInfo != null)
            {
                if (inventoryDmgText != null)
                    inventoryDmgText.text = $"{weaponInfo.weaponDamage}";
                if (inventoryCritText != null)
                    inventoryCritText.text = $"{(weaponInfo.criticalChance * 100)}%";
            }
            else
            {
                if (inventoryDmgText != null)
                    inventoryDmgText.text = "N/A";
                if (inventoryCritText != null)
                    inventoryCritText.text = "N/A";
            }
        }
        else
        {
            if (inventoryDmgText != null)
                inventoryDmgText.text = "N/A";
            if (inventoryCritText != null)
                inventoryCritText.text = "N/A";
        }
    }

    public void ApplyRelicEffect(Relic relic)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        switch (relic.type)
        {
            case RelicType.BobsContainmentField:
                Debug.Log("Bob's Containment Field equipped: First skill use in each room grants a 5-second shield blocking all enemy damage.");
                break;
            case RelicType.EmpoweredBangle:
                Debug.Log("Empowered Bangle equipped: First skill use in each room increases weapon damage by 50% for 3 seconds.");
                break;
            case RelicType.BonePlate:
                if (health != null)
                {
                    health.AddArmor(15f);
                    health.SetArmorTextActive(true);
                }
                break;
            case RelicType.ConduitSpike:
                Debug.Log("Conduit Spike equipped: Every 3rd attack will deal 4-10 damage to 2 nearest enemies.");
                break;
            case RelicType.DiscountCard:
                Debug.Log("Discount Card equipped: Shop items are 25% cheaper.");
                UpdateShopPrices();
                break;
            case RelicType.DoomShell:
                Debug.Log("Doom Shell equipped: Dash deals 5-10 damage to enemies in range.");
                break;
            case RelicType.RazorClaw:
                Debug.Log("Razor Claw equipped: Critical hits deal double damage.");
                break;
            case RelicType.SpiritShelter:
                Debug.Log("Spirit Shelter equipped: Revive with full HP on first death, then relic is removed.");
                break;
            case RelicType.RejuvenationGlove:
                Debug.Log("Rejuvenation Glove equipped: Critical hits heal 1 HP, up to max health.");
                break;
            case RelicType.JuicyOpal:
                Debug.Log("Juicy Opal equipped: Increases healing amount by 1 HP.");
                break;
            case RelicType.VoltClaw:
                Debug.Log("VoltClaw equipped: Critical hits deal additional 3-15 lightning damage to the enemy.");
                break;
            case RelicType.ArchangelsScythe:
                Debug.Log("ArchangelsScythe equipped: Healing HP deals damage to nearby enemies equal to 4x the healed amount.");
                break;
            case RelicType.TitansWargear:
                Debug.Log("Titan's Wargear equipped: Every 4 enemies killed increases weapon damage by 1.");
                break;
            case RelicType.DazeClaw:
                Debug.Log("DazeClaw equipped: Critical hits stun enemies for 1 second.");
                break;
            case RelicType.GiantMace:
                Debug.Log("GiantMace equipped: Deal 50% more damage to stunned enemies.");
                break;
            case RelicType.ImpactCharm:
                Debug.Log("ImpactCharm equipped: Whenever you stun an enemy, deal 15 AOE damage around it.");
                break;
            case RelicType.TraumaticBlow:
                Debug.Log("Traumatic Blow equipped: Instantly defeat enemies with less than 20% HP when stunned.");
                break;
            case RelicType.RecoveryRing:
                Debug.Log("Recovery Ring equipped: Heal 1 HP when entering a new battle map.");
                break;
            case RelicType.FieryImbuement:
                Debug.Log("Fiery Imbuement equipped: Using a skill causes the next 7 attacks to apply a burn effect for 3 seconds.");
                break;
        }
        UpdateInventoryUI();
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        if (playerInventory.Exists(relic => relic.type == RelicType.BonePlate))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.AddArmor(1f);
            }
        }

        if (playerInventory.Exists(relic => relic.type == RelicType.TitansWargear))
        {
            titansWargearKillCount++;
            if (titansWargearKillCount >= 4)
            {
                foreach (var projectile in FindObjectsOfType<Projectile>())
                {
                    WeaponInfo weaponInfo = projectile.GetWeaponInfo();
                    if (weaponInfo != null)
                    {
                        weaponInfo.weaponDamage += 1;
                        Debug.Log($"Titan's Wargear triggered: Increased {weaponInfo.name} damage to {weaponInfo.weaponDamage}.");
                    }
                }
                titansWargearKillCount = 0;
                UpdateInventoryUI();
            }
            Debug.Log($"Titan's Wargear: {titansWargearKillCount}/4 enemies killed.");
        }
    }

    public void UpdateArmorTextVisibility()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            bool hasBonePlate = playerInventory.Exists(relic => relic.type == RelicType.BonePlate);
            health.SetArmorTextActive(hasBonePlate);
        }
    }

    public void RemoveRelic(RelicType relicType)
    {
        Relic relicToRemove = playerInventory.Find(relic => relic.type == relicType);
        if (relicToRemove != null)
        {
            playerInventory.Remove(relicToRemove);
            usedRelics.Add(relicToRemove);
            UpdateInventoryUI();
            UpdateArmorTextVisibility();
            if (relicType == RelicType.DiscountCard)
            {
                UpdateShopPrices();
            }
            Debug.Log($"Removed relic: {relicToRemove.relicName}");
            if (relicType == RelicType.TitansWargear)
            {
                titansWargearKillCount = 0;
            }
        }
    }

    public bool IsRelicUsed(Relic relic)
    {
        return usedRelics.Contains(relic);
    }

    public void AddUsedRelic(Relic relic)
    {
        if (!usedRelics.Contains(relic))
            usedRelics.Add(relic);
    }

    public bool HasRecoveryRing()
    {
        return playerInventory.Exists(relic => relic.type == RelicType.RecoveryRing);
    }

    public bool HasBobsContainmentField()
    {
        return playerInventory.Exists(relic => relic.type == RelicType.BobsContainmentField);
    }

    public bool HasEmpoweredBangle()
    {
        return playerInventory.Exists(relic => relic.type == RelicType.EmpoweredBangle);
    }

    public bool HasFieryImbuement()
    {
        return playerInventory.Exists(relic => relic.type == RelicType.FieryImbuement);
    }

    public void UpdateShopPrices()
    {
        ShopRelicDisplay[] shopDisplays = FindObjectsOfType<ShopRelicDisplay>();
        if (shopDisplays.Length == 0)
        {
            Debug.LogWarning("No ShopRelicDisplay found in scene!");
            return;
        }
        foreach (ShopRelicDisplay display in shopDisplays)
        {
            display.UpdateCostDisplay();
        }
        Debug.Log("Shop prices updated due to DiscountCard status change.");
    }

    public void ResetAll()
    {
        playerInventory.Clear();
        usedRelics.Clear();
        UpdateShopPrices();
        UpdateArmorTextVisibility();
        UpdateInventoryUI();
        if (SkillSelectorManager.Instance != null)
            SkillSelectorManager.Instance.HardResetSkills();
    }

    public void HardResetRelics()
    {
        playerInventory.Clear();
        usedRelics.Clear();
        UpdateShopPrices();
        UpdateArmorTextVisibility();
        UpdateInventoryUI();
    }

    public void SyncWithSkillSelector()
    {
        if (SkillSelectorManager.Instance != null)
            SkillSelectorManager.Instance.UpdateUI();
        UpdateInventoryUI();
    }
}