using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject text;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetNearPortal(true, transform);
            text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetNearPortal(false, null);
            text.SetActive(false);
        }
    }
}
