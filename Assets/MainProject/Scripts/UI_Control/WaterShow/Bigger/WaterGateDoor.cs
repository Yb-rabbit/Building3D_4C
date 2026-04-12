using UnityEngine;

public class WaterGateDoor : MonoBehaviour 
{
    [Header("旋转设置")]
    [Tooltip("门被冲开时的旋转角度")]
    public Vector3 openRotation = new Vector3(0, 0, -50);
    
    private Quaternion targetRotation;
    private bool isClosed = false;

    void Start() 
    {
        // 游戏开始时，门是打开的（倒灌状态）
        targetRotation = Quaternion.Euler(openRotation);
    }

    void Update() 
    {
        // 每帧平滑插值旋转过去
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    // 外部调用：倒灌了，把门冲开
    public void ForceOpen() 
    {
        isClosed = false;
        targetRotation = Quaternion.Euler(openRotation);
    }

    public void CloseGate(int currentCount, int totalCount) 
    {
        if (totalCount <= 0) return;

        isClosed = true;
        
        // 计算当前进度
        float progress = (float)currentCount / totalCount;
        Vector3 newTargetAngle = openRotation * (1.0f - progress);
        
        targetRotation = Quaternion.Euler(newTargetAngle);
    }
}