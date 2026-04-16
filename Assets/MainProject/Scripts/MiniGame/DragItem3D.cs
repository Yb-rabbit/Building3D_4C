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

    [Tooltip("此物品所属的小游戏管理器")]
    public MiniGameManager myManager;

    private Vector3 originalPosition; 
    private Camera mainCam;
    private bool isPlaced = false;    
    private bool isResetting = false; 

    void Start()
    {
        mainCam = Camera.main;
        originalPosition = transform.position;

        if (myManager == null)
        {
            myManager = GetComponentInParent<MiniGameManager>();
        }
    }

    void OnMouseDown()
    {
        if (isPlaced) return;

        // 如果正在平滑回弹，点击它时打断回弹，直接接着拖
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
        
        // 平面
        Plane horizontalPlane = new Plane(Vector3.up, originalPosition);

        if (horizontalPlane.Raycast(ray, out float distance))
        {
            // 获取射线与水平面相交的点
            Vector3 hitPoint = ray.GetPoint(distance);
            
            // 保持在 X-Z 平面滑动
            hitPoint.y = originalPosition.y;
            transform.position = hitPoint;
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

        // 禁用碰撞器
        GetComponent<Collider>().enabled = false;

        if (myManager != null)
        {
            myManager.OnItemPlaced();
        }
        else
        {
            Debug.LogError($"3D物体 {gameObject.name} 未找到对应的 MiniGameManager！", this);
        }
    }

    // 平滑回弹逻辑 (完全保留原有平滑功能)
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
            float progress = Mathf.Clamp01(elapsed / resetDuration);
            
            // 先快后慢的平滑曲线
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            
            // 插值位置
            Vector3 currentPos = Vector3.Lerp(startPos, originalPosition, smoothProgress);
            
            // 回弹过程中也强制锁定 Y 轴，防止万一出现的浮空/下沉
            currentPos.y = originalPosition.y;
            transform.position = currentPos;

            yield return null; 
        }

        // 确保最终精准归位
        transform.position = originalPosition;
        isResetting = false;
    }

    // 提供给外部（如重置小游戏时）调用的方法
    public void ResetForNewGame()
    {
        StopAllCoroutines();
        isPlaced = false;
        isResetting = false;
        transform.position = originalPosition;
        GetComponent<Collider>().enabled = true;
    }
}
