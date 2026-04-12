using UnityEngine;

public class WeightStone : MonoBehaviour
{
    public BridgeManager manager;
    
    [Tooltip("这个石块放上去后，滑轨上对应的那个石块（默认是隐藏的）")]
    public GameObject sliderWeightVisual; 

    private float clickCooldown = 0f;

    void Update()
    {
        if (clickCooldown > 0f) clickCooldown -= Time.deltaTime;
    }

    void OnMouseEnter()
    {
        // 鼠标悬停变黄
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        // 鼠标移出恢复
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    void OnMouseDown()
    {
        if (clickCooldown > 0f) return;
        clickCooldown = 0.2f;

        // 1. 隐藏地上的石块（假装被拿起来了）
        gameObject.SetActive(false);

        // 2. 显示滑轨上的石块
        if (sliderWeightVisual != null)
        {
            sliderWeightVisual.SetActive(true);
        }

        // 3. 通知总控：我放好了一个配重
        if (manager != null)
        {
            manager.AddWeight();
        }
    }
}
