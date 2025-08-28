using System.Collections.Generic;
using UnityEngine;

public class UserDataSave : MonoBehaviour
{
    public static UserDataSave Instance;

    public UserData currentUserData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUserData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadUserData()
    {
        string username = SessionManager.CurrentUsername;
        currentUserData = UserDataRead.GetUser(username);

        if (currentUserData == null)
        {
            Debug.LogWarning("Can't find user: " + username);
        }

        if (!currentUserData.unlockedWeapons.Contains("Sword"))
        {
            currentUserData.unlockedWeapons.Add("Sword");
            SaveUserData(); 
        }
    }

    public void SaveUserData()
    {
        if (currentUserData != null)
        {
            UserDataRead.SaveUser(currentUserData); 
        }
    }



    public bool IsWeaponUnlocked(string weapon)
    {
        return currentUserData != null && currentUserData.unlockedWeapons.Contains(weapon);
    }

    public void UnlockWeapon(string weapon)
    {
        if (!currentUserData.unlockedWeapons.Contains(weapon))
        {
            currentUserData.unlockedWeapons.Add(weapon);
            SaveUserData();
        }
    }

    public int GetSouls()
    {
        return currentUserData?.souls ?? 0;
    }

    public void AddSouls(int amount)
    {
        if (currentUserData != null)
        {
            currentUserData.souls += amount;
            SaveUserData();
        }
    }
}
