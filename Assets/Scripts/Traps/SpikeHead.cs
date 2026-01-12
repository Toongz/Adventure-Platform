using System.Collections;
using UnityEngine;

public class SpikeHead : TrapBase
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float chaseDuration = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2[] directions;


    private Vector3 startPosition;
    private bool isChasing = false;

    private void Start()
    {
        startPosition = transform.position;
    }
    private void Update()
    {
        if (!isChasing)
        {
            foreach (Vector2 dir in directions)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionDistance, playerLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    StartCoroutine(Chase(dir));
                    break;
                }
            }
        }
    }

    private IEnumerator Chase(Vector2 direction)
    {
        isChasing = true;
        float elapsed = 0f;
        Vector3 target = transform.position + (Vector3)(direction.normalized * detectionDistance);
        while (elapsed < chaseDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
        isChasing = false;
    }

    private void OnDrawGizmos()
    {
        if (directions == null) return;

        Gizmos.color = Color.red;
        foreach (Vector2 dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir.normalized * detectionDistance);
        }
    }
}
