using UnityEngine;
using UnityEngine.EventSystems;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance; // 单例模式

    [Tooltip("需要成功放置的总数量")]
    public int requiredAmount = 3;

    private int currentPlacedAmount = 0;

    public GameObject ActObject; //结束后要激活的物体

    void Awake()
    {
        Instance = this;
    }

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
        ActObject.SetActive(true); // 激活指定物体
        //CloseMiniGame(); // 关闭小游戏
    }

    // 打开小游戏时调用
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
