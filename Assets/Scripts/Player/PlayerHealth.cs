using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float knockbackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    private int currentHealth;
    private int maxHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    public CharacterStatSO characterStat;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    public void InitFromStats(CharacterStatSO stats)
    {
        characterStat = stats;
        maxHealth = Mathf.RoundToInt(characterStat.maxHealth);
        currentHealth = maxHealth;
        InitUI();
    }

    private void InitUI()
    {
        if (hpSlider != null)
            hpSlider.maxValue = maxHealth;

        UpdateUI();
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;
        currentHealth -= damageAmount;

        knockback?.GetKnockedBack(hitTransform, knockbackThrustAmount);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }

        StartCoroutine(ResetDamageRecovery());
    }

    private IEnumerator ResetDamageRecovery()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHealth;

        if (hpText != null)
            hpText.text = $"{currentHealth} / {maxHealth}";
    }

    private void Die()
    {
        Debug.Log("Player died");
    }
}
