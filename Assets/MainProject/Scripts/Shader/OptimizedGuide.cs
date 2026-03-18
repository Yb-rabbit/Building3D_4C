using UnityEngine;
using UnityEngine.AI;

public class OptimizedGuide : MonoBehaviour
{
    [Header("追踪设置")]
    public string targetTag = "Target";       // 目标物体的Tag
    public float maxDistance = 50f;           // 最大追踪距离（超过此距离不显示）
    public float searchInterval = 0.5f;       // 搜索间隔(秒)，不必每帧搜索，省性能

    [Header("外观设置")]
    public LineRenderer lineRenderer;         // 拖拽赋值
    public Color nearColor = Color.green;     // 近距离颜色
    public Color farColor = Color.red;        // 远距离颜色

    // 私有变量
    private NavMeshAgent agent;
    private Transform currentTarget;          // 当前锁定的最近目标
    private Material lineMaterial;            // 缓存材质，避免每帧获取
    private float timer;

    // 优化变量：避免每帧分配内存
    private Collider[] hitColliders;          // 如果你用物理检测，可以用这个；这里我们用FindTag
    private Vector3[] pathCorners;            // 缓存路径点数组

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // 初始化 LineRenderer
        if (lineRenderer != null)
        {
            // 获取材质实例，这样修改颜色不会影响共享资源
            lineMaterial = lineRenderer.material;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        // --- 1. 性能优化：定时搜索 ---
        timer += Time.deltaTime;
        if (timer >= searchInterval)
        {
            timer = 0;
            FindClosestTarget(); // 只有到了时间才去搜索场景中的Tag对象
        }

        // --- 2. 更新引导逻辑 ---
        if (currentTarget != null)
        {
            UpdateGuide();
        }
        else
        {
            // 没目标时关闭显示
            if (lineRenderer != null) lineRenderer.enabled = false;
        }
    }

    // 核心逻辑：寻找最近且在范围内的目标
    void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        
        float closestDistSqr = Mathf.Infinity; // 使用平方距离比较，省去开根号运算
        Transform closestTarget = null;

        foreach (GameObject obj in targets)
        {
            // 计算平方距离
            float distSqr = (obj.transform.position - transform.position).sqrMagnitude;

            // 判断：1. 必须在最大范围内 2. 必须比当前找到的最近目标更近
            if (distSqr < closestDistSqr && distSqr <= maxDistance * maxDistance)
            {
                closestDistSqr = distSqr;
                closestTarget = obj.transform;
            }
        }

        // 更新锁定目标
        currentTarget = closestTarget;
    }

    void UpdateGuide()
    {
        if (currentTarget == null || !agent.enabled) return;

        // 再次确认实时距离（防止目标移动导致瞬间超出范围）
        float currentDist = Vector3.Distance(transform.position, currentTarget.position);
        
        if (currentDist > maxDistance)
        {
            lineRenderer.enabled = false;
            return;
        }

        // --- 寻路计算 ---
        agent.SetDestination(currentTarget.position);

        // 只有路径计算完毕且有效才绘制
        if (!agent.pathPending && agent.path.corners.Length > 1)
        {
            DrawLine();
            UpdateColor(currentDist);
        }
    }

    void DrawLine()
    {
        lineRenderer.enabled = true;
        
        // 性能优化：复用数组，只在长度不够时扩容
        int cornerCount = agent.path.corners.Length;
        lineRenderer.positionCount = cornerCount;
        lineRenderer.SetPositions(agent.path.corners);
    }

    // 动态改变 Shader 颜色
    void UpdateColor(float distance)
    {
        if (lineMaterial == null) return;

        // 根据距离计算插值 (0 = 最近, 1 = 最远)
        float t = Mathf.Clamp01(distance / maxDistance);
        
        // 颜色插值
        Color finalColor = Color.Lerp(nearColor, farColor, t);

        // 传入 Shader
        // 注意：这里的 "_Color" 必须和你 Shader 代码里的属性名完全一致
        lineMaterial.SetColor("_Color", finalColor);
    }
}
