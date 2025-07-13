using UnityEngine;

public class Portal_Lob : MonoBehaviour
{
    public MapController mapManager;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player đã đi vào cổng. Bắt đầu load map chiến đấu.");
            mapManager.StartBattleSequence();
        }
    }
}
