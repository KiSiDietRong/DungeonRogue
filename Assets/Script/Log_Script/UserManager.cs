using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private string savePath;
    private Dictionary<string, string> userDatabase = new Dictionary<string, string>();

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/users.json";
        LoadUsers();
    }

    // Hàm đăng ký
    public string Register(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return "Please enter username and password.";

        if (username.Length > 12)
            return "Username can long than 12 sentences.";

        if (userDatabase.ContainsKey(username))
            return "Account already exist.";

        userDatabase.Add(username, password);
        SaveUsers();
        return "Register Success!";
    }

    public string Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return "Please enter username and password.";

        if (userDatabase.ContainsKey(username))
        {
            if (userDatabase[username] == password)
                return "Login Success!";
            else
                return "Incorrect username or password.";
        }

        return "Incorrect username or password.";
    }

    private void SaveUsers()
    {
        List<UserData> userList = new List<UserData>();
        foreach (var pair in userDatabase)
        {
            userList.Add(new UserData { username = pair.Key, password = pair.Value });
        }
        string json = JsonUtility.ToJson(new UserListWrapper { users = userList }, true);
        Debug.Log("Save JSON: " + json);
        File.WriteAllText(savePath, json);
    }

    private void LoadUsers()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        UserListWrapper wrapper = JsonUtility.FromJson<UserListWrapper>(json);
        userDatabase.Clear();

        if (wrapper != null && wrapper.users != null)
        {
            foreach (var user in wrapper.users)
            {
                userDatabase[user.username] = user.password;
            }
        }
    }
}

[System.Serializable]
public class UserListWrapper
{
    public List<UserData> users = new List<UserData>();
}
