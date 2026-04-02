using UnityEngine;

public class PuzzlePiece3D : MonoBehaviour
{
    [HideInInspector] public Vector3 correctPosition; // 正确的世界坐标
    [HideInInspector] public bool isPlaced = false;

    private float snapDistance = 0.5f; // 吸附距离阈值
    private Vector3 offset;           // 鼠标点击点与物体中心的偏移量
    private float objectY;            // 固定拖拽时的Y轴高度，防止飞到天上去

    void Start()
    {
        // 记录初始的Y高度（也就是薄块的厚度），拖拽时保持这个高度
        objectY = transform.position.y; 
    }

    // 鼠标按下时（需要物体有Collider）
    void OnMouseDown()
    {
        if (isPlaced) return;

        // 计算鼠标射线与物体表面的交点，记录偏移量，这样拖拽时物体不会瞬间跳到鼠标中心
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            offset = transform.position - hit.point;
        }
    }

    // 鼠标拖动时
    void OnMouseDrag()
    {
        if (isPlaced) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 创建一个与Y轴平行的平面，高度为objectY，用于拦截射线
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, objectY, 0));
        
        if (dragPlane.Raycast(ray, out float distance))
        {
            // 获取射线在平面上的交点，加上偏移量，更新物体位置
            Vector3 targetPos = ray.GetPoint(distance) + offset;
            transform.position = new Vector3(targetPos.x, objectY, targetPos.z);
        }
    }

    // 鼠标松开时
    void OnMouseUp()
    {
        if (isPlaced) return;

        CheckPlacement();
    }

    void CheckPlacement()
    {
        // 计算当前X和Z坐标与正确坐标的距离（忽略Y轴，因为Y是固定的）
        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z), 
            new Vector2(correctPosition.x, correctPosition.z)
        );

        if (distance < snapDistance)
        {
            // 吸附到完美位置
            transform.position = correctPosition;
            isPlaced = true;
            
            // 通知管理器
            PuzzleManager3D.instance.OnPiecePlaced();
        }
    }
}
