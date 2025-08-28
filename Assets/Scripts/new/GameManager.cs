using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private int enemiesPerSpeedIncrease = 10;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private int enemiesPerSpawnIncrease = 15;
    [SerializeField] private float spawnRateIncrease = 0.2f;

    [Header("Scene Management")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private int enemiesKilled = 0;
    private int currentRound = 1;
    private EnemySpawner enemySpawner;

    // Events
    public static UnityEvent<int> OnEnemiesKilledChanged = new UnityEvent<int>();
    public static UnityEvent OnPlayerDeath = new UnityEvent();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void Start()
    {
        OnPlayerDeath.AddListener(HandlePlayerDeath);
    }

    public void RegisterEnemyKill()
    {
        enemiesKilled++;
        OnEnemiesKilledChanged?.Invoke(enemiesKilled);

        CheckForSpeedIncrease();
        CheckForSpawnIncrease();
    }

    private void CheckForSpeedIncrease()
    {
        if (enemiesKilled % enemiesPerSpeedIncrease == 0)
        {
            if (enemySpawner != null)
            {
                enemySpawner.IncreaseEnemySpeed(speedIncreaseAmount);
            }
        }
    }

    private void CheckForSpawnIncrease()
    {
        if (enemiesKilled % enemiesPerSpawnIncrease == 0)
        {
            currentRound++;
            if (enemySpawner != null)
            {
                enemySpawner.DecreaseSpawnInterval(spawnRateIncrease);
            }
        }
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(DelayedSceneChange());
    }

    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(1f); // Pequeña pausa antes de cambiar escena
        SceneManager.LoadScene(gameOverSceneName);
    }

    public int GetEnemiesKilled() => enemiesKilled;
    public int GetCurrentRound() => currentRound;

    private void OnDestroy()
    {
        OnPlayerDeath.RemoveListener(HandlePlayerDeath);
    }
}