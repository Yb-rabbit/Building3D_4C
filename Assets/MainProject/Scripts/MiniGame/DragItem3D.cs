using UnityEngine;
using System.Collections;

public class DragItem3D : MonoBehaviour
{
    [Tooltip("目标放置位置的Transform")]
    public Transform targetArea;
    
    [Tooltip("判定为放置成功的距离阈值")]
    public float snapDistance = 1.5f;

    [Tooltip("回弹到原位所需的时间(秒)")]
    public float resetDuration = 0.3f;

    private Vector3 originalPosition; // 初始3D坐标
    private Quaternion originalRotation; // 初始旋转（防止被意外修改）
    private Camera mainCam;
    private bool isPlaced = false;    
    private bool isResetting = false; // 是否正在平滑回弹中
    
    private Plane dragPlane;

    void Start()
    {
        mainCam = Camera.main;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        // 以物体自身的"上方"为法线构建拖拽平面
        dragPlane = new Plane(transform.up, originalPosition);
    }

    void OnMouseDown()
    {
        if (isPlaced) return;

        // 如果物体正在平滑回弹，点击它时打断回弹，直接接着拖
        if (isResetting)
        {
            StopCoroutine("SmoothResetRoutine");
            isResetting = false;
        }
    }

    void OnMouseDrag()
    {
        if (isPlaced || isResetting) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        
        if (dragPlane.Raycast(ray, out float distance))
        {
            // 获取射线与斜面相交的世界坐标点
            Vector3 hitPoint = ray.GetPoint(distance);
            
            // 计算从初始位置到命中点的向量
            Vector3 offset = hitPoint - originalPosition;
            
            // 【关键修复3】：将移动向量投影到物体的平面上，剔除掉穿透平面的错误位移
            offset = Vector3.ProjectOnPlane(offset, transform.up);
            
            // 更新位置（只改变X和Z在物体自身平面上的投影，不会乱跑）
            transform.position = originalPosition + offset;
        }
    }

    void OnMouseUp()
    {
        if (isPlaced || isResetting) return;

        if (targetArea != null && Vector3.Distance(transform.position, targetArea.position) <= snapDistance)
        {
            PlaceItem();
        }
        else
        {
            // 失败，启动平滑回弹
            StartSmoothReset();
        }
    }

    private void PlaceItem()
    {
        isPlaced = true;
        
        // 吸附到目标位置和旋转
        transform.position = targetArea.position;
        transform.rotation = targetArea.rotation;

        // 禁用碰撞器，防止干扰后续其他物体的拖拽
        GetComponent<Collider>().enabled = false;

        // 通知管理器
        MiniGameManager.Instance.OnItemPlaced();
    }

    // 平滑回弹逻辑
    private void StartSmoothReset()
    {
        isResetting = true;
        StartCoroutine(SmoothResetRoutine());
    }

    private IEnumerator SmoothResetRoutine()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            // 计算当前进度 (0 到 1)
            float progress = Mathf.Clamp01(elapsed / resetDuration);
            
            // 使用 SmoothStep 让回弹有减速缓冲的效果，更自然
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            
            // 位置插值
            transform.position = Vector3.Lerp(startPos, originalPosition, smoothProgress);
            
            // 如果需要旋转也平滑恢复，可以取消下面这行的注释
            // transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, smoothProgress);

            yield return null; // 等待下一帧
        }

        // 确保最终精准归位
        transform.position = originalPosition;
        // transform.rotation = originalRotation;
        
        isResetting = false;
    }

    // 提供给外部（如重置小游戏时）调用的方法
    public void ResetForNewGame()
    {
        StopAllCoroutines(); // 停止可能正在进行的回弹
        isPlaced = false;
        isResetting = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        GetComponent<Collider>().enabled = true;
    }
}