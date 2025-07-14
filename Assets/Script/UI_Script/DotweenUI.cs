using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DotweenUI : MonoBehaviour
{
    [Header("Main UI")]
    public Transform logo;
    public RectTransform signInButton;
    public RectTransform loginButton;
    public RectTransform settingsButton;

    [Header("Panels")]
    public GameObject signInPanel;
    public GameObject loginPanel;

    [Header("Panel Elements")]
    public RectTransform[] signInPanelElements;
    public RectTransform[] loginPanelElements;

    public float logoDropDuration = 1.2f;
    public float buttonPopDuration = 0.5f;
    public float delayBetweenButtons = 0.2f;
    public float panelPopDuration = 0.3f;

    private Vector3 logoStartPos;
    private Vector3 logoTargetPos;

    private Vector3 signInOriginalScale;
    private Vector3 loginOriginalScale;
    private Vector3 settingsOriginalScale;

    // Scale gốc của các phần tử panel
    private Vector3[] signInElementScales;
    private Vector3[] loginElementScales;

    void Start()
    {
        logoTargetPos = logo.position;
        logoStartPos = logoTargetPos + new Vector3(0, 2.5f, 0);
        logo.position = logoStartPos;

        signInOriginalScale = signInButton.localScale;
        loginOriginalScale = loginButton.localScale;
        settingsOriginalScale = settingsButton.localScale;

        signInButton.localScale = Vector3.zero;
        loginButton.localScale = Vector3.zero;
        settingsButton.localScale = Vector3.zero;

        // Lưu scale gốc của các phần tử panel
        signInElementScales = new Vector3[signInPanelElements.Length];
        for (int i = 0; i < signInPanelElements.Length; i++)
        {
            signInElementScales[i] = signInPanelElements[i].localScale;
            signInPanelElements[i].localScale = Vector3.zero;
        }

        loginElementScales = new Vector3[loginPanelElements.Length];
        for (int i = 0; i < loginPanelElements.Length; i++)
        {
            loginElementScales[i] = loginPanelElements[i].localScale;
            loginPanelElements[i].localScale = Vector3.zero;
        }

        signInPanel.SetActive(false);
        loginPanel.SetActive(false);

        AnimateLogo();
    }

    void AnimateLogo()
    {
        logo.DOMove(logoTargetPos, logoDropDuration)
            .SetEase(Ease.OutBounce)
            .OnComplete(AnimateButtons);
    }

    void AnimateButtons()
    {
        signInButton.DOScale(signInOriginalScale, buttonPopDuration).SetEase(Ease.OutBack);
        loginButton.DOScale(loginOriginalScale, buttonPopDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(delayBetweenButtons);
        settingsButton.DOScale(settingsOriginalScale, buttonPopDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(delayBetweenButtons * 2);
    }

    public void OnSignInPressed()
    {
        ShowPanel(signInPanel, signInPanelElements, signInElementScales);
    }

    public void OnLoginPressed()
    {
        ShowPanel(loginPanel, loginPanelElements, loginElementScales);
    }

    public void OnReturnFromSignIn()
    {
        HidePanel(signInPanel, signInPanelElements);
    }

    public void OnReturnFromLogin()
    {
        HidePanel(loginPanel, loginPanelElements);
    }

    public void OnConfirmLogin()
    {
        Invoke("LoadNextScene", 3f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("MainMenu"); // Thay đổi theo tên scene của bạn
    }

    void ShowPanel(GameObject panel, RectTransform[] elements, Vector3[] originalScales)
    {
        panel.SetActive(true);
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].localScale = Vector3.zero;
            elements[i].DOScale(originalScales[i], panelPopDuration).SetEase(Ease.OutBack);
        }
    }

    void HidePanel(GameObject panel, RectTransform[] elements)
    {
        Sequence s = DOTween.Sequence();
        foreach (RectTransform item in elements)
        {
            s.Join(item.DOScale(Vector3.zero, panelPopDuration).SetEase(Ease.InBack));
        }
        s.OnComplete(() => panel.SetActive(false));
    }
}
