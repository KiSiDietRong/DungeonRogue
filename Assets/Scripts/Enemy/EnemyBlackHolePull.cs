using UnityEngine;

public class EnemyBlackHolePull : MonoBehaviour
{
    private bool beingPulled = false;
    private Vector2 pullTarget;
    private float pullSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (beingPulled)
        {
            Vector2 dir = (pullTarget - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * pullSpeed * Time.deltaTime);
            return;
        }
    }
    public void PullTowards(Vector2 center)
    {
        pullTarget = center;
        beingPulled = true;
    }

    public void StopPull()
    {
        beingPulled = false;
    }
}
