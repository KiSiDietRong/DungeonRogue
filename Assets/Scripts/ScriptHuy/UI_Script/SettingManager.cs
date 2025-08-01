using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button soundButton;
    public Button controlButton;

    [Header("Sound UI")]
    public GameObject soundSlider;
    public GameObject sfxSlider;
    public GameObject mainText;
    public GameObject sfxText;

    [Header("Keybind UI")]
    public GameObject keybindGroup; // group các UI về keybind

    [Header("Panels")]
    public GameObject blackPanel;

    void Start()
    {
        // Gán sự kiện cho các nút
        soundButton.onClick.AddListener(ShowSoundSettings);
        controlButton.onClick.AddListener(ShowKeybindSettings);

        // Khi mở setting, mặc định là sound
        ShowSoundSettings();
    }

    public void ShowSoundSettings()
    {
        // Bật UI Sound
        soundSlider.SetActive(true);
        sfxSlider.SetActive(true);
        mainText.SetActive(true);
        sfxText.SetActive(true);

        // Ẩn UI Keybind
        keybindGroup.SetActive(false);

        // Vô hiệu hóa nút sound, bật lại control
        soundButton.interactable = false;
        controlButton.interactable = true;
    }

    public void ShowKeybindSettings()
    {
        // Ẩn UI Sound
        soundSlider.SetActive(false);
        sfxSlider.SetActive(false);
        mainText.SetActive(false);
        sfxText.SetActive(false);

        // Bật UI Keybind
        keybindGroup.SetActive(true);

        // Vô hiệu hóa nút control, bật lại sound
        soundButton.interactable = true;
        controlButton.interactable = false;
    }

    public void CloseSetting()
    {
        blackPanel.SetActive(false);
    }
}
