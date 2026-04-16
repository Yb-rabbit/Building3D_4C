using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 originalPosition;
    
    [Tooltip("目标放置区域")]
    public RectTransform targetArea;
    
    [Tooltip("此物品所属的管理器")]
    public MiniGameManager myManager;

    void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        // 记录物体的初始锚点位置
        originalPosition = rectTransform.anchoredPosition;
    }

    void Start()
    {
        // 容错处理：如果忘记在面板拖拽赋值，自动向上寻找父级的管理器
        if (myManager == null)
        {
            myManager = GetComponentInParent<MiniGameManager>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        // 拖拽开始时，把物体提到最上层，防止被别的UI挡住
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) 
    {
        // 拖拽中：物体跟着鼠标/手指走
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        // 判断松手时，鼠标是否在目标区域内
        if (RectTransformUtility.RectangleContainsScreenPoint(targetArea, Input.mousePosition)) 
        {
            // 成功放置，禁用拖拽功能
            this.enabled = false;
            
            if (myManager != null)
            {
                myManager.OnItemPlaced();
            }
            else
            {
                Debug.LogError("未找到对应的 MiniGameManager，请检查绑定！");
            }
        } 
        else 
        {
            // 失败直接把坐标重置为初始记录的坐标
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}