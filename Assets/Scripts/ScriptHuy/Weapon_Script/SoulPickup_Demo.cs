using UnityEngine;

public class SoulPickup_Demo : MonoBehaviour
{
    public int soulAmount = 50;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddSouls(soulAmount);

                WeaponLock[] allWeaponLocks = FindObjectsOfType<WeaponLock>();
                foreach (var weapon in allWeaponLocks)
                {
                    weapon.CheckForUnlock();
                }

                Destroy(gameObject);
            }
        }
    }

}
