using UnityEngine;

public class TrapBase : MonoBehaviour
{   protected void OnHit(PlayerController playerController)
    {
        playerController.OnPlayerHit();
        Debug.Log("OnHit");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            OnHit(player);
            Collider2D col = GetComponent<Collider2D>();
            col.enabled = false;
        }
    }
}
