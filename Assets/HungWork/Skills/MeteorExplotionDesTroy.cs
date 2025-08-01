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
            // fallback nếu không có Animator
            Destroy(gameObject, 1f);
        }
    }
}
