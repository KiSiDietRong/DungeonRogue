using UnityEngine;

public class WeaponLock : MonoBehaviour
{
    public enum WeaponType { Sword, Staff, Bow }

    public WeaponType weaponType;

    public int soulRequired = 0; 
    private PlayerController player;

    public bool isLocked = true;
    public GameObject lockOverlay;

    private ClassChanger classChanger;
    private WeaponBounce floatTween;

    void Start()
    {
        classChanger = GetComponent<ClassChanger>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        floatTween = GetComponentInChildren<WeaponBounce>();

        if (UserDataSave.Instance != null && UserDataSave.Instance.IsWeaponUnlocked(weaponType.ToString()))
        {
            isLocked = false;
        }

        if (!isLocked && floatTween != null)
            floatTween.StartFloating();

        UpdateLockState();
    }



    void Update()
    {
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
        if (!isLocked) return;

        isLocked = false;
        Debug.Log($"{weaponType} unlocked!");
        UpdateLockState();

        UserDataSave.Instance.UnlockWeapon(weaponType.ToString());

        if (floatTween != null)
            floatTween.StartFloating();
    }


    public void CheckForUnlock()
    {
        if (!isLocked)
            return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.GetComponent<PlayerController>();
        }

        if (player == null)
        {
            Debug.LogWarning("Player not found in CheckForUnlock!");
            return;
        }

        if (player.Souls >= soulRequired)
            UnlockWeapon();
    }
}
