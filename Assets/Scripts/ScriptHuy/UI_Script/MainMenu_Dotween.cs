using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Dotween : MonoBehaviour
{
    public Button[] menuButtons;         
    public float popupDelay = 0.2f;      
    public float popupDuration = 0.3f;   

    private Vector3[] originalScales;   

    void Start()
    {
        originalScales = new Vector3[menuButtons.Length];

        for (int i = 0; i < menuButtons.Length; i++)
        {
            originalScales[i] = menuButtons[i].transform.localScale;   
            menuButtons[i].transform.localScale = Vector3.zero;        
            menuButtons[i].interactable = false;
        }

        StartCoroutine(PopupMenu());
    }

    IEnumerator PopupMenu()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button btn = menuButtons[i];

            btn.transform.DOScale(originalScales[i], popupDuration)
                .SetEase(Ease.OutBack);

            yield return new WaitForSeconds(popupDelay);
        }

        foreach (Button btn in menuButtons)
        {
            btn.interactable = true;
        }
    }
}
