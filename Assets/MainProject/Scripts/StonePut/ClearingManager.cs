using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class ClearingManager : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("需要清理的坏石头总数")]
    public int targetCount = 4;
    private int currentCount = 0;

    [Header("UI绑定")]
    [Tooltip("场景中显示积分的Text物体")]
    public Text scoreText; 

    [Header("相机与鼠标")]
    [Tooltip("清理时切换到的俯视/特写Cinemachine相机")]
    public CinemachineVirtualCamera clearCamera;
    
    // 记录游戏原本的鼠标状态，方便结束后恢复
    private bool originalCursorState;

    [Header("完成后的激活列表")]
    [Tooltip("清理完成后要激活的所有物体（水流特效、NPC、铭文砖等）")]
    public List<GameObject> objectsToActivateOnComplete;

    // 外部调用这个方法来开始清理小游戏（比如NPC对话完后调用）
    public void StartClearing()
    {
        currentCount = 0;
        UpdateUI();

        // 1. 显示并解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 激活清理专用相机 (确保在Inspector里把这个相机的Priority调到最高，比如100)
        if (clearCamera != null)
        {
            clearCamera.gameObject.SetActive(true);
        }
    }

    // 石头被点击后，会调用这个方法加分
    public void AddScore()
    {
        currentCount++;
        UpdateUI();

        // 检查是否完成
        if (currentCount >= targetCount)
        {
            OnClearComplete();
        }
    }

    // 更新UI文本
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"已清除填充物: {currentCount} / {targetCount}";
        }
    }

    // 清理完成逻辑
    private void OnClearComplete()
    {
        // 1. 隐藏UI文本
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        // 2. 关闭清理相机（如果你的主相机是跟着Cinemachine Brain走的，关掉这个就会自动切回去）
        if (clearCamera != null)
        {
            clearCamera.gameObject.SetActive(false);
        }

        // 3. 恢复鼠标状态（根据你游戏原本的设定改，这里暂时给个自由状态）
        Cursor.lockState = CursorLockMode.None; 
        
        // 4. 激活所有后续物体
        foreach (var obj in objectsToActivateOnComplete)
        {
            if (obj != null) obj.SetActive(true);
        }

        //Debug.Log("清淤完成，系统已就绪！");
    }
}
