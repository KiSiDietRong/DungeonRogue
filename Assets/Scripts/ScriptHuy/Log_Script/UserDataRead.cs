using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserDataRead
{
    private static string savePath = Application.persistentDataPath + "/users.json";

    public static UserData GetUser(string username)
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        UserListWrapper wrapper = JsonUtility.FromJson<UserListWrapper>(json);

        if (wrapper != null && wrapper.users != null)
        {
            foreach (var user in wrapper.users)
            {
                if (user.username == username)
                {
                    return user;
                }
            }
        }

        return null;
    }
}
