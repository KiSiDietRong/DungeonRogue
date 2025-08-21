using UnityEngine;

public class SwordHitCol : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.swordHitSFX);
        }
    }
}
