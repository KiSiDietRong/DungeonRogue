using UnityEngine;
using System.Collections;

public class Enemypoison : MonoBehaviour
{
    private Coroutine poisonCoroutine;

    public void ApplyPoison(float duration, int dps)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }

        poisonCoroutine = StartCoroutine(PoisonRoutine(duration, dps));
    }

    private IEnumerator PoisonRoutine(float duration, int dps)
    {
        float timer = 0f;
        while (timer < duration)
        {
            GetComponent<Enemy>().TakeDamage(dps);
            yield return new WaitForSeconds(1f);
            timer += 1f;
        }
    }
}
