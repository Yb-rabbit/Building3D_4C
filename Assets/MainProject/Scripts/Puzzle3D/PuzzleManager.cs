using UnityEngine;
using System.Collections; // 用于协程

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Header("预制体")]
    public GameObject slotPrefab;     // 拖入Slot预制体
    public GameObject piecePrefab;    // 拖入Piece预制体

    [Header("拼图配置")]
    public Sprite[] pieceSprites;     // 9张切好的小图
    public int rows = 3;
    public int cols = 3;

    [Header("UI容器")]
    public Transform slotArea;        // 装凹槽
    public Transform pieceArea;       // 装散落拼图块

    [Header("激活对象")]
    public GameObject objectToActivate;

    [Header("推币机设置")]
    public Vector2 entryPosition;     // 推币机入口位置
    public float delayBetweenPieces = 0.5f; // 每块拼图推出的间隔时间（秒）

    private int totalPieces;

    void Awake()
    {
        instance = this;
        if (objectToActivate != null) objectToActivate.SetActive(false);
    }

    void Start()
    {
        if (pieceSprites.Length != rows * cols) return;
        totalPieces = rows * cols;
        GeneratePuzzle();
    }

    void GeneratePuzzle()
    {
        // 1. 生成 3x3 的凹槽（固定不动）
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                GameObject slotObj = Instantiate(slotPrefab, slotArea);
                slotObj.name = $"Slot_{index}";

                PuzzleSlot slot = slotObj.GetComponent<PuzzleSlot>();
                slot.correctIndex = index;

                // 排列成3x3网格
                float posX = (col - (cols - 1) / 2f) * 205f;
                float posY = -((row - (rows - 1) / 2f) * 205f);
                slotObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            }
        }

        // 2. 生成拼图块并推出来（依次推出）
        // 简单洗牌算法
        Sprite[] shuffledSprites = (Sprite[])pieceSprites.Clone();
        for (int i = 0; i < shuffledSprites.Length; i++)
        {
            Sprite temp = shuffledSprites[i];
            int randomIndex = Random.Range(i, shuffledSprites.Length);
            shuffledSprites[i] = shuffledSprites[randomIndex];
            shuffledSprites[randomIndex] = temp;
        }

        // 生成打乱后的拼图块，从入口位置依次推出
        for (int i = 0; i < totalPieces; i++)
        {
            StartCoroutine(SpawnPieceWithDelay(i, shuffledSprites[i]));
        }
    }

    // 延迟生成拼图块的协程
    IEnumerator SpawnPieceWithDelay(int index, Sprite sprite)
    {
        // 等待一段时间（每块拼图间隔 delayBetweenPieces 秒）
        yield return new WaitForSeconds(index * delayBetweenPieces);

        // 实例化拼图块（初始位置设为入口位置）
        GameObject pieceObj = Instantiate(piecePrefab, pieceArea);
        pieceObj.GetComponent<UnityEngine.UI.Image>().sprite = sprite;

        PuzzlePiece piece = pieceObj.GetComponent<PuzzlePiece>();
        // 找到这张图原本在数组中的索引
        piece.pieceIndex = System.Array.IndexOf(pieceSprites, sprite);

        // 初始位置设为入口位置（例如：屏幕右侧）
        pieceObj.GetComponent<RectTransform>().anchoredPosition = entryPosition;

        // 随机散落位置（从入口位置推出后，随机散落在屏幕上）
        float randomX = Random.Range(300f, 500f); // 散落区域的X范围（可根据需求调整）
        float randomY = Random.Range(-200f, 200f); // 散落区域的Y范围（可根据需求调整）
        pieceObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);
    }

    // 检查是否全部拼对
    public void CheckCompletion()
    {
        PuzzleSlot[] allSlots = slotArea.GetComponentsInChildren<PuzzleSlot>();
        bool isComplete = true;

        foreach (PuzzleSlot slot in allSlots)
        {
            if (!slot.isOccupied) { isComplete = false; break; }

            // 检查槽位里的拼图块，是不是属于这个槽位的
            PuzzlePiece pieceInSlot = slot.transform.GetComponentInChildren<PuzzlePiece>();
            if (pieceInSlot == null || pieceInSlot.pieceIndex != slot.correctIndex)
            {
                isComplete = false;
                break;
            }
        }

        if (isComplete)
        {
            if (objectToActivate != null) objectToActivate.SetActive(true);
        }
    }
}
