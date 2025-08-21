using System.Collections;
using UnityEngine;

public class AoeDamageController : MonoBehaviour
{
    private AoeDamageSkill data;
    private float elapsed = 0f;

    public void Setup(AoeDamageSkill skillData)
    {
        data = skillData;
        transform.localScale = Vector3.one * data.radius * 2f;

        StartCoroutine(DoDamageOverTime());
        Destroy(gameObject, data.duration + 0.1f);
    }

    private IEnumerator DoDamageOverTime()
    {
        while (elapsed < data.duration)
        {
            DealDamageOnce();
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }

    private void DealDamageOnce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.radius, data.targetLayer);
        foreach (var col in hits)
        {
            col.gameObject.SendMessage("TakeDamage", Mathf.RoundToInt(data.damagePerSecond), SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data != null ? data.radius : 1f);
    }
}
