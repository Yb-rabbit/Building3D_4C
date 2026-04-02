using UnityEngine;

public class PuzzleManagerUI : MonoBehaviour
{
    public static PuzzleManagerUI instance;

    [Header("拼图配置")]
    public GameObject piecePrefab;     // 拖入拼图块预制体
    public Sprite[] pieceSprites;      // 把9张切好的小图全选拖到这里（数组大小必须是9）
    public int rows = 3;              // 行数
    public int cols = 3;              // 列数

    [Header("激活对象")]
    public GameObject objectToActivate; // 拼图完成后要激活的3D物体（比如门、宝箱）

    private int totalPieces;
    private int placedPieces = 0;

    void Awake()
    {
        instance = this;
        if (objectToActivate != null) objectToActivate.SetActive(false);
    }

    void Start()
    {
        if (pieceSprites.Length != rows * cols)
        {
            Debug.LogError("图片数量不对！需要9张图片，请检查 Piece Sprites 数组。");
            return;
        }
        totalPieces = rows * cols;
        GeneratePuzzle();
    }

    void GeneratePuzzle()
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        if (canvasRect.rect.width <= 0 || canvasRect.rect.height <= 0)
        {
            Debug.LogError("Canvas尺寸无效！请检查Canvas的Width和Height（建议设为600×600）。");
            return;
        }

        // 计算拼图块尺寸（固定为200×200，与Canvas的600×600匹配）
        float pieceWidth = 200;   // 每块拼图的宽度
        float pieceHeight = 200;  // 每块拼图的高度

        // 计算整体拼图的起始点（让拼图居中在Canvas(0,0)点）
        float startX = -((cols - 1) * pieceWidth) / 2f;    // 起始X坐标（居中）
        float startY = ((rows - 1) * pieceHeight) / 2f;    // 起始Y坐标（居中）

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                GameObject pieceObj = Instantiate(piecePrefab, transform);
                pieceObj.GetComponent<UnityEngine.UI.Image>().sprite = pieceSprites[index];

                // 【关键修改】固定拼图块尺寸（200×200）
                pieceObj.GetComponent<RectTransform>().sizeDelta = new Vector2(pieceWidth, pieceHeight);

                // 计算并记录正确位置
                Vector2 correctPos = new Vector2(
                    startX + col * pieceWidth, 
                    startY - row * pieceHeight
                );
                pieceObj.GetComponent<PuzzlePieceUI>().correctPosition = correctPos;

                // 【关键修改】扩大随机范围（减少中间密度）
                pieceObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    Random.Range(-canvasRect.rect.width * 0.6f, canvasRect.rect.width * 0.6f), 
                    Random.Range(-canvasRect.rect.height * 0.6f, canvasRect.rect.height * 0.6f)
                );
            }
        }
    }

    public void OnPiecePlaced()
    {
        placedPieces++;
        if (placedPieces == totalPieces) PuzzleCompleted();
    }

    void PuzzleCompleted()
    {
        Debug.Log("拼图完成！");
        if (objectToActivate != null) objectToActivate.SetActive(true);
    }
}
