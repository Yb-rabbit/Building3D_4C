using UnityEngine;

public class MovableItem : MonoBehaviour
{
    [Header("目标容器")]
    public Container targetContainer; // 关联的篮子（拖入场景中的篮子物体）
    [Header("距离阈值")]
    public float distanceThreshold = 2f; // 与容器的最大距离（小于此值才能放下）
    [Header("检测距离的物体")]
    public Transform checkObject; // 指定检测距离的物体（如玩家，可在Inspector中拖入）
    [Header("状态")]
    public bool isCarrying = false; // 是否被拿起（隐藏状态）

    /// <summary>
    /// 初始化：若未指定检测对象，自动获取玩家
    /// </summary>
    void Start()
    {
        if (checkObject == null)
        {
            // 尝试通过Tag获取玩家（需确保玩家有"Player" Tag）
            checkObject = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (checkObject == null)
            {
                Debug.LogError("未找到带'Player' Tag的物体，请手动指定checkObject！");
            }
        }
    }

    /// <summary>
    /// 拿起石头（第一次E键）
    /// </summary>
    public void PickUp()
    {
        if (!isCarrying)
        {
            isCarrying = true;
            gameObject.SetActive(false); // 隐藏石头
            Debug.Log($"{gameObject.name} 已拿起（隐藏）");
        }
    }

    /// <summary>
    /// 放下石头（第二次E键，需满足检测对象与容器的距离条件）
    /// </summary>
    public void PlaceItem()
    {
        if (isCarrying && targetContainer != null && checkObject != null)
        {
            // 计算检测对象与容器的距离
            float distance = Vector3.Distance(checkObject.position, targetContainer.placementPosition);
            if (distance < distanceThreshold)
            {
                isCarrying = false;
                gameObject.SetActive(true); // 显示石头
                transform.position = targetContainer.placementPosition; // 放到容器位置
                targetContainer.AddItem(this); // 通知容器
                Debug.Log($"{gameObject.name} 已放下到 {targetContainer.gameObject.name}");
            }
            else
            {
                Debug.Log($"{checkObject.name} 与容器的距离 {distance:F2} 超过阈值 {distanceThreshold:F2}，无法放下");
            }
        }
    }
}
