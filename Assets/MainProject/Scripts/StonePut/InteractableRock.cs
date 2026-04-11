using UnityEngine;

public class InteractableRock : MonoBehaviour
{
    [Header("类型判定")]
    public bool isBadRock = true;

    [Header("总控引用")]
    public ClearingManager manager;

    [Header("颜色设置")]
    public Color highlightColor = Color.red;
    public Color warningColor = Color.yellow;
    
    private Color originalColor;
    private MeshRenderer meshRenderer;
    private float clickCooldown = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    void Update()
    {
        if (clickCooldown > 0f)
        {
            clickCooldown -= Time.deltaTime;
        }
    }

    void OnMouseEnter()
    {
        if (meshRenderer == null) return;

        if (isBadRock)
        {
            meshRenderer.material.color = highlightColor;
        }
        else
        {
            meshRenderer.material.color = warningColor;
        }
    }

    void OnMouseExit()
    {
        if (meshRenderer == null) return;
        meshRenderer.material.color = originalColor; 
    }

    void OnMouseDown()
    {
        if (clickCooldown > 0f) return;

        if (!isBadRock)
        {
            StartCoroutine(FlashWarning());
            clickCooldown = 0.2f; 
            return;
        }

        // 1. 先把自己隐藏
        gameObject.SetActive(false); 
        
        // 2. 通知管理器加分
        if (manager != null)
        {
            manager.AddScore();
        }
    }

    private System.Collections.IEnumerator FlashWarning()
    {
        meshRenderer.material.color = warningColor;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = warningColor;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = originalColor;
    }
}
