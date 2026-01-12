using System.Collections.Generic;
using UnityEngine;

public class MyPool
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private GameObject baseObject;
    private GameObject tmpObject;
    private ReturnToMyPool returnPool;

    public MyPool(GameObject baseObject)
    {
        this.baseObject = baseObject;
    }

    public GameObject Get(Transform parent = null)
    {
        if (stack.Count > 0)
        {
            tmpObject = stack.Pop();
            tmpObject.SetActive(true);
  
            if (parent != null)
                tmpObject.transform.SetParent(parent);
            else
                tmpObject.transform.SetParent(null);
            return tmpObject;
        }

        tmpObject = GameObject.Instantiate(baseObject);
        returnPool = tmpObject.AddComponent<ReturnToMyPool>();
        returnPool.pool = this;
       
        if (parent != null)
            tmpObject.transform.SetParent(parent);
        else
            tmpObject.transform.SetParent(null);
        return tmpObject;
    }

    public void AddToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(MyPoolManager.Instance.transform);

        stack.Push(obj);
    }





}

