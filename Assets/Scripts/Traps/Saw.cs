using System.Collections.Generic;
using UnityEngine;

public class Saw : TrapBase
{
    [SerializeField] private float _speed;
    [SerializeField] private List<Vector2> _wayPoints = new List<Vector2>();
    private int _currentIndex = 0;

    private void Update()
    {
        if (_wayPoints.Count == 0) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            _wayPoints[_currentIndex],
            _speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _wayPoints[_currentIndex]) < 0.1f)
        {
            _currentIndex++;
            if (_currentIndex >= _wayPoints.Count)
            {
                _currentIndex = 0;
            }
         }
    }

    private void OnDrawGizmos()
    {
        if (_wayPoints == null || _wayPoints.Count == 0) return;

        Gizmos.color = Color.red;

        for(int i = 0; i < _wayPoints.Count; i++)
        {
            Vector3 point = new Vector3(_wayPoints[i].x, _wayPoints[i].y, 0);
            Gizmos.DrawSphere(point, 0.15f);

            if(i + 1 < _wayPoints.Count)
            {
                Vector3 nextPoint = new Vector3(_wayPoints[i + 1].x, _wayPoints[i + 1].y, 0);
                Gizmos.DrawLine(point, nextPoint);
            }
        }

        Vector3 first = new Vector3(_wayPoints[0].x, _wayPoints[0].y, 0);
        Vector3 last = new Vector3(_wayPoints[^1].x, _wayPoints[^1].y, 0);
        Gizmos.DrawLine(first, last);

    }
}
