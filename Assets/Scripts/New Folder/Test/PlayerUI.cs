using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider hpSlider;
    private PlayerInfo stats;
    public TextMeshProUGUI healthText;

    void Start()
    {
        stats = GetComponent<PlayerInfo>();
        hpSlider.maxValue = stats.maxHP;
        UpdateUI();
    }
    public void UpdateUI()
    {
        hpSlider.maxValue = stats.maxHP;
        hpSlider.value=stats.currentHP;
        healthText.text = stats.currentHP + "/" + stats.maxHP;
    }
}
