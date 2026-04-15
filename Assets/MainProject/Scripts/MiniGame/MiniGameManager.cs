using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance; // 单例模式方便调用

    [Tooltip("需要成功放置的总数量")]
    public int requiredAmount = 3;

    private int currentPlacedAmount = 0;

    void Awake()
    {
        Instance = this;
    }

    // 由 DragItem 调用
    public void OnItemPlaced()
    {
        currentPlacedAmount++;

        if (currentPlacedAmount >= requiredAmount)
        {
            OnMiniGameWin();
        }
    }

    private void OnMiniGameWin()
    {
        Debug.Log("小游戏胜利！");
        // 1. 播放胜利动画/UI
        // 2. 给3D游戏发送奖励或触发剧情
        // 3. 关闭小游戏UI
        // 4. 恢复3D角色控制（见第四步）
    }

    // 打开小游戏时调用
    public void OpenMiniGame()
    {
        currentPlacedAmount = 0;
        gameObject.SetActive(true);
        // 在这里禁用3D角色的控制脚本
        // 例如：ThirdPersonController.enabled = false;
    }

    // 关闭小游戏时调用
    public void CloseMiniGame()
    {
        gameObject.SetActive(false);
        // 恢复3D角色的控制脚本
        // 例如：ThirdPersonController.enabled = true;
    }
}
