using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    public int correctIndex; // 这个槽位应该放第几张图
    public bool isOccupied = false; // 里面有没有块

    public void OnDrop(PointerEventData eventData)
    {
        // 获取拖过来的新拼图块
        PuzzlePiece draggedPiece = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (draggedPiece == null) return;

        if (isOccupied)
        {
            // 1. 找到当前槽位里住的“旧块”
            PuzzlePiece oldPiece = transform.GetComponentInChildren<PuzzlePiece>();
            
            if (oldPiece != null)
            {
                // 2. 把旧块踢到新块拖拽前的位置
                oldPiece.transform.SetParent(draggedPiece.originalParent);
                oldPiece.transform.localPosition = draggedPiece.originalPosition;

                // 3. 检查新块
                PuzzleSlot previousSlot = draggedPiece.originalParent.GetComponent<PuzzleSlot>();
                if (previousSlot != null)
                {
                    // 旧块去了槽位B，把槽位B标记为已占用
                    previousSlot.isOccupied = true; 
                }
            }
        }

        // 4. 把新拼图块放进当前槽位（位置瞬间归零，绝对精准）
        draggedPiece.transform.SetParent(this.transform);
        draggedPiece.transform.localPosition = Vector3.zero;
        
        isOccupied = true;
        
        // 5. 通知管理器检查有没有拼对
        PuzzleManager.instance.CheckCompletion();
    }

    // 当拼图块被拖走时调用（这个由 PuzzlePiece 的 OnBeginDrag 触发）
    public void ClearSlot()
    {
        isOccupied = false;
    }
}
