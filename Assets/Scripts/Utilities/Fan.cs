using System.Collections;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float _force;

    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _swingParticle;
    [SerializeField] private float _onTime;
    [SerializeField] private float _offTime;
    private bool _isOn;
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;
    


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _boxCollider = GetComponent<BoxCollider2D>();

        StartCoroutine(FanRoutine());
    }

    

    private IEnumerator FanRoutine()
    {
        while(true)
        {
            _isOn = true;
            _animator.SetBool("isOn", _isOn);
            _swingParticle.Play();
            //_boxCollider.gameObject.SetActive(true);
            _boxCollider.enabled = true ;
            yield return new WaitForSeconds(_onTime);


            _isOn = false;
            _animator.SetBool("isOn", _isOn);
            _swingParticle.Stop();
            _boxCollider.enabled = false;
            yield return new WaitForSeconds(_offTime);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.AddForce(transform.up * _force, ForceMode2D.Force);
        }
    }


   

}
