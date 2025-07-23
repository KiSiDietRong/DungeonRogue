using UnityEngine;

public class SlashAnim : MonoBehaviour
{
    private WeaponInfo weaponInfo;

    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo = info;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && weaponInfo != null)
            {
                enemy.TakeDamage(weaponInfo.weaponDamage);
            }
        }
    }
}