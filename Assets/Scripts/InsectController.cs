using UnityEngine;


public class InsectController : MonoBehaviour
{
    private InsectType insectType;
    private float speed;
    private bool hasBeenHit = false;
    private Rigidbody2D rb;

    // Nuevo: control de patrón
    private bool usePauseMovement = false;
    private float walkDuration = 1f;
    private float pauseDuration = 0.5f;
    private float patternTimer = 0f;
    private bool isWalking = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(InsectType type, float moveSpeed, bool walkPausePattern = false, float walkTime = 1f, float pauseTime = 0.5f)
    {
        insectType = type;
        speed = moveSpeed;
        hasBeenHit = false;

        usePauseMovement = walkPausePattern;
        walkDuration = walkTime;
        pauseDuration = pauseTime;

        rb.linearVelocity = Vector2.right * speed;
    }

    private void Update()
    {
        if (usePauseMovement && !hasBeenHit)
        {
            patternTimer += Time.deltaTime;

            if (isWalking)
            {
                if (patternTimer >= walkDuration)
                {
                    isWalking = false;
                    patternTimer = 0f;
                    rb.linearVelocity = Vector2.zero; // parar
                }
            }
            else
            {
                if (patternTimer >= pauseDuration)
                {
                    isWalking = true;
                    patternTimer = 0f;
                    rb.linearVelocity = Vector2.right * speed; // volver a caminar
                }
            }
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