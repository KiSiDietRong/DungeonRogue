using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public IWeapon CurrentActiveWeapon { get; private set; }

    private GameObject currentWeaponObj;
    private float timeBetweenAttacks;
    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
        }
    }

    public void SetActiveWeapon(GameObject newWeaponPrefab)
    {
        if (currentWeaponObj != null)
            Destroy(currentWeaponObj);

        currentWeaponObj = Instantiate(newWeaponPrefab, transform);
        CurrentActiveWeapon = currentWeaponObj.GetComponent<IWeapon>();

        timeBetweenAttacks = CurrentActiveWeapon.GetWeaponInfo().weaponCooldown;
    }

    private void Attack()
    {
        if (CurrentActiveWeapon != null)
        {
            CurrentActiveWeapon.Attack();
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }
}
