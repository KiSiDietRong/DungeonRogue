using System.Collections.Generic;
using UnityEngine;

public class ShopRandom : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> relicPrefabs; // Danh sách các Relic
    public List<GameObject> skillPrefabs; // Danh sách các Skill

    [Header("Slot chứa item (6 bàn)")]
    public List<Transform> itemSlots; // Slot0 đến Slot5

    void Start()
    {
        SpawnShopItems();
    }

    void SpawnShopItems()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // 50% relic, 50% skill
            bool isRelic = Random.value < 0.5f;

            GameObject prefabToSpawn;

            if (isRelic)
            {
                int randIndex = Random.Range(0, relicPrefabs.Count);
                prefabToSpawn = relicPrefabs[randIndex];
            }
            else
            {
                int randIndex = Random.Range(0, skillPrefabs.Count);
                prefabToSpawn = skillPrefabs[randIndex];
            }

            Instantiate(prefabToSpawn, itemSlots[i].position, Quaternion.identity, itemSlots[i]);
        }
    }

    public void RerollItems()
    {
        foreach (Transform slot in itemSlots)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }

        // Spawn lại
        SpawnShopItems();
    }
}
