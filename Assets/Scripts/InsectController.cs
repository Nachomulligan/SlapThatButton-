using UnityEngine;

public class InsectController : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private InsectType insectType;
    [SerializeField] private bool hasBeenHit = false;

    private Rigidbody2D rb;
    private IMovementStrategy movementStrategy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(InsectType type, IMovementStrategy strategy)
    {
        insectType = type;
        hasBeenHit = false;
        movementStrategy = strategy;

        // Resetear la estrategia de movimiento al inicializar
        movementStrategy?.Reset();
    }

    private void Update()
    {
        // Solo manejar movimiento si no ha sido golpeado y tiene estrategia
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
        // Solo procesar si está activo, no ha sido golpeado y es un mosquito
        if (gameObject.activeInHierarchy && !hasBeenHit && insectType == InsectType.Mosquito)
        {
            GameManager.Instance.OnMosquitoMissed();
            ReturnToPool();
        }
    }

    // Métodos públicos para acceso desde las estrategias si necesitan información del insecto
    public InsectType GetInsectType() => insectType;
    public bool HasBeenHit() => hasBeenHit;

    // Para debugging
    public string GetCurrentMovementStrategy()
    {
        return movementStrategy?.GetType().Name ?? "No Strategy";
    }
}