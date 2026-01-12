using System.Collections.Generic;
using UnityEngine;

public class MyPoolManager : MonoBehaviour
{
    public static MyPoolManager Instance { get; private set; }

    private Dictionary<GameObject, MyPool> dicPools = new Dictionary<GameObject, MyPool>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetFromPool(GameObject obj, Transform parent = null)
    {
        if (dicPools.ContainsKey(obj) == false)
        {
            dicPools.Add(obj, new MyPool(obj));
        }
        return dicPools[obj].Get(parent);
    }
}
