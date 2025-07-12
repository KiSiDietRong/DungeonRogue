using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
    }

    public void Attack()
    {
        Debug.Log("Staff Attack");
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
