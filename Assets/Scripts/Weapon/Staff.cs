using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;

    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject magicPrefab;
    [SerializeField] private Transform magicSpawnPoint;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    private Animator myAnimator;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        myAnimator.SetTrigger(ATTACK_HASH);
        GameObject newArrow = Instantiate(magicPrefab, magicSpawnPoint.position, activeWeapon.transform.rotation);
        Projectile projectile = newArrow.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.UpdateWeaponInfo(weaponInfo);
        }
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    void Update()
    {
        MouseFollowWithOffset();
    }

    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);
        Vector2 delta = mousePos - playerScreenPoint;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        activeWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 scale = activeWeapon.transform.localScale;
        scale.y = delta.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        activeWeapon.transform.localScale = scale;
    }
}