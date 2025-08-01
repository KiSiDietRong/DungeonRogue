using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MapController.Instance.TryLoadNextMapIfEnemiesCleared();
        }
    }
}
