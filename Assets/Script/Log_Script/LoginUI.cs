using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Text messageText;
    public UserManager userManager;

    public void OnRegisterClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        string result = userManager.Register(username, password);
        messageText.text = result;
    }

    public void OnLoginClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        string result = userManager.Login(username, password);
        messageText.text = result;

        if (result == "Login Success!")
        {
            // Gọi coroutine để đợi 3 giây trước khi chuyển scene
            StartCoroutine(LoadSceneAfterDelay("MainMenu", 3f));
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
