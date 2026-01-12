using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private bool isActived;
    [SerializeField] private float fireRate = 0.5f;

    [SerializeField] private float _detectDistance = 10f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    private float nextTime;


    private void Update()
    {
        CheckPlayerByRaycast();

        if (isActived)
        {
            Shoot();
        }
    }
    private void CheckPlayerByRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            transform.right,         
            _detectDistance,
            _playerLayer | _obstacleLayer
        );

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                isActived = true;
                return;
            }
        }

        isActived = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActived = true;
        }
    }
    private void Shoot()
    {

        if (Time.time >= nextTime)
        {
            nextTime = Time.time + fireRate;
            GameObject gameObject = MyPoolManager.Instance.GetFromPool(bulletPrefab, transform);
            gameObject.transform.position = firePoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            firePoint.position,
            firePoint.position + transform.right * _detectDistance
        );
    }


}
