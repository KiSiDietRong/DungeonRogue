using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class ShopRandom : MonoBehaviour
{
    [Header("Relic Prefab")]
    public List<GameObject> relicDisplayPrefab; // Prefab chứa ShopRelicDisplay, UI, Collider

    [Header("Slot chứa item (6 bàn)")]
    public List<Transform> itemSlots;

    void Start()
    {
        SpawnShopItems();
    }

    void SpawnShopItems()
    {
        // Lọc prefab dựa trên relicData chưa được dùng
        List<GameObject> availablePrefabs = new List<GameObject>();

        foreach (GameObject prefab in relicDisplayPrefab)
        {
            ShopRelicDisplay display = prefab.GetComponent<ShopRelicDisplay>();
            if (display != null && display.relicData != null)
            {
                if (!InventoryManager.Instance.IsRelicUsed(display.relicData))
                {
                    availablePrefabs.Add(prefab);
                }
            }
        }

        Utility.Shuffle(availablePrefabs);

        for (int i = 0; i < itemSlots.Count && i < availablePrefabs.Count; i++)
        {
            GameObject prefab = availablePrefabs[i];
            GameObject relicInstance = Instantiate(prefab, itemSlots[i].position, Quaternion.identity, itemSlots[i]);

            relicInstance.transform.localPosition = Vector3.zero;
            relicInstance.transform.localRotation = Quaternion.identity;
            relicInstance.transform.localScale = Vector3.one;
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

    public static class Utility
    {
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rnd = Random.Range(0, i + 1);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
    }
}
