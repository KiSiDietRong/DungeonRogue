using UnityEngine;

public class PlayerSpell : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint; // Where the fireball spawns
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime) // Left click
        {
            CastFireball();
            nextFireTime = Time.time + fireRate;
        }
    }

    void CastFireball()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - firePoint.position;
       

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().SetDirection(direction);
    }
}