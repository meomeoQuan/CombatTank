using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    void LateUpdate()
    {
        if (Target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(Target.position + Offset);
        transform.position = screenPos;
    }
}
