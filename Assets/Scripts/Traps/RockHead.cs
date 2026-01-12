using System.Collections;
using UnityEngine;

public class RockHead : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float delayTime = 1f;
    [SerializeField] private Vector2 moveDirection = Vector2.right; 
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    private bool isDelaying = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
       
    }

    private void FixedUpdate()
    {
        if (!isDelaying)
        {
            rb.linearVelocity = moveDirection * speed;
           
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.OnPlayerHit();

        }
      
        if (moveDirection.x > 0) animator.SetTrigger("RightHit");
        else if (moveDirection.x < 0) animator.SetTrigger("LeftHit");
        else if (moveDirection.y > 0) animator.SetTrigger("TopHit");
        else if (moveDirection.y < 0) animator.SetTrigger("BottomHit");

        
        moveDirection = -moveDirection;

        
        StartCoroutine(DelayAfterHit());
    }

    private IEnumerator DelayAfterHit()
    {
        isDelaying = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(delayTime);
        isDelaying = false;
    }


}
