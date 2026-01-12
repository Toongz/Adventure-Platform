using UnityEngine;

public class ReturnToMyPool : MonoBehaviour
{
    public MyPool pool;

    //public void OnDisable()
    //{
    //    pool.AddToPool(gameObject);
    //}
    public void ReturnToPool()
    {
        pool?.AddToPool(gameObject);
    }
}
