using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bat : MonoBehaviour
{
    public enum State { Idle, Chasing, Returning, Die }

    [Header("Movement")]
    [SerializeField] float speed = 3f;
    [SerializeField] float waypointThreshold = 0.1f;
    [SerializeField] float repathRate = 0.5f;

    [Header("Detection")]
    [SerializeField] Transform target;
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float loseRadius = 7f;
    [SerializeField] float loseDelay = 1f;
    [SerializeField] LayerMask viewMask;
    [SerializeField] bool requireLineOfSight = true;
    [SerializeField] float detectionCheckRate = 0.2f;

    [Header("Visual")]
    [SerializeField] Animator animator;
    [SerializeField] Transform visualTransform;
    [SerializeField] bool autoFlip = true;

    [Header("Debug")]
    [SerializeField] bool showGizmos = true;
    [SerializeField] bool showLogs = false;
    [SerializeField] Color idleColor = Color.yellow;
    [SerializeField] Color chasingColor = Color.red;
    [SerializeField] Color returningColor = Color.blue;

    // Animation hashes
    static readonly int IdleHash = Animator.StringToHash("Idle");
    static readonly int FlyingHash = Animator.StringToHash("Flying");
    static readonly int CeilingInHash = Animator.StringToHash("CeilingIn");
    static readonly int CeilingOutHash = Animator.StringToHash("CeilingOut");
    static readonly int HitHash = Animator.StringToHash("Hit");

    // Runtime
    Rigidbody2D rb;
    Vector2 originalPosition;
    State currentState = State.Idle;
    bool facingRight = true;

    List<Vector2> path;
    int currentWaypoint;
    float loseTimer;

    Coroutine detectionCoroutine;
    Coroutine pathfindingCoroutine;
    Coroutine movementCoroutine;

    #region Initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (visualTransform == null) visualTransform = transform;

        SetupRigidbody();
    }

    void Start()
    {
        originalPosition = transform.position;
        ChangeState(State.Idle);
        StartDetection();
    }

    void SetupRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;
    }

    void OnEnable() => StartDetection();
    void OnDisable() => StopAllCoroutines();

    void StartDetection()
    {
        if (detectionCoroutine == null)
            detectionCoroutine = StartCoroutine(DetectionLoop());
    }

    #endregion

    #region State Management

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        State oldState = currentState;
        currentState = newState;

        if (showLogs) Debug.Log($"[State] {oldState} -> {newState}");

        OnStateEnter(newState);
    }

    void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.Idle:
                PlayAnimation(IdleHash);
                break;

            case State.Chasing:
                PlayAnimation(CeilingOutHash);
                StartCoroutine(DelayedAnimation(FlyingHash, 0.25f));
                StartChasing();
                break;

            case State.Returning:
                StopChasing();
                StartCoroutine(ReturnToOrigin());
                break;

            case State.Die:
                ExecuteDeath();
                break;
        }
    }

    IEnumerator DelayedAnimation(int animHash, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == State.Chasing)
            PlayAnimation(animHash);
    }

    #endregion

    #region Detection

    IEnumerator DetectionLoop()
    {
        while (currentState != State.Die)
        {
            bool targetVisible = IsTargetVisible();

            switch (currentState)
            {
                case State.Idle when targetVisible:
                    ChangeState(State.Chasing);
                    break;

                case State.Chasing:
                    UpdateChaseTracking(targetVisible);
                    break;
            }

            yield return new WaitForSeconds(detectionCheckRate);
        }
    }

    bool IsTargetVisible()
    {
        if (target == null) return false;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > detectionRadius) return false;

        return !requireLineOfSight || HasLineOfSight();
    }

    bool HasLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, viewMask);
        return hit.collider == null;
    }

    void UpdateChaseTracking(bool targetVisible)
    {
        if (target == null)
        {
            ChangeState(State.Returning);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= loseRadius)
        {
            loseTimer = 0f;
        }
        else
        {
            loseTimer += detectionCheckRate;
            if (loseTimer >= loseDelay)
                ChangeState(State.Returning);
        }
    }

    #endregion

    #region Chasing & Pathfinding

    void StartChasing()
    {
        loseTimer = 0f;
        pathfindingCoroutine = StartCoroutine(PathfindingLoop());
    }

    void StopChasing()
    {
        if (pathfindingCoroutine != null)
        {
            StopCoroutine(pathfindingCoroutine);
            pathfindingCoroutine = null;
        }

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    IEnumerator PathfindingLoop()
    {
        while (currentState == State.Chasing && target != null)
        {
        

            path = Pathfinding.Instance.FindPath(transform.position, target.position);

            if (path != null && path.Count > 0)
            {
                currentWaypoint = 0;
                if (movementCoroutine != null) StopCoroutine(movementCoroutine);
                movementCoroutine = StartCoroutine(FollowPath());
            }
            else if (showLogs)
            {
                Debug.LogWarning($"[Path] No path found from {transform.position} to {target.position}");
            }

            yield return new WaitForSeconds(repathRate);
        }
    }

   
    IEnumerator FollowPath()
    {
        while (currentState == State.Chasing && currentWaypoint < path.Count)
        {
            Vector2 waypoint = path[currentWaypoint];
            Vector2 currentPos = rb.position;
            Vector2 direction = waypoint - currentPos;

            if (direction.magnitude <= waypointThreshold)
            {
                currentWaypoint++;
                yield return null;
                continue;
            }

            MoveTowards(direction.normalized);
            yield return new WaitForFixedUpdate();
        }
    }

    void MoveTowards(Vector2 direction)
    {
        if (autoFlip) UpdateFlip(direction);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    #endregion

    #region Returning

    IEnumerator ReturnToOrigin()
    {
        while (currentState == State.Returning)
        {
            Vector2 direction = originalPosition - rb.position;

            if (direction.magnitude < 0.05f)
            {
                PlayAnimation(CeilingInHash);
                yield return new WaitForSeconds(0.25f);
                ChangeState(State.Idle);
                yield break;
            }

            MoveTowards(direction.normalized);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Visual & Flip

    void PlayAnimation(int animHash)
    {
        if (animator != null) animator.Play(animHash);
    }

    void UpdateFlip(Vector2 direction)
    {
        if (direction.x > 0 && !facingRight) Flip();
        else if (direction.x < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = visualTransform.localScale;
        scale.x *= -1;
        visualTransform.localScale = scale;
    }

    #endregion

    #region Combat

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == State.Die) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null) return;
           
            if (player.IsDead) return;

            float yDifference = collision.transform.position.y - transform.position.y;

            if (yDifference > 0.3f)
            {
                player.OnHit();
                ChangeState(State.Die);
            }
            else
            {
                player.OnPlayerHit();
            }
        }
    }

    void ExecuteDeath()
    {
        StopAllCoroutines();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 4f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        PlayAnimation(HitHash);

        Destroy(gameObject, 1f);
    }

    #endregion

    #region Gizmos

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 pos = Application.isPlaying ? transform.position : transform.position;

        // Detection radius
        Gizmos.color = GetStateColor();
        Gizmos.DrawWireSphere(pos, detectionRadius);

        // Lose radius
        if (currentState == State.Chasing)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(pos, loseRadius);
        }

        // Target line
        if (target != null)
        {
            bool hasLOS = !requireLineOfSight || HasLineOfSight();
            Gizmos.color = hasLOS ? Color.green : Color.red;
            Gizmos.DrawLine(pos, target.position);

            if (hasLOS && Vector2.Distance(pos, target.position) <= detectionRadius)
            {
                Gizmos.DrawWireSphere(target.position, 0.3f);
            }
        }

        // Original position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(originalPosition, 0.2f);
            Gizmos.DrawLine(pos, originalPosition);
        }

        // Path
        if (path != null && path.Count > 0 && currentState == State.Chasing)
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(path[i], path[i + 1]);

            if (currentWaypoint < path.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(path[currentWaypoint], 0.15f);
            }
        }
    }

    Color GetStateColor()
    {
        return currentState switch
        {
            State.Idle => idleColor,
            State.Chasing => chasingColor,
            State.Returning => returningColor,
            State.Die => Color.gray,
            _ => Color.white
        };
    }

    #endregion
}