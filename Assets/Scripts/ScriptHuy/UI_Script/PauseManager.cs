using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void GoToMainMenu(string mainMenuSceneName)
    {
        Time.timeScale = 1f;
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.gameObject.SetActive(false);
        }
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                OpenSettings();
        }
    }
}
