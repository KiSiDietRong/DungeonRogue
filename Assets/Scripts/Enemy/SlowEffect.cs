using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    private Enemy Enemy;
    private float originalSpeed;

    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        if (Enemy != null)
        {
            originalSpeed = Enemy.moveSpeed;
        }
    }

    public void ApplySlow(float slowAmount)
    {
        if (Enemy != null)
        {
            Enemy.moveSpeed = originalSpeed * (1f - slowAmount);
        }
    }

    public void RemoveSlow()
    {
        if (Enemy != null)
        {
            Enemy.moveSpeed = originalSpeed;
        }
    }
}
