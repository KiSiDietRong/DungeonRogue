using UnityEngine;

public class Item : MonoBehaviour
{
    private bool isPlayerNearby = false;
    public GameObject eIndicatorPrefab;
    private GameObject eIndicatorInstance;

    void Start()
    {
        if (eIndicatorPrefab != null)
        {
            eIndicatorInstance = Instantiate(eIndicatorPrefab, transform);
            eIndicatorInstance.transform.localPosition = new Vector3(0, 1f, 0);
            eIndicatorInstance.SetActive(false);
        }
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        isPlayerNearby = distance < 2f;

        if (eIndicatorInstance != null)
            eIndicatorInstance.SetActive(isPlayerNearby);

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenItem();
        }
    }

    void OpenItem()
    {
        Debug.Log("Opening Item...");
        FindObjectOfType<InventoryManager>().OpenCanvas();
        Destroy(gameObject);
    }
}
