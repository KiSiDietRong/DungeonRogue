using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockbackThrustAmount = 0.2f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage || knockback.GettingKnockedBack) return;

        currentHealth -= damageAmount;
        canTakeDamage = false;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
        }

        if (knockback != null)
        {
            knockback.GetKnockedBack(hitTransform, knockbackThrustAmount);
        }

        StartCoroutine(DamageRecoveryRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Debug.Log("Player has died!");
    }
}