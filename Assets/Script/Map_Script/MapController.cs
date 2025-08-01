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

    [Header("Vị trí spawn map (tự động tìm theo tên 'SpawnPoint')")]
    private Transform spawnPoint;

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
            return;
        }

        // Tìm SpawnPoint trong scene
        GameObject found = GameObject.Find("SpawnPoint");
        if (found != null)
        {
            spawnPoint = found.transform;
            Debug.Log("Đã tìm thấy SpawnPoint.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy SpawnPoint! Hãy đảm bảo đã đặt tên đúng trong scene.");
        }
    }

    public void StartBattleSequence()
    {
        Debug.Log("Khởi động chuỗi battle...");

        GenerateSequentialMapSequence();
        LoadNextMap();
    }

    void GenerateSequentialMapSequence()
    {
        foreach (var map in battleMapPrefabs)
        {
            mapQueue.Enqueue(map);
        }

        mapQueue.Enqueue(minibossMapPrefab);
        mapQueue.Enqueue(shopMapPrefab);

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

        transitionPanel.gameObject.SetActive(true);
        transitionPanel.anchoredPosition = new Vector2(-screenWidth, 0);

        yield return transitionPanel.DOAnchorPos(Vector2.zero, transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        if (lobbyMap != null && lobbyMap.activeSelf)
        {
            lobbyMap.SetActive(false);
            Debug.Log("Map sảnh đã được ẩn (sau khi panel che toàn màn hình).");
        }

        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
            Debug.Log("Map cũ đã bị xoá.");
        }

        if (mapQueue.Count > 0)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("SpawnPoint chưa được gán! Không thể load map.");
                yield break;
            }

            GameObject nextMap = mapQueue.Dequeue();
            currentMapInstance = Instantiate(nextMap, spawnPoint.position, Quaternion.identity);
            currentMapIndex++;
            Debug.Log($"→ [Map {currentMapIndex}] đã được load: {nextMap.name}");
        }
        else
        {
            Debug.Log("🎉 Tất cả map đã được load xong (bao gồm miniboss và shop).");
        }

        yield return new WaitForSeconds(0.2f);
        yield return transitionPanel.DOAnchorPos(new Vector2(screenWidth, 0), transitionDuration)
            .SetEase(Ease.InOutQuad).WaitForCompletion();

        transitionPanel.gameObject.SetActive(false);
        isLoading = false;
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
