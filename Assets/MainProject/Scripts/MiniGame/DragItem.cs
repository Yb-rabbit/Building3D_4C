using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 originalPosition;    // 记录一开始出生的坐标

    [Tooltip("目标放置区域")]
    public RectTransform targetArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        // 记录物体的初始锚点位置
        originalPosition = rectTransform.anchoredPosition;
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
            // 【情况1：成功】
            // 什么都不用做！因为 OnDrag 已经把物体带到了终点，我们只要不改变它，它就停在终点。

            // 锁定该物体，不能再拖拽
            this.enabled = false;

            // 通知管理器
            MiniGameManager.Instance.OnItemPlaced();
        }
        else
        {
            // 【情况2：失败】
            // 直接把坐标重置为初始记录的坐标，物体就回去了
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
