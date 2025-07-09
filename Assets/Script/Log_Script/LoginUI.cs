using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            // Chuyển cảnh hoặc vào game
            SceneManager.LoadScene("MainScene");
        }
    }
}
