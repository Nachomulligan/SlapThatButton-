using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;

    [Header("Enemy Configuration")]
    [SerializeField] private float initialEnemySpeed = 2f;
    [SerializeField] private float maxEnemySpeed = 8f;

    private float currentSpawnInterval;
    private float currentEnemySpeed;
    private bool isSpawning = false;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentEnemySpeed = initialEnemySpeed;

        // Verificar configuración
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool is not assigned!");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Debug.Log($"Starting enemy spawner with {spawnPoints.Length} spawn points and interval {currentSpawnInterval}");
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnCoroutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnCoroutine()
    {
        while (isSpawning)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPool == null || spawnPoints.Length == 0) return;

        GameObject enemy = enemyPool.GetObject();
        if (enemy != null)
        {
            // Seleccionar punto de spawn aleatorio
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;

            // IMPORTANTE: Activar el enemigo DESPUÉS de posicionarlo
            enemy.SetActive(true);

            // Configurar velocidad del enemigo
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.SetSpeed(currentEnemySpeed);
                enemyComponent.SetPool(enemyPool);
            }

            Debug.Log($"Enemy spawned at {spawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("No enemy available in pool!");
        }
    }

    public void IncreaseEnemySpeed(float increase)
    {
        currentEnemySpeed = Mathf.Min(currentEnemySpeed + increase, maxEnemySpeed);
        Debug.Log($"Enemy speed increased to: {currentEnemySpeed}");
    }

    public void DecreaseSpawnInterval(float decrease)
    {
        currentSpawnInterval = Mathf.Max(currentSpawnInterval - decrease, minimumSpawnInterval);
        Debug.Log($"Spawn interval decreased to: {currentSpawnInterval}");
    }
}