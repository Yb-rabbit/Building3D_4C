using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [Header("底图设置")]
    public RawImage mapBg;
    public float mapWorldSize = 100f;

    [Header("UI图标")]
    public RectTransform playerDot;  
    public RectTransform targetDot;  
    
    [Header("3D物体")]
    public Transform player;
    public Transform target;

    [Header("视野设置")]
    public float viewRange = 20f;// 小地图

    void Update()
    {
        // 计算玩家在地图上的比例位置（0~1）
        float u = (player.position.x / mapWorldSize) + 0.5f;
        float v = (player.position.z / mapWorldSize) + 0.5f;

        // 计算小地图窗口占整张大图的比例
        float uvWidth = viewRange / mapWorldSize;
        float uvHeight = viewRange / mapWorldSize;

        // 动态调整UV矩形的范围（避免边缘显示错误）
        float uvX = u - uvWidth * 0.5f;
        float uvY = v - uvHeight * 0.5f;

        // 限制UV矩形的范围在0~1之间（避免超出地图边界）
        if (uvX < 0) uvX = 0;
        if (uvX + uvWidth > 1) uvX = 1 - uvWidth;
        if (uvY < 0) uvY = 0;
        if (uvY + uvHeight > 1) uvY = 1 - uvHeight;

        // 设置UV矩形
        mapBg.uvRect = new Rect(uvX, uvY, uvWidth, uvHeight);

        // 红点始终在正中心
        playerDot.anchoredPosition = Vector2.zero;

        // 蓝点位置计算（不变）
        Vector3 offset = target.position - player.position;
        offset.y = 0;
        float perfectScale = mapBg.rectTransform.sizeDelta.x / viewRange;
        Vector2 uiPos = new Vector2(offset.x, offset.z) * perfectScale;
        float halfSize = mapBg.rectTransform.sizeDelta.x * 0.5f - 5f;
        uiPos = Vector2.ClampMagnitude(uiPos, halfSize);
        targetDot.anchoredPosition = uiPos;
    }
}
