using UnityEngine;

public class PlatformGrey : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            waypoints[currentIndex].position,
            speed * Time.deltaTime
                                                 );

        if (Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }


}
