using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHP = 700f;
    public float damage = 20f;
    public float currentHP;

    [Header("Attack Prefabs")]
    public GameObject bulletPrefab;
    public GameObject laserPrefab;
    public GameObject shieldPrefab;

    [Header("Spawn Points")]
    public Transform firePoint;
    public Transform laserPoint;
    public Transform shieldPoint;

    public GameObject damagePopupPrefab;
    protected GameObject player;
    [SerializeField] private float popupOffsetRadius = 0.5f;

    private Animator anim;
    private bool isDead = false;

    public delegate void BossDeathHandler(BossController boss);
    public static event BossDeathHandler OnBossDeath;

    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // =====================
    // HỆ THỐNG NHẬN DAMAGE
    // =====================
    public void TakeDamage(float damage, Vector3 hitPosition, bool isCritical)
    {
        if (isDead) return;

        currentHP -= damage;
        BossHealthUI.Instance?.UpdateHealth(currentHP);

        if (damagePopupPrefab != null)
        {
            Vector2 offset = Random.insideUnitCircle * popupOffsetRadius;
            Vector3 popupPosition = transform.position + new Vector3(offset.x, offset.y + 1f, 0f);
            GameObject popup = Instantiate(damagePopupPrefab, popupPosition, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup((int)damage, isCritical);
            }
        }

        if (currentHP <= 0) Die();
    }

    public float GetCurrentHealth() => currentHP;

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Death");

        // Tắt AI
        GetComponent<BossAI>()?.OnDeath();

        // Gửi event
        OnBossDeath?.Invoke(this);

        // Boss không destroy ngay -> đợi anim xong
        Destroy(gameObject, 1.5f);
    }

    // =====================
    // SKILL / ANIM
    // =====================
    public void DoGlow() => anim.SetTrigger("Glow");
    public void DoShoot() => anim.SetTrigger("Shoot");
    public void DoMelee() => anim.SetTrigger("Melee");
    public void DoLaser() => anim.SetTrigger("Laser");
    public void DoShield() => anim.SetTrigger("Shield");

    // Gọi trong Animation Event
    public void SpawnBullet()
    {
        if (bulletPrefab && firePoint)
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }

    public void SpawnLaser()
    {
        if (laserPrefab && laserPoint)
            Instantiate(laserPrefab, laserPoint.position, Quaternion.identity);
    }

    public void SpawnShield()
    {
        if (shieldPrefab && shieldPoint)
            Instantiate(shieldPrefab, shieldPoint.position, Quaternion.identity, transform);
    }

    public void AttackPlayer()
    {
        var playerScript = player.GetComponent<PlayerHealth>();
        if (playerScript != null)
        {
            PlayerController pc = playerScript.GetComponent<PlayerController>();
            if (pc != null && !pc.isDashing && !playerScript.GetComponent<Knockback>().GettingKnockedBack)
            {
                playerScript.TakeDamage((int)damage, transform);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, transform.position, false);
    }
}
