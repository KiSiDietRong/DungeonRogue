using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button soundButton;
    public Button controlButton;
    public Button screenButton;
    

    [Header("Sound UI")]
    public GameObject soundSlider;
    public GameObject sfxSlider;
    public GameObject mainSlider;
    public GameObject mainText;
    public GameObject sfxText;
    public GameObject soundText;

    [Header("Keybind UI")]
    public GameObject keybindGroup;

    [Header("Screen UI")]
    public GameObject screenGroup;

    [Header("Panels")]
    public GameObject blackPanel;

    void Start()
    {
        soundButton.onClick.AddListener(ShowSoundSettings);
        controlButton.onClick.AddListener(ShowKeybindSettings);
        screenButton.onClick.AddListener(ShowScreenSettings);

        ShowSoundSettings();
    }

    public void ShowSoundSettings()
    {
        mainSlider.SetActive(true);
        soundSlider.SetActive(true);
        sfxSlider.SetActive(true);
        mainText.SetActive(true);
        sfxText.SetActive(true);
        soundText.SetActive(true);

        keybindGroup.SetActive(false);
        screenGroup.SetActive(false);

        soundButton.interactable = false;
        controlButton.interactable = true;
        screenButton.interactable = true;
    }

    public void ShowKeybindSettings()
    {
        mainSlider.SetActive(false);
        soundSlider.SetActive(false);
        sfxSlider.SetActive(false);
        mainText.SetActive(false);
        sfxText.SetActive(false);
        soundText.SetActive(false);

        keybindGroup.SetActive(true);
        screenGroup.SetActive(false);

        soundButton.interactable = true;
        controlButton.interactable = false;
        screenButton.interactable = true;
    }
    public void ShowScreenSettings() 
    {
        mainSlider.SetActive(false);
        soundSlider.SetActive(false);
        sfxSlider.SetActive(false);
        mainText.SetActive(false);
        sfxText.SetActive(false);
        soundText.SetActive(false);

        keybindGroup.SetActive(false);

        screenGroup.SetActive(true);

        soundButton.interactable = true;
        controlButton.interactable = true;
        screenButton.interactable = false;
    }
    public void CloseSetting()
    {
        blackPanel.SetActive(false);
    }
}
