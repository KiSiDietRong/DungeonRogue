using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    public GameObject exitPopup; 
    public void OnExitButtonClicked()
    {
        exitPopup.SetActive(true); 
    }

    public void OnCancelExit()
    {
        exitPopup.SetActive(false); 
    }

    public void OnConfirmExit()
    {
        SessionManager.CurrentUsername = ""; 
        SceneManager.LoadScene("Log_Scene"); 
    }
}
