using UnityEngine;

public class InsectController : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private InsectType insectType;
    [SerializeField] private bool hasBeenHit = false;

    private Rigidbody2D rb;
    private IMovementStrategy movementStrategy;
    public AudioSource Squash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(InsectType type, IMovementStrategy strategy)
    {
        insectType = type;
        hasBeenHit = false;
        movementStrategy = strategy;
        movementStrategy?.Reset();
    }

    private void Update()
    {
        if (!hasBeenHit && movementStrategy != null)
        {
            movementStrategy.Move(this, rb);
        }
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
        StopMovement();

        if (insectType == InsectType.Mosquito)
        {
            GameManager.Instance.OnMosquitoHit();
            Squash.Play();
        }
        else
        {
            GameManager.Instance.OnButterflyHit();
            Squash.Play();

            //Cancelar mosquito pendiente de esta mariposa
            GameManager.Instance.insectSpawner.CancelButterflyMosquitoSpawn(gameObject);
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

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void ReturnToPool()
    {
        StopMovement();
        GameManager.Instance.insectSpawner.ReturnToPool(gameObject, insectType);
    }

    private void OnBecameInvisible()
    {
        if (gameObject.activeInHierarchy && !hasBeenHit && insectType == InsectType.Mosquito)
        {
            GameManager.Instance.OnMosquitoMissed();
            ReturnToPool();
        }
    }

    public InsectType GetInsectType() => insectType;
    public bool HasBeenHit() => hasBeenHit;

    public string GetCurrentMovementStrategy()
    {
        return movementStrategy?.GetType().Name ?? "No Strategy";
    }
}