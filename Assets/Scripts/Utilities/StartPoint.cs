using UnityEngine;

public class StartPoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    readonly private int _Idle = Animator.StringToHash("Idle");
    readonly private int _Start = Animator.StringToHash("Start");
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = transform.position;

            if (_animator != null)
            {
                _animator.Play(_Start);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            _animator.Play(_Start);
        }
    }

}

