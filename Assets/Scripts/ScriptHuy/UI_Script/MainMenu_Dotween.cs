using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Dotween : MonoBehaviour
{
    public Button[] menuButtons;         // Kéo 5 nút vào từ Inspector
    public float popupDelay = 0.2f;      // Thời gian delay giữa các nút
    public float popupDuration = 0.3f;   // Thời gian popup từng nút

    private Vector3[] originalScales;    // Lưu scale gốc từng button

    void Start()
    {
        // Khởi tạo mảng lưu scale gốc
        originalScales = new Vector3[menuButtons.Length];

        // Ẩn toàn bộ nút ban đầu và disable tương tác
        for (int i = 0; i < menuButtons.Length; i++)
        {
            originalScales[i] = menuButtons[i].transform.localScale;   // Lưu scale gốc
            menuButtons[i].transform.localScale = Vector3.zero;        // Scale về 0 để chuẩn bị popup
            menuButtons[i].interactable = false;
        }

        // Bắt đầu chuỗi popup
        StartCoroutine(PopupMenu());
    }

    IEnumerator PopupMenu()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button btn = menuButtons[i];

            // Scale từ 0 về lại đúng scale gốc
            btn.transform.DOScale(originalScales[i], popupDuration)
                .SetEase(Ease.OutBack);

            yield return new WaitForSeconds(popupDelay);
        }

        // Sau khi tất cả đã hiện, cho phép bấm
        foreach (Button btn in menuButtons)
        {
            btn.interactable = true;
        }
    }
}
