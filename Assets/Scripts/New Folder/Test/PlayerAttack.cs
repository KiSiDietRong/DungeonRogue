using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    private PlayerInfo stats;

    private void Start()
    {
        stats = GetComponent<PlayerInfo>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().SetDamage(stats.attackPower);
    }
}
