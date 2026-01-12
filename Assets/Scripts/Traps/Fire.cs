using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider2D _boxCol;
    private bool _isTrigger;
    private bool _isActive;
    private float _delayTime = 0.5f;
    private float _countDownTime = 2f;





    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.tag == "Player")
        {
            if (!_isTrigger)
            {
                StartCoroutine(OnTrigger());
            }
        }
        

    }

    private IEnumerator OnTrigger()
    {
        _isTrigger = true;
        _animator.SetTrigger("hit");
        yield return new WaitForSeconds(_delayTime);
        _isActive = true;
        _animator.SetBool("isOn", _isActive);
        if(_boxCol != null) _boxCol.enabled = true;
        yield return new WaitForSeconds(_countDownTime);
        _isTrigger = false;
        _isActive = false;
        _animator.SetBool("isOn", _isActive);
        _boxCol.enabled = false;
    }

}
