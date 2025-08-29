using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private int baseEnemiesPerRound = 5;
    [SerializeField] private float roundDelay = 3f; 
    [SerializeField] private float speedIncreasePerRound = 0.2f;
    [SerializeField] private float spawnRateIncreasePerRound = 0.1f;

    [Header("Scene Management")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private int enemiesKilled = 0;
    private int currentRound = 0;
    private int enemiesToKillThisRound = 0;
    private int enemiesKilledThisRound = 0;

    private EnemySpawner enemySpawner;


    public static UnityEvent<int> OnEnemiesKilledChanged = new UnityEvent<int>();
    public static UnityEvent<int> OnRoundStarted = new UnityEvent<int>();
    public static UnityEvent OnPlayerDeath = new UnityEvent();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        StartCoroutine(StartNextRound()); 
    }

    public void RegisterEnemyKill()
    {
        enemiesKilled++;
        enemiesKilledThisRound++;
        OnEnemiesKilledChanged?.Invoke(enemiesKilled);

        if (enemiesKilledThisRound >= enemiesToKillThisRound)
        {
            StartCoroutine(StartNextRound());
        }
    }

    private IEnumerator StartNextRound()
    {
        currentRound++;
        enemiesKilledThisRound = 0;
        enemiesToKillThisRound = baseEnemiesPerRound + (currentRound - 1) * 3;

        yield return new WaitForSeconds(roundDelay);

        OnRoundStarted?.Invoke(currentRound);

        if (enemySpawner != null)
        {
            enemySpawner.SetEnemiesToSpawn(enemiesToKillThisRound);
            enemySpawner.IncreaseEnemySpeed(speedIncreasePerRound);
            enemySpawner.DecreaseSpawnInterval(spawnRateIncreasePerRound);
        }
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(DelayedSceneChange());
    }

    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(gameOverSceneName);
    }

    public int GetEnemiesKilled() => enemiesKilled;
    public int GetCurrentRound() => currentRound;

    private void OnDestroy()
    {
        OnPlayerDeath.RemoveListener(HandlePlayerDeath);
    }
}