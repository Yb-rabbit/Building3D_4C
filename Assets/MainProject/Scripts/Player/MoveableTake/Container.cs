using UnityEngine;
using System.Collections.Generic;

public class Container : MonoBehaviour {
    [Header("放置位置（物品最终位置）")]
    public Vector3 placementPosition; // 容器内物品的最终位置（如篮子中心）
    [Header("容量")]
    public int maxCapacity = 1; // 最大容纳数量（如篮子只能放1个石头）
    private List<MovableItem> items = new List<MovableItem>(); // 已放置的物品

    /// <summary>
    /// 检查是否可放置物品
    /// </summary>
    public bool CanPlace() {
        return items.Count < maxCapacity;
    }

    /// <summary>
    /// 添加物品到容器
    /// </summary>
    public void AddItem(MovableItem item) {
        items.Add(item);
    }
}
