using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SandStormController : MonoBehaviour
{
    [HideInInspector] public float slowAmount;
    [HideInInspector] public float slowDuration;
    [HideInInspector] public float zoneRadius;
    [HideInInspector] public float zoneLifetime;

    private List<Enemy> enemiesInZone = new List<Enemy>();

    void Start()
    {
        Destroy(gameObject, zoneLifetime);
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, zoneRadius);
        List<Enemy> currentEnemies = new List<Enemy>();

        foreach (Collider2D hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e != null)
            {
                currentEnemies.Add(e);
                if (!enemiesInZone.Contains(e))
                {
                    e.moveSpeed *= (1 - slowAmount);
                    enemiesInZone.Add(e);
                }
            }
        }

        for (int i = enemiesInZone.Count - 1; i >= 0; i--)
        {
            if (!currentEnemies.Contains(enemiesInZone[i]))
            {
                Enemy e = enemiesInZone[i];
                StartCoroutine(ResetSpeedAfterDelay(e));
                enemiesInZone.RemoveAt(i);
            }
        }
    }

    private System.Collections.IEnumerator ResetSpeedAfterDelay(Enemy e)
    {
        yield return new WaitForSeconds(slowDuration);
        if (e != null)
            e.moveSpeed /= (1 - slowAmount);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}