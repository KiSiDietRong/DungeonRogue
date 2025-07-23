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
    public Image[] inventorySlots;
    private List<Relic> playerInventory = new List<Relic>();

    [Header("UI Panel")]
    public GameObject canvasUI;

    [Header("Rarity Spawn Chances (0-100)")]
    [Range(0, 100)] public int chanceCommon = 70;
    [Range(0, 100)] public int chanceEpic = 24;
    [Range(0, 100)] public int chanceLegendary = 6;

    private int selectedRelicIndex = 0;
    private int activeRelicCount = 0;
    private bool canvasActive = false;
    private bool relicSelected = false;
    private bool inventoryOnlyView = false;
    private bool isPickingRelic = false;
    private HashSet<Relic> usedRelics = new HashSet<Relic>();

    private bool waitingForReplace = false;
    private int selectedReplaceIndex = 0;
    private bool clickOnce = false;
    private bool relicConfirming = false;
    private bool relicLocked = false;
    private bool pendingRelicChoose = false;
    private Relic relicToReplace;

    void Awake() => Instance = this;

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

                for (int i = 0; i < inventorySlots.Length; i++)
                    inventorySlots[i].transform.DOScale(Vector3.one, 0.2f);
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
                canvasUI.SetActive(true);
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

    void HandleRelicSelection(Relic relic)
    {
        if (playerInventory.Count < inventorySlots.Length)
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
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(inventorySlots[i].rectTransform, mousePos))
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
        inventorySlots[selectedReplaceIndex].transform.DOScale(Vector3.one, 0.2f);
        selectedReplaceIndex = index;
        inventorySlots[selectedReplaceIndex].transform.DOScale(Vector3.one * 1.2f, 0.2f);
        clickOnce = false;
    }

    void ReplaceSlotConfirmed()
    {
        if (selectedReplaceIndex >= 0 && selectedReplaceIndex < playerInventory.Count)
        {
            playerInventory[selectedReplaceIndex] = relicToReplace;
        }
        else
        {
            playerInventory.Add(relicToReplace);
        }
        usedRelics.Add(relicToReplace);
        UpdateInventoryUI();
        ApplyRelicEffect(relicToReplace);
        waitingForReplace = false;
        clickOnce = false;
        isPickingRelic = false;
        relicSelected = false;
        relicConfirming = false;
        pendingRelicChoose = false;
        StartCoroutine(CloseCanvasAfterDelay(1f));
    }

    public void OpenCanvas()
    {
        canvasUI.SetActive(true);
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
    }

    public void ToggleInventoryOnlyUI()
    {
        bool active = !canvasUI.activeSelf;
        canvasUI.SetActive(active);
        canvasActive = active;
        inventoryOnlyView = active;
        relicSelected = false;
        UpdateInventoryUI();

        foreach (var btn in relicButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public bool IsCanvasActive()
    {
        return canvasActive;
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
        canvasUI.SetActive(false);
        canvasActive = false;
        isPickingRelic = false;
        relicSelected = false;
        relicConfirming = false;
        pendingRelicChoose = false;
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].transform.DOScale(Vector3.one, 0.1f);

            if (i < playerInventory.Count)
            {
                inventorySlots[i].sprite = playerInventory[i].icon;
                inventorySlots[i].color = Color.white;
            }
            else
            {
                inventorySlots[i].sprite = null;
                inventorySlots[i].color = new Color(0, 0, 0, 0);
            }
        }
    }

    void ApplyRelicEffect(Relic relic)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"Apply effect of relic: {relic.relicName}");
    }
}