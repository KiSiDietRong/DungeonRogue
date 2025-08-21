using UnityEngine;
using UnityEngine.UI;
public class ExitGame : MonoBehaviour
{
    public Button exitButton;
    void Start()
    {
        exitButton.onClick.AddListener(Exit);
    }

    public void Exit()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }

}
