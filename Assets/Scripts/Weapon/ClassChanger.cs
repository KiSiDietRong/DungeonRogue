using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassChanger : MonoBehaviour
{
    public CharacterStatSO characterStat;
    public WeaponInfo weaponInfo;

    [Header("UI Weapon Info (World Space Canvas)")]
    public GameObject weaponInfoPanel;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI critText;

    private bool playerInZone;

    void Start()
    {
        if (weaponInfoPanel != null)
            weaponInfoPanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInZone) return;

        WeaponLock lockController = GetComponent<WeaponLock>();
        if (lockController != null && lockController.isLocked)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
                health.InitFromStats(characterStat);

            var activeWeapon = player.GetComponentInChildren<ActiveWeapon>();
            if (activeWeapon != null && weaponInfo != null)
                activeWeapon.SetActiveWeapon(weaponInfo.weaponPrefab);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            ShowWeaponInfo();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            HideWeaponInfo();
        }
    }

    void ShowWeaponInfo()
    {
        if (weaponInfoPanel == null || weaponInfo == null || characterStat == null) return;

        WeaponLock lockController = GetComponent<WeaponLock>();
        if (lockController != null && lockController.isLocked)
        {
            weaponInfoPanel.SetActive(false);
            return;
        }

        hpText.text = "HP: " + characterStat.maxHealth;
        dmgText.text = "DMG: " + weaponInfo.weaponDamage;
        critText.text = "Crit: " + (weaponInfo.criticalChance * 100f) + "%";

        weaponInfoPanel.SetActive(true);
    }


    void HideWeaponInfo()
    {
        if (weaponInfoPanel != null)
            weaponInfoPanel.SetActive(false);
    }
}
