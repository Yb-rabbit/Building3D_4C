using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePieceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CanvasGroup canvasGroup;

    [HideInInspector] public Vector2 correctPosition; // 正确位置
    [HideInInspector] public bool isPlaced = false;    // 是否已经拼好

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        
        // 【关键修改】使用正确的缩放因子（解决乱飞问题）
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        CheckPlacement();
    }

    void CheckPlacement()
    {
        float distance = Vector2.Distance(rectTransform.anchoredPosition, correctPosition);
        
        // 【关键修改】增大吸附阈值（从0.5f改为5f）
        if (distance < 5f)
        {
            rectTransform.anchoredPosition = correctPosition;
            isPlaced = true;
            PuzzleManagerUI.instance.OnPiecePlaced();
        }
    }
}
