using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("Map Sảnh Chờ")]
    public GameObject lobbyMap; // Map sảnh (để ẩn sau khi vào battle)

    [Header("List Prefabs Map Chiến Đấu")]
    public List<GameObject> battleMapPrefabs; // 5 prefab màn chiến đấu

    [Header("Prefab Map Miniboss")]
    public GameObject minibossMapPrefab;

    [Header("Vị trí spawn map")]
    public Transform spawnPoint;

    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private int currentMapIndex = 0;
    private GameObject currentMapInstance;

    private bool isLoading = false;

    void Start()
    {
        // Không làm gì ở Start - chờ player đi qua cổng
    }

    /// <summary>
    /// Gọi khi player đi vào cổng sảnh
    /// </summary>
    public void StartBattleSequence()
    {
        Debug.Log("Khởi động chuỗi battle...");

        // Ẩn map sảnh nếu có
        if (lobbyMap != null)
        {
            lobbyMap.SetActive(false);
            Debug.Log("Map sảnh đã được ẩn.");
        }

        GenerateRandomMapSequence();
        LoadNextMap();
    }

    // Tạo danh sách 5 map random + miniboss
    void GenerateRandomMapSequence()
    {
        List<GameObject> tempList = new List<GameObject>(battleMapPrefabs);

        for (int i = 0; i < 5; i++)
        {
            int randIndex = Random.Range(0, tempList.Count);
            mapQueue.Enqueue(tempList[randIndex]);
            tempList.RemoveAt(randIndex);
        }

        // Đảm bảo map miniboss là cuối cùng
        mapQueue.Enqueue(minibossMapPrefab);
        Debug.Log("Đã tạo queue map ngẫu nhiên gồm 5 map + miniboss.");
    }

    public void LoadNextMap()
    {
        if (isLoading) return;

        isLoading = true;

        // Xoá map hiện tại nếu có
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
            Debug.Log("Map cũ đã bị xoá.");
        }

        if (mapQueue.Count > 0)
        {
            GameObject nextMap = mapQueue.Dequeue();
            currentMapInstance = Instantiate(nextMap, spawnPoint.position, Quaternion.identity);
            currentMapIndex++;

            Debug.Log($"→ [Map {currentMapIndex}] đã được load: {nextMap.name}");

            // Test: Load tiếp map sau 3s
            Invoke(nameof(LoadNextMap), 3f);
        }
        else
        {
            Debug.Log("🎉 Tất cả map đã được load xong (bao gồm miniboss).");
        }

        isLoading = false;
    }
}
