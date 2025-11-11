using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _collectablePrefabs;

    public void SpawnCollectable(Vector2 position) //cần vector2 để xác định vị trí sưu tầm sinh ra
    {
        int index = Random.Range(0, _collectablePrefabs.Count);
        var selectedCollectable = _collectablePrefabs[index];

        //phương thức instantiate để tạo ra vật phẩm sưu tầm được chọn tại điểm position
        Instantiate(selectedCollectable, position, Quaternion.identity); //sử dụng quaternion identity để không quay
    }
}
