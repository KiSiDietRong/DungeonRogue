using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [Header("Map Sảnh Chờ")]
    public GameObject lobbyMap;

    [Header("List Prefabs Map Chiến Đấu (theo thứ tự)")]
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Không làm gì ở Start
    }

    public void StartBattleSequence()
    {
        Debug.Log("Khởi động chuỗi battle...");

        GenerateSequentialMapSequence();
        LoadNextMap();
    }

    /// <summary>
    /// Tạo queue map theo đúng thứ tự từ battleMapPrefabs, sau đó là miniboss và shop
    /// </summary>
    void GenerateSequentialMapSequence()
    {
        // Thêm các map chiến đấu theo thứ tự
        foreach (var map in battleMapPrefabs)
        {
            mapQueue.Enqueue(map);
        }

        // Cuối cùng thêm miniboss và shop
        mapQueue.Enqueue(minibossMapPrefab); // map thứ n+1
        mapQueue.Enqueue(shopMapPrefab);     // map thứ n+2

        Debug.Log("Đã tạo queue map theo thứ tự: các map chiến đấu → miniboss → shop.");
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

        // Slide panel vào giữa
        yield return transitionPanel.DOAnchorPos(Vector2.zero, transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        // Ẩn lobby nếu đang hiển thị
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

        // Load map mới từ queue
        if (mapQueue.Count > 0)
        {
            GameObject nextMap = mapQueue.Dequeue();
            currentMapInstance = Instantiate(nextMap, spawnPoint.position, Quaternion.identity);
            currentMapIndex++;
            Debug.Log($"→ [Map {currentMapIndex}] đã được load: {nextMap.name}");
        }
        else
        {
            Debug.Log("🎉 Tất cả map đã được load xong (bao gồm miniboss và shop).");
        }

        // Slide panel ra bên phải
        yield return new WaitForSeconds(0.2f);
        yield return transitionPanel.DOAnchorPos(new Vector2(screenWidth, 0), transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        // Tắt panel
        transitionPanel.gameObject.SetActive(false);
        isLoading = false; // Reset trạng thái
        yield break;
    }

    public void TryLoadNextMapIfEnemiesCleared()
    {
        if (EnemySpawner.Instance.AllEnemiesCleared())
        {
            Debug.Log("Tất cả enemy đã bị tiêu diệt, tiến hành load map mới.");
            LoadNextMap();
        }
        else
        {
            Debug.Log("Chưa tiêu diệt hết enemy, không load map.");
        }
    }
}
