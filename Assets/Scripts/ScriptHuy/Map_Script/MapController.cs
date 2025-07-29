using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("Map Sảnh Chờ")]
    public GameObject lobbyMap;

    [Header("List Prefabs Map Chiến Đấu")]
    public List<GameObject> battleMapPrefabs;

    [Header("Prefab Map Miniboss")]
    public GameObject minibossMapPrefab;

    [Header("Vị trí spawn map")]
    public Transform spawnPoint;

    [Header("Transition Settings")]
    public RectTransform transitionPanel;
    public float transitionDuration = 1f;

    [Header("Prefab Map Shop")]
    public GameObject shopMapPrefab;

    private Queue<GameObject> mapQueue = new Queue<GameObject>();
    private int currentMapIndex = 0;
    private GameObject currentMapInstance;

    private bool isLoading = false;

    void Start()
    {
        // Không làm gì ở Start
    }

    public void StartBattleSequence()
    {
        Debug.Log("Khởi động chuỗi battle...");

        // KHÔNG ẩn lobbyMap ở đây nữa
        GenerateRandomMapSequence();
        LoadNextMap();
    }

    void GenerateRandomMapSequence()
    {
        List<GameObject> tempList = new List<GameObject>(battleMapPrefabs);

        for (int i = 0; i < 5; i++)
        {
            int randIndex = Random.Range(0, tempList.Count);
            mapQueue.Enqueue(tempList[randIndex]);
            tempList.RemoveAt(randIndex);
        }

        mapQueue.Enqueue(minibossMapPrefab); // map thứ 6
        mapQueue.Enqueue(shopMapPrefab);     // map thứ 7

        Debug.Log("Đã tạo queue map gồm 5 map chiến đấu + miniboss + shop.");
    }

    public void LoadNextMap()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(TransitionAndLoadMap());
    }

    System.Collections.IEnumerator TransitionAndLoadMap()
    {
        float screenWidth = Screen.width;

        // Bật panel và đưa nó ra trái
        transitionPanel.gameObject.SetActive(true);
        transitionPanel.anchoredPosition = new Vector2(-screenWidth, 0);

        // Slide vào giữa
        yield return transitionPanel.DOAnchorPos(Vector2.zero, transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        // ✅ ẨN MAP SẢNH NGAY SAU KHI PANEL CHE TOÀN MÀN HÌNH
        if (lobbyMap != null && lobbyMap.activeSelf)
        {
            lobbyMap.SetActive(false);
            Debug.Log("Map sảnh đã được ẩn (sau khi panel che toàn màn hình).");
        }

        // Xoá map cũ nếu có
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
            Debug.Log("Map cũ đã bị xoá.");
        }

        // Load map mới
        if (mapQueue.Count > 0)
        {
            GameObject nextMap = mapQueue.Dequeue();
            currentMapInstance = Instantiate(nextMap, spawnPoint.position, Quaternion.identity);
            currentMapIndex++;
            Debug.Log($"→ [Map {currentMapIndex}] đã được load: {nextMap.name}");
        }
        else
        {
            Debug.Log("🎉 Tất cả map đã được load xong (bao gồm miniboss).");
        }

        // Slide panel ra bên phải
        yield return new WaitForSeconds(0.2f);
        yield return transitionPanel.DOAnchorPos(new Vector2(screenWidth, 0), transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        // Tắt panel
        transitionPanel.gameObject.SetActive(false);

        // Đợi 5s rồi load tiếp
        yield return new WaitForSeconds(5f);
        isLoading = false;
        LoadNextMap();
    }
}
