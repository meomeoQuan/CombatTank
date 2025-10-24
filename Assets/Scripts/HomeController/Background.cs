using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Transform mainCam;
    public Transform[] backgrounds; // gán MidBg, SideBg (ít nhất 2)
    private List<Transform> bgList;
    private float length;
    private int leftIndex;
    private int rightIndex;

    void Start()
    {
        if (backgrounds == null || backgrounds.Length < 2)
        {
            Debug.LogError("Assign at least 2 backgrounds in the inspector.");
            enabled = false;
            return;
        }

        // lấy chiều rộng (unity units) từ SpriteRenderer
        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Backgrounds must have a SpriteRenderer.");
            enabled = false;
            return;
        }
        length = sr.bounds.size.x;

        // tạo danh sách và sắp xếp theo vị trí x (từ trái sang phải)
        bgList = new List<Transform>(backgrounds);
        bgList.Sort((a, b) => a.position.x.CompareTo(b.position.x));
        leftIndex = 0;
        rightIndex = bgList.Count - 1;
    }

    // Quan trọng: chạy sau khi camera đã được cập nhật (Cinemachine cập nhật ở LateUpdate)
    void LateUpdate()
    {
        float camX = mainCam.position.x;

        // nếu camera đi quá sang phải (vượt nửa phần phải của rightmost), dịch chuyển leftmost sang phải
        while (camX > bgList[rightIndex].position.x - length / 2f)
        {
            // move leftmost to right of rightmost
            Vector3 newPos = bgList[rightIndex].position + Vector3.right * length;
            newPos.x = Mathf.Round(newPos.x * 1000f) / 1000f; // làm tròn để tránh float-gap nhỏ
            bgList[leftIndex].position = new Vector3(newPos.x, bgList[leftIndex].position.y, bgList[leftIndex].position.z);

            int oldLeft = leftIndex;
            leftIndex = (leftIndex + 1) % bgList.Count;
            rightIndex = oldLeft; // object moved becomes the new rightmost
        }

        // nếu camera đi quá sang trái (vượt nửa phần trái của leftmost), dịch chuyển rightmost sang trái
        while (camX < bgList[leftIndex].position.x + length / 2f)
        {
            // move rightmost to left of leftmost
            Vector3 newPos = bgList[leftIndex].position + Vector3.left * length;
            newPos.x = Mathf.Round(newPos.x * 1000f) / 1000f;
            bgList[rightIndex].position = new Vector3(newPos.x, bgList[rightIndex].position.y, bgList[rightIndex].position.z);

            int oldRight = rightIndex;
            rightIndex = (rightIndex - 1 + bgList.Count) % bgList.Count;
            leftIndex = oldRight; // object moved becomes the new leftmost
        }
    }
}
