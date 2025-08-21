using UnityEngine;

public class MeteorExplotionDestroy : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, duration);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }
}
