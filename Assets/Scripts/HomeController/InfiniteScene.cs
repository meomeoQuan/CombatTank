using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteScene : MonoBehaviour
{
    public Transform mainCam;         // Camera chính
    public Tilemap sceneTilemap;      // Tilemap bạn đã vẽ cảnh
    public int buffer = 20;           // Khoảng cách an toàn trước khi spawn thêm
    // public int repeatCount = 3;       // Bao nhiêu lần nhân bản trước và sau
    private BoundsInt sceneBounds;
    private TileBase[] sceneTiles;
    private int sceneWidth;
    private int leftBound;
    private int rightBound;

    void Start()
    {
        // Lấy toàn bộ block từ Scene Tilemap
        sceneBounds = sceneTilemap.cellBounds;
        sceneTiles = sceneTilemap.GetTilesBlock(sceneBounds);

        sceneWidth = sceneBounds.size.x; // số ô ngang

        int camX = Mathf.RoundToInt(mainCam.position.x);
        leftBound = camX - buffer;
        rightBound = camX + buffer;

        // Nhân ban đầu
        FillScene(leftBound, rightBound);
    }

    void Update()
    {
        int camX = Mathf.RoundToInt(mainCam.position.x);

        if (camX + buffer > rightBound)
        {
            int newRight = rightBound + sceneWidth;
            FillScene(rightBound + 1, newRight);
            rightBound = newRight;
        }

        if (camX - buffer < leftBound)
        {
            int newLeft = leftBound - sceneWidth;
            FillScene(newLeft, leftBound - 1);
            leftBound = newLeft;
        }
    }

    void FillScene(int xStart, int xEnd)
    {
        // Copy block gốc ra các vị trí mới
        for (int offsetX = xStart; offsetX <= xEnd; offsetX += sceneWidth)
        {
            Vector3Int checkPos = new Vector3Int(offsetX, sceneBounds.yMin, 0);
            if (sceneTilemap.HasTile(checkPos)) continue;
            sceneTilemap.SetTilesBlock(new BoundsInt(checkPos, sceneBounds.size), sceneTiles);
        }
    }
}
