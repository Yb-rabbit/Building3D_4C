using UnityEngine;

public class WaterGateDoor : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("门被冲开时的旋转角度")]
    public Vector3 openRotation = new Vector3(0, 0, -50);
    
    private Quaternion targetRotation;
    private bool isClosed = false; // 初始状态为开（被倒灌）

    void Start()
    {
        // 游戏开始时，门是打开的（倒灌状态）
        targetRotation = Quaternion.Euler(openRotation);
    }

    void Update()
    {
        // 每帧平滑插值旋转过去（数值5可以调，越大关门越快）
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
    }

    // 外部调用：倒灌了，把门冲开
    public void ForceOpen()
    {
        isClosed = false;
        targetRotation = Quaternion.Euler(openRotation);
    }

    // 外部调用：配重够了，把门关死
    public void CloseGate()
    {
        isClosed = true;
        targetRotation = Quaternion.Euler(Vector3.zero); // 回归0度，即关门
    }
}