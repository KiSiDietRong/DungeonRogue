using UnityEngine;

public class EnemyBlackHolePull : MonoBehaviour
{
    private bool beingPulled = false;
    private Vector2 pullTarget;
    private float pullSpeed;

    void Update()
    {
        if (beingPulled)
        {
            Vector2 dir = (pullTarget - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * pullSpeed * Time.deltaTime);
        }
    }

    public void PullTowards(Vector2 center, float speed)
    {
        pullTarget = center;
        pullSpeed = speed;
        beingPulled = true;
    }

    public void StopPull()
    {
        beingPulled = false;
    }
}
