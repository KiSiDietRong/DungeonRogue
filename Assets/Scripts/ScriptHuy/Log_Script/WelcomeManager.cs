using UnityEngine;
using UnityEngine.UI;

public class WelcomeManager : MonoBehaviour
{
    public Text welcomeText;

    void Start()
    {
        string username = SessionManager.CurrentUsername;

        if (!string.IsNullOrEmpty(username))
        {
            if (SessionManager.IsReturningUser)
            {
                welcomeText.text = "Welcome back, " + username;
            }
            else
            {
                welcomeText.text = "Welcome, " + username;
            }
        }
        else
        {
            welcomeText.text = "Welcome!";
        }
    }
}
