using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    private PlayerController playerController;
    private ActiveWeapon activeWeapon;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private Animator myAnimator;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
    }
    public void Attack()
    { 
        myAnimator.SetTrigger(FIRE_HASH);
        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position,activeWeapon.transform.rotation);
    }
}
