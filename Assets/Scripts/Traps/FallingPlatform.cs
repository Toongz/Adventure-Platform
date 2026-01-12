using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public float fallDelay = 1f;      
    public float destroyDelay = 2f;   

    private Rigidbody2D rb;
    private bool playerOn = false;
    private float timer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;  
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if (playerOn)
        {
            timer += Time.deltaTime;

            if (timer >= fallDelay)
            {
                Fall();
            }
        }
    }

    private void Fall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        _animator.Play("Idle");
        Destroy(gameObject, destroyDelay);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOn = true;
            timer = 0f;
        }
    }
}
