using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lifeTime = 5f; 

    private float timer;
    private ObjectPool pool;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        timer = lifeTime;
    }

    public void SetPool(ObjectPool pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log($"Damage {damage} to enemy: {hit.name}");
            }
            Rigidbody2D hitRb = hit.attachedRigidbody;
            if (hitRb != null)
            {
                Vector2 dir = (hitRb.position - (Vector2)transform.position).normalized;
                hitRb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (pool != null)
            pool.ReturnObject(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}