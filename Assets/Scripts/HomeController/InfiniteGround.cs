using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteGround : MonoBehaviour
{
    public Transform mainCam;
    public Tilemap groundTilemap;
    public TileBase groundTile;
    public int chunkSize = 10;
    public int buffer = 20;

    private int leftBound;
    private int rightBound;
    private int groundY; // tự tính

    void Start()
    {
        // Lấy hàng thấp nhất nơi bạn đã vẽ gạch
        groundY = groundTilemap.cellBounds.yMin;

        int camX = Mathf.RoundToInt(mainCam.position.x);
        leftBound = camX - buffer;
        rightBound = camX + buffer;

        FillGround(leftBound, rightBound);
    }

    void Update()
    {
        int camX = Mathf.RoundToInt(mainCam.position.x);

        if (camX + buffer > rightBound)
        {
            int newRight = rightBound + chunkSize;
            FillGround(rightBound + 1, newRight);
            rightBound = newRight;
        }

        if (camX - buffer < leftBound)
        {
            int newLeft = leftBound - chunkSize;
            FillGround(newLeft, leftBound - 1);
            leftBound = newLeft;
        }
    }

    void FillGround(int xStart, int xEnd)
    {
        for (int x = xStart; x <= xEnd; x++)
        {
            groundTilemap.SetTile(new Vector3Int(x, groundY, 0), groundTile);
        }

        // Update tile hiển thị và collider
        groundTilemap.RefreshAllTiles();
    }
}
