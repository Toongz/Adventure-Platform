using UnityEngine;

public class Rino : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform groundCheck;         
    [SerializeField] private Transform wallCheck;          
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;        
    [SerializeField] private Animator _animator;

    private int _Idle = Animator.StringToHash("Idle");
    private int _Dead = Animator.StringToHash("Dead");

    private Rigidbody2D rb;
    private int _moveDirection = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        bool noGroundAhead = !Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

        bool hitWall = Physics2D.Raycast(wallCheck.position, transform.right * _moveDirection, checkDistance, groundLayer);

        if (noGroundAhead || hitWall)
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(_moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        _moveDirection *= -1;
        transform.eulerAngles = new Vector3(0, _moveDirection == 1 ? 0 : 180, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * _moveDirection * checkDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (collision.transform.position.y - this.transform.position.y > 0.85f)
            {
                player.OnHit();
                this.OnDead();

            }
            else
            {
                player.OnPlayerHit();
            }
        }
    }

    private void OnDead()
    {
        Debug.Log("Au ui");
        _animator.Play(_Dead);

        Rigidbody2D rigid = this.GetComponent<Rigidbody2D>();
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        Collider2D[] cols = this.GetComponents<Collider2D>();
        for(int i = 0; i < cols.Length; i++) {
            cols[i].enabled = false;
        }
    }
}


    