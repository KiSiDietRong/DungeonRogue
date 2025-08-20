using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class EnemyLineConnector : MonoBehaviour
{
    public Color lineColor = Color.white;  // Màu mặc định
    public float lineWidth = 0.05f;

    private LineRenderer lineRenderer;
    private List<Transform> targets = new List<Transform>();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Nếu là Buffer thì line = đỏ, Healer thì line = xanh lá
        if (GetComponent<EnemyBuffer>() != null)
        {
            lineColor = Color.red;
        }
        else if (GetComponent<EnemyHealer>() != null)
        {
            lineColor = Color.green;
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    public void SetTargets(List<Transform> enemyTargets)
    {
        targets = enemyTargets;
    }

    void Update()
    {
        if (targets == null || targets.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // Tổng số điểm = 1 (enemy gốc) + số target
        lineRenderer.positionCount = targets.Count * 2;

        int index = 0;
        foreach (Transform t in targets)
        {
            if (t != null)
            {
                lineRenderer.SetPosition(index, transform.position);
                lineRenderer.SetPosition(index + 1, t.position);
                index += 2;
            }
        }
    }
}
