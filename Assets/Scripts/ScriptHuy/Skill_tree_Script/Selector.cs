using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Selector : MonoBehaviour
{
    public static Selector Instance;

    private GameObject currentSelectedUp;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectUpgrade(GameObject up)
    {
        if (currentSelectedUp != null && currentSelectedUp != up)
        {
            HideOutline(currentSelectedUp);
        }

        if (currentSelectedUp == up)
        {
            currentSelectedUp = null;
            return;
        }

        currentSelectedUp = up;
        ShowOutline(up);
        DeselectOthersInLine(up.transform.parent);
    }

    private void ShowOutline(GameObject up)
    {
        Outline outline = up.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            outline.effectDistance = Vector2.zero;
            DOTween.Kill(outline);
            DOTween.To(
                () => outline.effectDistance,
                x => outline.effectDistance = x,
                new Vector2(12f, 12f),  
                0.5f 
            ).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine)
             .SetTarget(outline);
        }
    }

    private void HideOutline(GameObject up)
    {
        Outline outline = up.GetComponent<Outline>();
        if (outline != null)
        {
            DOTween.Kill(outline);
            outline.effectDistance = new Vector2(2f, 2f);
            outline.enabled = true;
        }
    }

    private void DeselectOthersInLine(Transform lineParent)
    {
        foreach (Transform up in lineParent)
        {
            if (up.gameObject != currentSelectedUp)
            {
                HideOutline(up.gameObject);
            }
        }
    }
}
