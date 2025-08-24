using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Slider bossHealthSlider;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Gọi khi Boss xuất hiện
    public void ShowBossUI(BossController boss, string bossName)
    {
        bossHealthSlider.maxValue = boss.maxHP;
        bossHealthSlider.value = boss.GetCurrentHealth();
    }

    // Cập nhật máu Boss
    public void UpdateHealth(float currentHP)
    {
        bossHealthSlider.value = currentHP;
    }
}
