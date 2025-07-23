using UnityEngine;

public class ClassChanger : MonoBehaviour
{
    public GameObject weaponPrefab;

    private bool playerInZone;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var activeWeapon = player.GetComponentInChildren<ActiveWeapon>();
            activeWeapon.SetActiveWeapon(weaponPrefab);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }
}
