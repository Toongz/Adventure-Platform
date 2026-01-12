using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletPieceSO bulletPieceSO;
    [SerializeField] private float speed = 10f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("Rigidbody2D is null");
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb == null) return;
        Vector2 bulletDir = rb.linearVelocity;
        for (int i = 0; i < bulletPieceSO.pieCount; i++)
        {
            GameObject piece = MyPoolManager.Instance.GetFromPool(bulletPieceSO.piecePrefab, transform.parent);
            Rigidbody2D rgb = piece.GetComponent<Rigidbody2D>();
            BulletPiece bulletPiece = piece.GetComponent<BulletPiece>();
            if (rgb != null)
            {
                Vector2 randomDir = bulletDir + Random.insideUnitCircle * 0.5f;
                randomDir.Normalize();
                rgb.linearVelocity = Vector2.zero;
                rgb.angularVelocity = 0f;
                rgb.AddForce(randomDir * bulletPieceSO.scatterForce, ForceMode2D.Impulse);
                rgb.AddTorque(Random.Range(bulletPieceSO.torqueMin, bulletPieceSO.torqueMax));
            }
            if (bulletPiece != null)
            {
                bulletPiece.Init(bulletPieceSO.pieceLifeTime);
                bulletPiece.transform.position = transform.position;
            }
        }

        //MyPoolManager.Instance.GetFromPool(gameObject);
        //gameObject.SetActive(false);

        GetComponent<ReturnToMyPool>()?.ReturnToPool();
    }







}
