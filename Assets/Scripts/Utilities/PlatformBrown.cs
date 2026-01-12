using UnityEngine;

public class PlatformBrown : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public Animator animator;


    private int currentIndex = 0;
    private bool playerOn = false;

    private void Update()
    {
        if (waypoints.Length == 0) return;

        if (playerOn)
        {
            MoveForward();
        }
        else
        {
            MoveBackToStart();
        }
    }

    private void MoveForward()
    {
        Transform target = waypoints[currentIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
                currentIndex = waypoints.Length - 1; 
        }
    }

    private void MoveBackToStart()
    {
        Transform start = waypoints[0];

        transform.position = Vector2.MoveTowards(
            transform.position,
            start.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, start.position) < 0.05f)
        {
            currentIndex = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOn = true;
            animator.SetBool("isOn", true);

            
            collision.transform.SetParent(transform);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOn = false;
            animator.SetBool("isOn", false);

            
            collision.transform.SetParent(null);
        }
    }




}
