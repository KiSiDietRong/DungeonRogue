using DG.Tweening;
using UnityEngine;

public class WeaponBounce : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatDropAmount = 0.05f;

    public float floatCycleDuration = 1.2f;

    private Vector3 originalPos;
    private Tween floatTween;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void StartFloating()
    {
        if (floatTween != null && floatTween.IsActive()) return;

        floatTween = transform.DOLocalMoveY(originalPos.y - floatDropAmount, floatCycleDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopFloating()
    {
        if (floatTween != null && floatTween.IsActive())
        {
            floatTween.Kill();
            floatTween = null;
            transform.localPosition = originalPos;
        }
    }
}
