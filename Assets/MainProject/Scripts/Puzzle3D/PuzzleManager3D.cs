using UnityEngine;

public class PuzzleManager3D : MonoBehaviour
{
    public static PuzzleManager3D instance;

    [Header("拼图设置")]
    public GameObject piecePrefab;
    public string imageName = "你的图片名字"; // Resources里的图片名
    public int rows = 3;
    public int cols = 3;

    [Header("激活对象设置")]
    public GameObject objectToActivate; // 你想拼图完成后激活的物体（比如一扇门、一个宝箱、一道光）

    private int totalPieces;
    private int placedPieces = 0;

    void Awake()
    {
        instance = this;
        
        // 确保一开始目标物体是隐藏的
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    void Start()
    {
        Sprite originalSprite = Resources.Load<Sprite>(imageName);
        if (originalSprite == null) return;

        totalPieces = rows * cols;
        GeneratePuzzle(originalSprite);
    }

    void GeneratePuzzle(Sprite originalSprite)
    {
        float pieceWidth = originalSprite.rect.width / cols;
        float pieceHeight = originalSprite.rect.height / rows;

        // 在3D世界中，设定拼图每一块的物理大小（比如宽1米，高1米）
        float worldPieceWidth = 1f;
        float worldPieceHeight = 1f;

        // 计算整体拼图的起始中心点
        float startX = -((cols - 1) * worldPieceWidth) / 2f;
        float startZ = ((rows - 1) * worldPieceHeight) / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // 1. 切割图片生成Sprite
                Rect rect = new Rect(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight);
                Sprite pieceSprite = Sprite.Create(
                    originalSprite.texture, rect, 
                    new Vector2(0.5f, 0.5f), 
                    originalSprite.pixelsPerUnit
                );

                // 2. 实例化3D拼图块
                GameObject pieceObj = Instantiate(piecePrefab, transform);

                // 3. 动态创建材质，将切好的图片贴在3D方块顶面
                // 注意：因为Cube默认的UV贴图比较特殊，直接贴可能会被拉伸。
                // 这里简单处理，如果想要完美贴图，建议把预制体的Cube换成Plane，或者使用自定义的带UV的模型
                Material mat = new Material(Shader.Find("Standard"));
                mat.mainTexture = pieceSprite.texture;
                mat.mainTextureScale = new Vector2(1f / cols, 1f / rows); // 关键：修正UV缩放
                mat.mainTextureOffset = new Vector2((float)col / cols, (float)row / rows); // 关键：修正UV偏移
                pieceObj.GetComponent<Renderer>().material = mat;

                // 4. 记录正确位置 (X和Z平面，Y保持0)
                Vector3 correctPos = new Vector3(
                    startX + col * worldPieceWidth, 
                    0f, 
                    startZ - row * worldPieceHeight
                );
                pieceObj.GetComponent<PuzzlePiece3D>().correctPosition = correctPos;

                // 5. 随机打散拼图块 (放在周围随机位置)
                pieceObj.transform.position = new Vector3(
                    Random.Range(-5f, 5f),
                    0f,
                    Random.Range(-5f, 5f)
                );
            }
        }
    }

    // 当有一块拼图拼对时调用
    public void OnPiecePlaced()
    {
        placedPieces++;

        if (placedPieces == totalPieces)
        {
            PuzzleCompleted();
        }
    }

    void PuzzleCompleted()
    {
        Debug.Log("3D拼图完成！");

        // 【核心功能】激活指定的对象
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            
            // 可选：如果激活的对象有动画，可以播放动画
            // Animator anim = objectToActivate.GetComponent<Animator>();
            // if (anim != null) anim.SetTrigger("Open");
        }
    }
}
