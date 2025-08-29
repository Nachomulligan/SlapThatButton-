using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRadius = 10f;

    private Transform player;
    private ObjectPool parentPool;
    private bool isActive = false;
    public AudioSource DieSFX;

    [Header("Audio Settings")]
    [SerializeField] private float audioDelay = 0.3f; // Tiempo para que termine el audio

    private void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Enemy found player");
            }
            else
            {
                Debug.LogWarning("Enemy couldn't find player with tag 'Player'");
            }
        }
        isActive = true;
        Debug.Log($"Enemy activated at position {transform.position}");
    }

    private void Update()
    {
        if (!isActive || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si está demasiado lejos, devolver al pool
        if (distanceToPlayer > detectionRadius * 2f)
        {
            ReturnToPool();
            return;
        }

        // Mover hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disparar evento de muerte del jugador
            GameManager.OnPlayerDeath?.Invoke();
            return;
        }

        // Si es golpeado por un proyectil
        if (other.CompareTag("Projectile"))
        {
            // SOLUCIÓN 1: Delay antes de devolver al pool
            StartCoroutine(HandleDeath());

            GameManager.Instance?.RegisterEnemyKill();

            // Devolver proyectil a su pool
            ObjectPool projectilePool = other.GetComponentInParent<ObjectPool>();
            if (projectilePool != null)
            {
                projectilePool.ReturnObject(other.gameObject);
            }
            else
            {
                other.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator HandleDeath()
    {
        // Detener movimiento inmediatamente
        isActive = false;

        // Reproducir sonido
        if (DieSFX != null)
        {
            DieSFX.Play();
            Debug.Log("Playing death sound");

            // Esperar a que termine el sonido (o un tiempo fijo)
            yield return new WaitForSeconds(audioDelay);
        }

        // Ahora sí devolver al pool
        ReturnToPool();
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetPool(ObjectPool pool)
    {
        parentPool = pool;
    }

    private void ReturnToPool()
    {
        if (parentPool != null)
        {
            parentPool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        isActive = false;
    }
}