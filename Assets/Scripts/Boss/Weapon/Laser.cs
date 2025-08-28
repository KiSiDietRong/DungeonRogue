using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public float damage = 20f;
    public float activeDelay = 1f;
    public float duration = 3f;
    private bool isActive = false;
    private Coroutine damageCoroutine;

    private void Start()
    {
        Invoke(nameof(ActivateLaser), activeDelay);

        Destroy(gameObject, duration);
    }

    private void ActivateLaser()
    {
        isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            damageCoroutine = StartCoroutine(DamageOverTime(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isActive && other.CompareTag("Player") && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageOverTime(other));
        }
    }

    private IEnumerator DamageOverTime(Collider2D player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        while (isActive && health != null)
        {
            health.TakeDamage((int)damage, transform);
            yield return new WaitForSeconds(1f);
        }
    }
}
