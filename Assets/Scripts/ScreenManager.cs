using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown cursorColorDropdown;
    public Slider cursorSizeSlider;

    [Header("Cursor Sprites")]
    public Texture2D yellowCursor;
    public Texture2D redCursor;
    public Texture2D blueCursor;

    private Resolution[] availableResolutions;
    private FullScreenMode currentMode = FullScreenMode.Windowed;

    private Texture2D CursorTexture;

    void Start()
    {
        SetupResolutions();
        SetupWindowModes();
        SetupCursorOptions();
    }

    // ==== RESOLUTION ====
    void SetupResolutions()
    {
        // Lấy độ phân giải 16:9, loại bỏ trùng lặp (chỉ giữ width x height duy nhất)
        availableResolutions = Screen.resolutions
            .Where(r => Mathf.Approximately((float)r.width / r.height, 16f / 9f))
            .GroupBy(r => new { r.width, r.height }) // group theo width, height
            .Select(g => g.First())                  // lấy 1 cái duy nhất
            .OrderBy(r => r.width)
            .ToArray();

        resolutionDropdown.ClearOptions();

        var options = availableResolutions
            .Select(r => r.width + " x " + r.height)
            .ToList();

        resolutionDropdown.AddOptions(options);

        // Chọn mặc định = current resolution
        int currentIndex = System.Array.FindIndex(availableResolutions,
            r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);

        if (currentIndex < 0) currentIndex = options.Count - 1;

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetResolution(int index)
    {
        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, currentMode);
    }

    // ==== WINDOW MODE ====
    void SetupWindowModes()
    {
        windowModeDropdown.ClearOptions();
        // BỎ borderless -> chỉ giữ Fullscreen và Windowed
        windowModeDropdown.AddOptions(new List<string>() { "Fullscreen", "Windowed" });

        windowModeDropdown.value = 1; // default Windowed
        windowModeDropdown.RefreshShownValue();

        windowModeDropdown.onValueChanged.AddListener(SetWindowMode);
    }

    void SetWindowMode(int index)
    {
        switch (index)
        {
            case 0: currentMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: currentMode = FullScreenMode.Windowed; break;
        }

        // reset lại theo resolution đang chọn
        SetResolution(resolutionDropdown.value);
    }

    // ==== CURSOR SCALE & COLOR ====
    void SetupCursorOptions()
    {
        cursorColorDropdown.ClearOptions();
        cursorColorDropdown.AddOptions(new List<string>() { "Yellow", "Red", "Blue" });

        cursorColorDropdown.onValueChanged.AddListener(SetCursorColor);
        cursorSizeSlider.onValueChanged.AddListener(SetCursorSize);

        // mặc định màu vàng
        SetCursor(yellowCursor, Vector2.zero, CursorMode.Auto);
    }

    void SetCursorColor(int index)
    {
        Texture2D tex = yellowCursor;
        if (index == 1) tex = redCursor;
        else if (index == 2) tex = blueCursor;

        SetCursor(tex, Vector2.zero, CursorMode.Auto);
    }

    void SetCursorSize(float scale)
    {
        if (CursorTexture == null) return;

        int newW = Mathf.RoundToInt(CursorTexture.width * scale);
        int newH = Mathf.RoundToInt(CursorTexture.height * scale);

        Texture2D scaled = new Texture2D(newW, newH);
        Graphics.ConvertTexture(CursorTexture, scaled);

        SetCursor(scaled, Vector2.zero, CursorMode.Auto);
    }

    private void SetCursor(Texture2D tex, Vector2 hotspot, CursorMode mode)
    {
        CursorTexture = tex;
        Cursor.SetCursor(tex, hotspot, mode);
    }
}
