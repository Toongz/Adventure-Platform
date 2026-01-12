using System.Collections;
using UnityEngine;

public class BulletPiece : MonoBehaviour
{
    public void Init(float lifetime)
    {
        StopAllCoroutines();
        StartCoroutine(DisableAfterTime(lifetime)); 
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        //gameObject.SetActive(false);
        GetComponent<ReturnToMyPool>()?.ReturnToPool();
    }
}
