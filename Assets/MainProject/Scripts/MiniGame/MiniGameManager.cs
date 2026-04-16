using UnityEngine;
using UnityEngine.Events;

public class MiniGameManager : MonoBehaviour 
{
    // 删除了单例 Instance

    [Tooltip("需要成功放置的总数量")]
    public int requiredAmount = 3;
    
    private int currentPlacedAmount = 0;

    [Header("完成事件")]
    [Tooltip("小游戏完成后触发的事件")]
    public UnityEvent FinishBoat;

    // DragItem 调用
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
        FinishBoat?.Invoke();
        //CloseMiniGame();
    }

    // 打开小游戏时调用（重置当前实例的计数）
    public void OpenMiniGame() 
    {
        currentPlacedAmount = 0;
        gameObject.SetActive(true);
    }

    // 关闭小游戏时调用
    public void CloseMiniGame() 
    {
        gameObject.SetActive(false);
    }
}