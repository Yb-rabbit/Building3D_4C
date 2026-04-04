using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int pieceIndex; //图块索引

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CanvasGroup canvasGroup;
    
    public Transform originalParent;  // 拖拽前的父物体
    public Vector3 originalPosition;  // 拖拽前的位置（如果没放进去，要弹回原位）
    private Vector2 offset;            // 鼠标点击位置与图片中心的偏移（防止拖拽时瞬移）

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果它已经在槽位里了，要把槽位清空
        if (transform.parent.GetComponent<PuzzleSlot>() != null)
        {
            transform.parent.GetComponent<PuzzleSlot>().ClearSlot();
        }

        // 记录原始位置和父级（为了放错了能弹回去）
        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        // 拖拽时提升到最上层，并放到Canvas下（防止被其他UI裁剪）
        transform.SetParent(parentCanvas.transform);
        transform.SetAsLastSibling();

        // 防止鼠标穿透
        canvasGroup.blocksRaycasts = false;

        // 计算鼠标和拼图块中心的偏移量，保证拖拽手感丝滑不乱跳
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        offset = localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 防乱飞
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos);

        rectTransform.localPosition = localPos - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复鼠标点击穿透
        canvasGroup.blocksRaycasts = true;

        // 检查：如果松手时，它的父物体没有PuzzleSlot组件，说明没放进槽位！
        if (transform.parent.GetComponent<PuzzleSlot>() == null)
        {
            // 没放进槽位，打回原形（回到刚才的位置）
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
        }
    }
}
