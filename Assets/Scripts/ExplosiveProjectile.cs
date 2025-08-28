using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifeTime = 5f;

    private float timer;
    private ObjectPoolMaxi pool;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        timer = lifeTime;
    }

    public void SetPool(ObjectPoolMaxi pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            DisableProjectile();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Mata al enemigo instantáneamente
        }

        DisableProjectile(); // La bala se destruye/recicla al chocar
    }

    private void DisableProjectile()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (pool != null)
            pool.ReturnObject(gameObject);
        else
            gameObject.SetActive(false);
    }
}
