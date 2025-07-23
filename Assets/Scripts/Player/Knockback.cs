using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private float knockBackDistance = 1f;

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        if (GettingKnockedBack) return;

        GettingKnockedBack = true;

        Vector2 direction = ((Vector2)transform.position - (Vector2)damageSource.position).normalized;

        Vector2 targetPosition = (Vector2)transform.position + direction * knockBackThrust;

        StartCoroutine(KnockRoutine(targetPosition));
    }

    private IEnumerator KnockRoutine(Vector2 targetPosition)
    {
        float elapsed = 0f;
        float duration = knockBackTime;
        Vector2 start = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(start, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        GettingKnockedBack = false;
    }
}
