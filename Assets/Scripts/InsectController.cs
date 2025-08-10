using UnityEngine;

public class InsectController : MonoBehaviour
{
    private InsectType insectType;
    private float speed;
    private bool hasBeenHit = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(InsectType type, float moveSpeed)
    {
        insectType = type;
        speed = moveSpeed;
        hasBeenHit = false;
        rb.linearVelocity = Vector2.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenHit) return;

        if (other.CompareTag("Hand"))
        {
            hasBeenHit = true;
            HandleHit();
        }
        else if (other.CompareTag("EndZone"))
        {
            HandleReachedEnd();
        }
    }

    public void HandleHit()
    {
        if (insectType == InsectType.Mosquito)
        {
            GameManager.Instance.OnMosquitoHit();
        }
        else
        {
            GameManager.Instance.OnButterflyHit();
        }

        ReturnToPool();
    }

    private void HandleReachedEnd()
    {
        if (insectType == InsectType.Mosquito)
        {
            GameManager.Instance.OnMosquitoMissed();
        }
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero;
        GameManager.Instance.insectSpawner.ReturnToPool(gameObject, insectType);
    }

    private void OnBecameInvisible()
    {
        if (!hasBeenHit && insectType == InsectType.Mosquito && gameObject.activeInHierarchy)
        {
            GameManager.Instance.OnMosquitoMissed();
            ReturnToPool();
        }
    }
}