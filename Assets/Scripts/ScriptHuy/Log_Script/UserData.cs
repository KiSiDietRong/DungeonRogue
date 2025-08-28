using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public bool hasLoggedIn;

    public int souls = 0;

    public List<string> unlockedWeapons = new List<string>();
}
