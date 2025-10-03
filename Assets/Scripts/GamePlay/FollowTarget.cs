using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Tooltip("Đối tượng cần follow (ví dụ Object A)")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);
    void Update()
    {
        if (target != null)
        {
            // Luôn lấy vị trí target cộng với offset
            transform.position = target.position + offset;
        }
    }
}
