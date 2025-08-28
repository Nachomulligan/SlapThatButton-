using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 20;
    [SerializeField] private bool autoExpand = true;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> allObjects = new List<GameObject>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        allObjects.Add(obj);
        return obj;
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            if (autoExpand)
            {
                CreateNewObject();
            }
            else
            {
                Debug.LogWarning("Pool is empty and autoExpand is disabled!");
                return null;
            }
        }

        GameObject obj = pool.Dequeue();
        // NO activar aquí, se activa en el spawner después de posicionar
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ReturnAllObjects()
    {
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy)
            {
                ReturnObject(obj);
            }
        }
    }
}
