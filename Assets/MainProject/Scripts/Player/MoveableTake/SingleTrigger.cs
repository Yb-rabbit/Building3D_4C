using UnityEngine;
using System.Collections;

public class SingleTrigger : MonoBehaviour
{
    [Header("关联的可移动物体")]
    public MovableItem movableItem; // 拖入当前MovableItem物体
    [Header("触发键")]
    public KeyCode triggerKey = KeyCode.E; // 触发键（如E）
    [Header("UI提示")]
    public GameObject pickUpPrompt; // 拿起提示（如“按E拿起”）
    public GameObject placePrompt; // 放下提示（如“按E放下”）

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 根据当前状态显示对应提示
            if (!movableItem.isCarrying)
            {
                pickUpPrompt.SetActive(true); // 显示“拿起”提示
            }
            else
            {
                placePrompt.SetActive(true); // 显示“放下”提示
            }
            StartCoroutine(WaitForInput());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpPrompt.SetActive(false);
            placePrompt.SetActive(false);
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// 等待玩家按键触发（拿起/放下）
    /// </summary>
    private IEnumerator WaitForInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(triggerKey))
            {
                if (!movableItem.isCarrying)
                {
                    movableItem.PickUp(); // 第一次E：拿起
                    pickUpPrompt.SetActive(false); // 隐藏“拿起”提示
                    placePrompt.SetActive(true); // 显示“放下”提示
                }
                else
                {
                    movableItem.PlaceItem(); // 第二次E：放下
                    placePrompt.SetActive(false); // 隐藏“放下”提示
                }
                break; // 触发后停止等待
            }
            yield return null;
        }
    }
}
