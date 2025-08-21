using System.Collections;
using UnityEngine;

public class FireBreathController : MonoBehaviour
{
    private GameObject caster;
    private Vector3 direction;
    private float duration;
    private float dps;
    private float angle;
    private float range;
    private string targetTag;

    public void Initialize(GameObject user, Vector3 dir, float dur, float damagePerSec, float unusedTick, float cone, float dist, string tag)
    {
        caster = user;
        direction = dir.normalized;
        duration = dur;
        dps = damagePerSec;
        angle = cone;
        range = dist;
        targetTag = tag;

        transform.position = caster.transform.position;

        float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleZ);

        StartCoroutine(DoFlame());
    }

    private IEnumerator DoFlame()
    {
        int ticks = Mathf.FloorToInt(duration);
        for (int i = 0; i < ticks; i++)
        {
            DamageEnemies();
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }

    private void DamageEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                Vector3 toTarget = hit.transform.position - caster.transform.position;
                float angleToTarget = Vector3.Angle(direction, toTarget);

                if (angleToTarget <= angle / 2f)
                {
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(Mathf.RoundToInt(dps));
                    }
                }
            }
        }
    }
}