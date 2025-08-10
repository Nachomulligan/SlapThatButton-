using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float baseSpeed = 3f;
    public float speedIncrement = 0.5f;
    public int butterflyStartLevel = 3;
    public int maxLevel = 10;

    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI messageText;

    [Header("Game Objects")]
    public Transform spawnPoint;
    public Transform endPoint;
    public HandController handController;
    public InsectSpawner insectSpawner;

    private int currentLevel = 1;
    private bool lastLevelWon = false; 
    private GameState currentState = GameState.WaitingToStart;

    public enum GameState
    {
        WaitingToStart,     // WAITING SPACE TO START LEVEL
        Playing,            // PLAYING
        WaitingToContinue   // WAITING SPACE AFTER WIN/LOSE
    }

    public System.Action<int> OnLevelChanged;
    public System.Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpaceInput();
        }
    }

    private void HandleSpaceInput()
    {
        switch (currentState)
        {
            case GameState.WaitingToStart:
                StartLevel();
                break;

            case GameState.Playing:
                handController.PerformSlap();
                break;

            case GameState.WaitingToContinue:
                ProcessContinue();
                break;
        }
    }

    private void StartLevel()
    {
        currentState = GameState.Playing;
        messageText.gameObject.SetActive(false);

        insectSpawner.SpawnInsect(currentLevel, spawnPoint.position, GetCurrentSpeed());

        OnGameStateChanged?.Invoke(currentState);
    }

    private void ProcessContinue()
    {
        if (lastLevelWon)
        {
            currentLevel++;
            if (currentLevel > maxLevel)
            {
                currentLevel = 1;
                messageText.text = "GAME COMPLETED!\nSPACE TO START";
            }
        }
        // IF LOSE, DONT CHANGE LEVEL

        currentState = GameState.WaitingToStart;
        messageText.text = "SPACE TO START";
        messageText.gameObject.SetActive(true);

        UpdateUI();
        OnGameStateChanged?.Invoke(currentState);
    }

    public void OnMosquitoHit()
    {
        currentState = GameState.WaitingToContinue;
        lastLevelWon = true;

        messageText.text = $"LEVEL {currentLevel} COMPLETED!\nSPACE TO CONTINUE";
        messageText.gameObject.SetActive(true);
    }

    public void OnButterflyHit()
    {
        currentState = GameState.WaitingToContinue;
        lastLevelWon = false;

        messageText.text = "LOSE!\nSPACE TO RETRY";
        messageText.gameObject.SetActive(true);
    }

    public void OnMosquitoMissed()
    {
        currentState = GameState.WaitingToContinue;
        lastLevelWon = false;

        messageText.text = "LOSE!\nSPACE TO RETRY";
        messageText.gameObject.SetActive(true);
    }

    private float GetCurrentSpeed()
    {
        return baseSpeed + (currentLevel - 1) * speedIncrement;
    }

    private void UpdateUI()
    {
        levelText.text = $"LEVEL: {currentLevel}";

        if (currentState == GameState.WaitingToStart)
        {
            messageText.text = "SPACE TO START";
            messageText.gameObject.SetActive(true);
        }

        OnLevelChanged?.Invoke(currentLevel);
    }

    public GameState CurrentState => currentState;
}
