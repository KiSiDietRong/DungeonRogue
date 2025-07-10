using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public IWeapon CurrentActiveWeapon { get; private set; }
    private GameObject currentWeaponObj;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
    }

    public void Attack()
    {
        if (CurrentActiveWeapon != null)
            CurrentActiveWeapon.Attack();
    }
}
