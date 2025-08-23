using UnityEngine;
using UnityEngine.UI;

public class SkillTreePage : MonoBehaviour
{
    [Header("List Page")]
    [SerializeField] private GameObject[] pages;

    [Header("Button")]
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;

    private int currentPageIndex = 0;

    private void OnEnable()
    {
        currentPageIndex = 0;
        UpdatePages();
    }

    private void Start()
    {
        buttonLeft.onClick.AddListener(PreviousPage);
        buttonRight.onClick.AddListener(NextPage);

        UpdatePages(); 
    }

    private void UpdatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }

        buttonLeft.interactable = currentPageIndex > 0;
        buttonRight.interactable = currentPageIndex < pages.Length - 1;
    }

    private void NextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePages();
        }
    }

    private void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePages();
        }
    }
}
