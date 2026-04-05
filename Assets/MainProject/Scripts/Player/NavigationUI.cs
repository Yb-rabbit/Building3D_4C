using UnityEngine;
using UnityEngine.UI;

public class NavigationUI : MonoBehaviour
{
    [Header("UI引用")]
    public RectTransform panelRect;      // 导航面板的RectTransform
    public RectTransform targetDot;      // 目标点的RectTransform
    
    [Header("目标物体")]
    public Transform target;             // 要导航到的目标物体
    
    [Header("参数配置")]
    public float maxDisplayDistance = 50f; // 超过这个距离，点贴边显示
    public bool clampToEdge = true;       // 是否限制在面板范围内
    
    private Vector2 panelSize;

    void Start()
    {
        // 获取面板尺寸（基于Delta尺寸）
        panelSize = panelRect.sizeDelta;
    }

    void Update()
    {
        if (target == null) return;
        
        UpdateTargetDot();
    }

    void UpdateTargetDot()
    {
        Transform player = transform;

        // 计算世界空间中的相对方向
        Vector3 dirToWorld = target.position - player.position;
        // 只取XZ平面（俯视图导航，忽略Y高度差）
        dirToWorld.y = 0;
        float distance = dirToWorld.magnitude;

        // 转换为玩家本地坐标（考虑玩家朝向）
        Vector3 localDir = player.InverseTransformDirection(dirToWorld);
        
        // 第3步：映射到UI坐标
        Vector2 uiPos = new Vector2(localDir.x, localDir.z);
        
        // 根据距离缩放
        float scale = panelSize.x * 0.5f / maxDisplayDistance;
        uiPos *= scale;

        // 是否限制在边界内（贴边效果）
        if (clampToEdge)
        {
            float radius = panelSize.x * 0.5f - 10f; // 留一点边距
            if (uiPos.magnitude > radius)
            {
                uiPos = uiPos.normalized * radius;
            }
        }
        targetDot.anchoredPosition = uiPos;
    }
}
