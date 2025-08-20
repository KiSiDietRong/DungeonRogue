using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Texture2D greenCursor;

    private Resolution[] resolutions;
    private Texture2D currentCursor;
    private float cursorScale = 1f;

    void Start()
    {
        // --- RESOLUTION ---
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentResolutionIndex = 0;

        var options = new System.Collections.Generic.List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option)) options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // --- WINDOW MODE ---
        windowModeDropdown.ClearOptions();
        var wmOptions = new System.Collections.Generic.List<string>() { "Fullscreen", "Windowed", "Borderless" };
        windowModeDropdown.AddOptions(wmOptions);
        windowModeDropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 :
                                   Screen.fullScreenMode == FullScreenMode.Windowed ? 1 : 2;
        windowModeDropdown.RefreshShownValue();
        windowModeDropdown.onValueChanged.AddListener(SetWindowMode);

        // --- CURSOR COLOR ---
        cursorColorDropdown.ClearOptions();
        var colorOptions = new System.Collections.Generic.List<string>() { "Yellow", "Red", "Green" };
        cursorColorDropdown.AddOptions(colorOptions);
        cursorColorDropdown.value = 0;
        cursorColorDropdown.RefreshShownValue();
        cursorColorDropdown.onValueChanged.AddListener(SetCursorColor);

        // --- CURSOR SIZE ---
        cursorSizeSlider.onValueChanged.AddListener(SetCursorSize);

        // default cursor
        SetCursorColor(0);
    }

    // ----------------- METHODS -----------------

    void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    void SetWindowMode(int modeIndex)
    {
        switch (modeIndex)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 1: Screen.fullScreenMode = FullScreenMode.Windowed; break;
            case 2: Screen.fullScreenMode = FullScreenMode.MaximizedWindow; break;
        }
    }

    void SetCursorColor(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0: currentCursor = yellowCursor; break;
            case 1: currentCursor = redCursor; break;
            case 2: currentCursor = greenCursor; break;
        }
        ApplyCursor();
    }

    void SetCursorSize(float value)
    {
        cursorScale = Mathf.Lerp(0.5f, 3f, value); // scale từ 0.5x đến 3x
        ApplyCursor();
    }

    void ApplyCursor()
    {
        if (currentCursor == null) return;

        int size = Mathf.RoundToInt(currentCursor.width * cursorScale);
        Texture2D scaled = ScaleTexture(currentCursor, size, size);

        Cursor.SetCursor(scaled, Vector2.zero, CursorMode.Auto);
    }

    // Hàm scale texture
    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / targetWidth);
        float incY = (1.0f / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * (px % targetWidth),
                                                  incY * (px / targetWidth));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}
