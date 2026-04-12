using UnityEngine;
using UnityEngine.UI;

public class MiniMapCompass : MonoBehaviour
{
    [Header("北朝向物体")]
    public Transform northMarker; // 场景中的北朝向物体（Z轴正方向为北）

    [Header("指南针UI")]
    public RectTransform compassArrow; // 地图UI中的指南针箭头（Pivot设为底部中心）

    [Header("玩家摄像机")]
    public Transform playerCamera; // 玩家摄像机（用于获取朝向）

    void Update()
    {
        // 1. 获取northMarker的Z轴方向（作为北方向）
        Vector3 northDirection = northMarker.forward; // northMarker的Z轴正方向
        
        // 2. 获取玩家摄像机的水平朝向（Y轴旋转角度，0=北，90=东，180=南，270=西）
        float playerYaw = playerCamera.eulerAngles.y;
        
        // 3. 计算玩家朝向向量（2D平面，忽略Y轴）
        Vector3 playerForward = new Vector3(Mathf.Sin(playerYaw * Mathf.Deg2Rad), 0, Mathf.Cos(playerYaw * Mathf.Deg2Rad));
        
        // 4. 计算北方向与玩家朝向的夹角（顺时针为正）
        float angle = Vector3.SignedAngle(playerForward, northDirection, Vector3.up);
        
        // 5. 指南针箭头绕Z轴反向旋转（玩家朝北时，箭头指向北）
        compassArrow.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
