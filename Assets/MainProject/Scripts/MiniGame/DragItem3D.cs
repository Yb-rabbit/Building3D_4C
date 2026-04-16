using UnityEngine;

public class DragItem3D : MonoBehaviour
{
    [Tooltip("目标放置位置的Transform")]
    public Transform targetArea;
    
    [Tooltip("判定为放置成功的距离阈值")]
    public float snapDistance = 1.5f;

    private Vector3 originalPosition; // 初始3D坐标
    private Camera mainCam;
    private bool isPlaced = false;    // 防止重复放置
    
    // 拖拽时用于计算的平面（基于物体初始Y轴高度的水平面）
    private Plane dragPlane;

    void Start()
    {
        mainCam = Camera.main;
        originalPosition = transform.position;
        // 初始化拖拽平面：过初始位置，法线朝上
        dragPlane = new Plane(Vector3.up, originalPosition);
    }

    void OnMouseDown()
    {
        if (isPlaced) return;
    }

    void OnMouseDrag()
    {
        if (isPlaced) return;

        // 将鼠标屏幕坐标转化为射线
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        
        // 射线与水平面求交点
        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPos = ray.GetPoint(distance);
            // 保持物体原本的Y轴高度不变，防止物体陷入地面或上下乱飞
            targetPos.y = originalPosition.y; 
            transform.position = targetPos;
        }
    }

    void OnMouseUp()
    {
        if (isPlaced) return;

        // 判断距离目标点是否足够近
        if (targetArea != null && Vector3.Distance(transform.position, targetArea.position) <= snapDistance)
        {
            PlaceItem();
        }
        else
        {
            // 失败回弹到原位
            ResetItem();
        }
    }

    private void PlaceItem()
    {
        isPlaced = true;
        
        // 1. 精准吸附到目标位置
        transform.position = targetArea.position;
        transform.rotation = targetArea.rotation;

        // 2. 禁用自身的碰撞器，防止干扰后续其他物体的射线检测
        GetComponent<Collider>().enabled = false;

        // 3. 通知管理器
        MiniGameManager.Instance.OnItemPlaced();
    }

    private void ResetItem()
    {
        // 插值Lerp回到原位
        transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
    }

    // 如果小游戏重开，需要提供重置方法
    public void ResetForNewGame()
    {
        isPlaced = false;
        transform.position = originalPosition;
        GetComponent<Collider>().enabled = true;
    }
}