using UnityEngine;
using UnityEngine.AI; // 引入寻路命名空间

public class GuideController : MonoBehaviour
{
    [Header("设置")]
    public Transform targetB; // 物体B
    public LineRenderer lineRenderer; // 用于画线的组件

    private NavMeshAgent agent;
    private float totalDistance = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // 初始化 LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = 0.5f;
            lineRenderer.endWidth = 0.5f;
            lineRenderer.positionCount = 0;
        }
    }

    void Update()
    {
        if (targetB == null) return;

        // 1. 设置寻路目标
        agent.SetDestination(targetB.position);

        // 2. 获取路径并绘制
        DrawPath();
    }

    void DrawPath()
    {
        if (agent.path == null || agent.path.corners.Length < 2) return;

        // 获取路径拐角点
        Vector3[] corners = agent.path.corners;
        
        // 设置 LineRenderer 的点数和位置
        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }

    // 辅助方法：计算路径真实长度
    float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2) return length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }
}
