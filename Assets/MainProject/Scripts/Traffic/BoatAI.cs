using UnityEngine;

public class BoatAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float maxTravelDistance = 100f;

    [Header("漂浮设置 (正弦波动)")]
    public float bobbingHeight = 0.5f;    // 上下漂浮的高度幅度
    public float bobbingSpeed = 2f;       // 漂浮的速度

    [Header("检测设置")]
    public float rayDistance = 5f;
    
    private bool shouldStop = false;
    private Vector3 startPosition;
    private bool isDestroyed = false; // 防止重复销毁

    public static int TotalBoatCount = 0;
    public static int MaxBoatLimit = 10;

    private float randomOffset; // 随机偏移量，让每艘船的漂浮节奏不同步

    void Start()
    {
        startPosition = transform.position;
        TotalBoatCount++;
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        if (isDestroyed) return;

        // 行驶距离限制
        if (Vector3.Distance(transform.position, startPosition) > maxTravelDistance)
        {
            DestroyBoat();
            return;
        }

        CheckObstacles();

        // 移动逻辑 (水平移动 + 垂直漂浮)
        if (!shouldStop)
        {
            // A. 水平前进
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);

            // B. 正弦漂浮 (上下位移)
            // 使用数学库计算正弦波
            float yOffset = Mathf.Sin((Time.time + randomOffset) * bobbingSpeed) * bobbingHeight;
            
            // 保持原有的 X 和 Z，修改 Y 值实现漂浮
            // 注意：这里基于 startPosition.y 作为基准水面
            Vector3 newPos = transform.position;
            newPos.y = startPosition.y + yOffset;
            transform.position = newPos;
        }
    }

    void CheckObstacles()
    {
        shouldStop = false;
        RaycastHit hit;
        
        // 射线起点稍微抬高，避免射到水面或船体底部
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f + transform.forward * 1f;

        // 射线检测
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance + moveSpeed * 0.5f))
        {
            if (hit.collider.CompareTag("Boat"))
            {
                shouldStop = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;

        // 如果撞到了其他船只
        if (other.CompareTag("Boat"))
        {
            // 销毁对方
            BoatAI otherBoat = other.GetComponent<BoatAI>();
            if (otherBoat != null && !otherBoat.isDestroyed)
            {
                otherBoat.DestroyBoat();
            }
            // 销毁自己
            DestroyBoat();
        }
    }

    public void DestroyBoat()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        TotalBoatCount--;
        Destroy(gameObject); 
    }
}
