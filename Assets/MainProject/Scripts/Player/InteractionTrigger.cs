using UnityEngine;
using StarterAssets;

public class InteractionTrigger : MonoBehaviour
{
    // 拖入玩家身上的输入脚本
    public StarterAssetsInputs inputController; 
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 玩家标签
        {
            // 进入区域：解锁鼠标，显示光标，停止视角转动
            inputController.SetCursorState(false);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 离开区域：锁定鼠标，隐藏光标，恢复视角转动
            inputController.SetCursorState(true);
        }
    }
}
