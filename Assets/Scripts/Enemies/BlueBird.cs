using UnityEngine;

public class BlueBird : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Vector2[] _waypoints;
    [SerializeField] private float _speed;
    private int _moveDirection = 1;
    private int _currentIndex = 0;
    private bool _isDead = false;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        if (!_isDead)
        {
            transform.position = Vector2.MoveTowards(
            transform.position,
            _waypoints[_currentIndex],
            _speed * Time.deltaTime
        );

            if (Vector2.Distance(transform.position, _waypoints[_currentIndex]) < 0.1f)
            {
                _currentIndex++;

                if (_currentIndex >= _waypoints.Length)
                {
                    _currentIndex = 0;
                }

                Flip();
            }
        }
    }

    private void Flip()
    {
        _moveDirection *= -1;
        transform.eulerAngles = new Vector3(0, _moveDirection == 1 ? 0 : 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null || player.IsDead || _isDead) return;

        bool stomp = collision.transform.position.y - transform.position.y > 0.85f;

        if (stomp)
        {
            player.OnHit();
            OnHit();
        }
        else
        {
            player.OnPlayerHit();
        }
    }


    private void OnHit()
    {
        _isDead = true;
        Debug.Log(gameObject.name + "is dead");
        _animator.Play("Hit");

        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = 4f;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse); ;
        Collider2D[] cols = this.GetComponents<Collider2D>();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }

        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmos()
    {
        if (_waypoints == null || _waypoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            Vector3 point = new Vector3(_waypoints[i].x, _waypoints[i].y, 0f);

            Gizmos.DrawSphere(point, 0.15f);

            Vector3 nextPoint = new Vector3(
                _waypoints[(i + 1) % _waypoints.Length].x,
                _waypoints[(i + 1) % _waypoints.Length].y,
                0f
            );

            Gizmos.DrawLine(point, nextPoint);
        }
    }
}
