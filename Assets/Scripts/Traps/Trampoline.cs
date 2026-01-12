using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public Animator animator;
    public float bounceForce = 300f;






    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse); 
            }
            animator.SetTrigger("trigger") ;
        }
    }
}
