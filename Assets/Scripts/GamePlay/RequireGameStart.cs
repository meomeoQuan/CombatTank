using UnityEngine;
using System.Collections.Generic;

public class RequireGameStart : MonoBehaviour
{
    [Tooltip("Danh sách component sẽ bị tắt đến khi game start")]
    public List<Behaviour> targetComponents = new List<Behaviour>();

    void Awake()
    {
        foreach (var comp in targetComponents)
        {
            if (comp != null)
                comp.enabled = false;
        }
    }

    public void EnableComponent()
    {
        foreach (var comp in targetComponents)
        {
            if (comp != null)
                comp.enabled = true;
        }
    }
    public void DisableComponent()
    {
        foreach (var comp in targetComponents)
        {
            if (comp != null)
                comp.enabled = false;
        }
    }

}
