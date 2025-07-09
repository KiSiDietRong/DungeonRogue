using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    private GameObject slashAnim;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        MouseFollowWithOffset();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
    private void Attack()
    {
        myAnimator.SetTrigger("Attack");

        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(playerController.transform.position);

        SpriteRenderer sr = slashAnim.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (mousePos.x < playerScreenPos.x);
        }
    }
    public void SwingUpFlipAnimEvent()
    {
        if (slashAnim == null) return;
        slashAnim.transform.rotation = Quaternion.Euler(-180, 0, 0);
    }
    public void SwingDownFlipAnimEvent()
    {
        if (slashAnim == null) return;
        slashAnim.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
