using UnityEngine;

public class ArrowOscillator : MonoBehaviour
{
    [Header("Cài đặt góc (theo Z)")]
    public float minAngle = -130f;   // Góc nhỏ nhất
    public float maxAngle = -50f;    // Góc lớn nhất
    public float speed = 30f;        // Tốc độ quay

    private float currentAngle;
    private int direction = 1;       // 1 = tăng (lên), -1 = giảm (xuống)

    void Start()
    {
        // Lấy góc ban đầu (Z)
        currentAngle = transform.localEulerAngles.z;

        // Chuyển từ 0-360 -> [-180,180] để dễ tính
        if (currentAngle > 180) currentAngle -= 360;

        // Mặc định khi mới vào thì đi lên trên trước
        direction = 1;
    }

    void Update()
    {
        // Cập nhật góc
        currentAngle += direction * speed * Time.deltaTime;

        // Nếu vượt max/min thì đổi chiều
        if (currentAngle >= maxAngle)
        {
            currentAngle = maxAngle;
            direction = -1;
        }
        else if (currentAngle <= minAngle)
        {
            currentAngle = minAngle;
            direction = 1;
        }

        // Áp dụng rotation
        transform.localEulerAngles = new Vector3(0, 0, currentAngle);
    }
}
