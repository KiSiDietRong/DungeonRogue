using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class LoginUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Text messageText;
    public UserManager userManager;

    public void OnRegisterClick()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        string result = userManager.Register(username, password);
        messageText.text = result;
    }

    public void OnLoginClick()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Please enter username and password.";
            return;
        }

        // Kiểm tra nếu đúng username & password
        string result = userManager.Login(username, password);
        messageText.text = result;

        if (result == "Login Success!")
        {
            SessionManager.CurrentUsername = username;

            UserData currentUser = UserDataRead.GetUser(username);

            if (currentUser != null)
            {
                if (currentUser.hasLoggedIn)
                {
                    SessionManager.IsReturningUser = true;
                }
                else
                {
                    SessionManager.IsReturningUser = false;

                    currentUser.hasLoggedIn = true;

                    SaveUserToFile(currentUser);
                }
            }

            StartCoroutine(LoadSceneAfterDelay("MainMenu", 3f));
        }
        else
        {
            Debug.Log("Login failed: " + result);
        }
    }


    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    private void SaveUserToFile(UserData updatedUser)
    {
        string savePath = Application.persistentDataPath + "/users.json";

        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        UserListWrapper wrapper = JsonUtility.FromJson<UserListWrapper>(json);

        if (wrapper != null && wrapper.users != null)
        {
            for (int i = 0; i < wrapper.users.Count; i++)
            {
                if (wrapper.users[i].username == updatedUser.username)
                {
                    wrapper.users[i] = updatedUser;
                    break;
                }
            }

            string updatedJson = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(savePath, updatedJson);
            Debug.Log("Updated user login status in file.");
        }
    }
}
