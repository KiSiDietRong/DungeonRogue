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
    private bool canvasActive = false;
    private bool relicSelected = false;
    private HashSet<Relic> usedRelics = new HashSet<Relic>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!canvasActive || relicSelected) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectRelic((selectedRelicIndex - 1 + 4) % 4);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectRelic((selectedRelicIndex + 1) % 4);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OnRelicChosen(selectedRelicIndex);
        }
    }

    public void OpenCanvas()
    {
        canvasUI.SetActive(true);
        canvasActive = true;
        relicSelected = false;
        ShowRandomRelics();
        UpdateInventoryUI();
        SelectRelic(0);
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
        int maxToSelect = 4;

        while (selected.Count < maxToSelect)
        {
            int roll = Random.Range(0, 100);
            List<Relic> pool = null;

            if (roll < chanceLegendary && legendaries.Count > 0)
                pool = legendaries;
            else if (roll < (chanceLegendary + chanceEpic) && epics.Count > 0)
                pool = epics;
            else if (commons.Count > 0)
                pool = commons;

            if (pool != null)
            {
                Relic r = pool[Random.Range(0, pool.Count)];
                if (!selected.Contains(r))
                    selected.Add(r);
            }

            if (commons.Count + epics.Count + legendaries.Count <= selected.Count)
                break;
        }

        selected.Sort((a, b) => a.rarityType.CompareTo(b.rarityType));

        bool hasLegendary = selected.Exists(r => r.rarityType == Rarity.Legendary);
        int buttonsToShow = hasLegendary ? 4 : 3;

        for (int i = 0; i < relicButtons.Length; i++)
        {
            if (i < buttonsToShow && i < selected.Count)
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
            }
            else
            {
                relicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectRelic(int index)
    {
        selectedRelicIndex = index;

        for (int i = 0; i < 4; i++)
        {
            relicButtons[i].transform.DOScale(Vector3.one, 0.2f);
        }

        relicButtons[selectedRelicIndex].transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }

    void OnRelicChosen(int index)
    {
        if (playerInventory.Count >= inventorySlots.Length)
        {
            Debug.Log("Inventory full!");
            return;
        }

        Relic chosen = currentRelicChoices[index];
        playerInventory.Add(chosen);
        usedRelics.Add(chosen);
        UpdateInventoryUI();

        relicSelected = true;

        ApplyRelicEffect(chosen);

        StartCoroutine(CloseCanvasAfterDelay(1f));
    }

    IEnumerator CloseCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canvasUI.SetActive(false);
        canvasActive = false;
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
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

        switch (relic.type)
        {
            case RelicType.BobsContainmentField:
                Debug.Log("Lần đầu tiên sử dụng Kỹ năng trong mỗi phòng sẽ cấp một Lá chắn trong 5 giây");
                break;
            case RelicType.BonePlate:
                Debug.Log("Nhận được 15 Giáp. Mỗi khi bạn đánh bại một kẻ địch, bạn nhận được 1 Giáp");
                break;
            case RelicType.ConduitSpike:
                Debug.Log("Đòn đánh thứ 3 của bạn giải phóng Chain Lightning, gây sát thương lên tới 2 kẻ địch khác");
                break;
            case RelicType.DiscountCard:
                Debug.Log("Mua vật phẩm giá rẻ hơn");
                break;
            case RelicType.DoomShell:
                Debug.Log("Bất cứ khi nào bạn lướt sẽ gây Sát thương lên kẻ địch xung quanh bạn");
                break;
            case RelicType.EmpoweredBangle:
                Debug.Log("Lần đầu tiên bạn sử dụng kỹ năng trong mỗi phòng sẽ gây thêm Sát thương");
                break;
            case RelicType.FuryCharm:
                Debug.Log("Các đòn tấn công của bạn gây thêm sát thương lên kẻ địch đang bị đốt cháy");
                break;
            case RelicType.GlacialLight:
                Debug.Log("Bất cứ khi nào kẻ địch bị đóng băng, chúng cũng bị thiêu đốt");
                break;
            case RelicType.RazorClaw:
                Debug.Log("Đòn chí mạng của bạn giờ đây gây gấp đôi sát thương");
                break;
            case RelicType.SpiritShelter:
                Debug.Log("Khi chết lần đầu, bạn sẽ hồi sinh với đầy HP và di vật này sẽ biến mất");
                break;
        }
    }
}
