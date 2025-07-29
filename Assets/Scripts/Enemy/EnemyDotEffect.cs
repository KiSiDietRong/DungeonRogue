using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    private int damagePerSecond;
    private float duration;

    private float timer = 0f;
    private float tickTimer = 1f;

    private Enemy enemy;

    private bool isInitialized = false;

    public void Init(int dps, float dur)
    {
        damagePerSecond = dps;
        duration = dur;
        isInitialized = true;

        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning("No EnemyController found on this object for DOT.");
            Destroy(this);
        }
    }

    void Update()
    {
        if (!isInitialized || enemy == null) return;

        timer += Time.deltaTime;
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f)
        {
            enemy.TakeDamage(damagePerSecond);
            tickTimer = 1f;
        }

        if (timer >= duration)
        {
            Destroy(this);
        }
    }
}
