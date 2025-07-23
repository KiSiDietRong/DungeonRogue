using UnityEngine;

public class ClassChanger : MonoBehaviour
{
    public CharacterStatSO characterStat;
    public GameObject weaponPrefab;

    private bool playerInZone;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
                controller.InitFromStats(characterStat);

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
                health.InitFromStats(characterStat);

            var activeWeapon = player.GetComponentInChildren<ActiveWeapon>();
            if (activeWeapon != null)
                activeWeapon.SetActiveWeapon(weaponPrefab);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }
}
