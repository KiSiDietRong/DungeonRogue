using UnityEngine;

public class WeaponLock : MonoBehaviour
{
    public enum WeaponType { Sword, Staff, Bow }

    public WeaponType weaponType;
    private PlayerController player;

    public bool isLocked = true; 
    public GameObject lockOverlay;

    private ClassChanger classChanger;

    private WeaponBounce floatTween;


    void Start()
    {
        classChanger = GetComponent<ClassChanger>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        floatTween = GetComponentInChildren<WeaponBounce>();
        if (!isLocked && floatTween != null)
        {
            floatTween.StartFloating();
        }

        UpdateLockState();
    }

    void Update()
    {
        if (isLocked && player != null)
        {
            switch (weaponType)
            {
                case WeaponType.Staff:
                    if (player.Souls >= 400)
                    {
                        UnlockWeapon();
                    }
                    break;
                case WeaponType.Bow:
                    if (player.Souls >= 600)
                    {
                        UnlockWeapon();
                    }
                    break;
            }
        }

        if (classChanger != null)
            classChanger.enabled = !isLocked;
    }

    private void UpdateLockState()
    {
        if (lockOverlay != null)
            lockOverlay.SetActive(isLocked);
    }

    public void UnlockWeapon()
    {
        isLocked = false;
        UpdateLockState();

        if (floatTween != null)
        {
            floatTween.StartFloating();
        }
    }
}
