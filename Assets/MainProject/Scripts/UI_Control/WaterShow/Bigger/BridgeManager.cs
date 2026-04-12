using UnityEngine;
using Cinemachine;
using System.Collections.Generic; // 新增：用于使用 List<GameObject>

public class BridgeManager : MonoBehaviour 
{
    [Header("核心引用")]
    public WaterGateDoor gateDoor; // 把水窗门拖进来

    [Header("特效切换")]
    public ParticleSystem floodParticle; // 粒子A：倒灌的黄泥水
    public ParticleSystem blockParticle;  // 粒子B：防住的清澈水

    [Header("设置")]
    public int requiredWeights = 3; // 需要放几个石块
    private int currentWeights = 0;

    [Header("结束后的激活列表")]
    [Tooltip("防倒灌成功后需要激活的物体（比如后续的NPC、过场动画触发器等）")]
    public List<GameObject> objectsToActivateOnComplete; 

    void Start() 
    {
        PlayFloodEffect();
    }

    // 石块被点击后调用这个方法
    public void AddWeight() 
    {
        currentWeights++;
        if (currentWeights >= requiredWeights) 
        {
            // 触发防倒灌成功逻辑
            SuccessBlocking();
        }
    }

    private void SuccessBlocking() 
    {
        // 1. 关门
        if (gateDoor != null) gateDoor.CloseGate();
        // 2. 延迟切换水流特效和结束流程
        Invoke("PlayBlockEffect", 1.5f); // 延迟1.5秒，等门关得差不多了再切水
    }

    private void PlayFloodEffect() 
    {
        floodParticle.Play();
        blockParticle.Stop();
    }

    private void PlayBlockEffect() 
    {
        floodParticle.Stop();
        blockParticle.Play();

        if (objectsToActivateOnComplete != null && objectsToActivateOnComplete.Count > 0)
        {
            foreach (var obj in objectsToActivateOnComplete)
            {
                if (obj != null) 
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
