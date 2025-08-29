using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI currentRoundText;

    [Header("UI Text Configuration")]
    [SerializeField] private string enemiesKilledPrefix = "Enemies Killed: ";
    [SerializeField] private string roundPrefix = "Round: ";

    private void Start()
    {
        // Suscribirse a los eventos del GameManager
        GameManager.OnEnemiesKilledChanged.AddListener(UpdateEnemiesKilledUI);
        GameManager.OnRoundStarted.AddListener(UpdateRoundUI);

        // Inicializar UI
        UpdateEnemiesKilledUI(0);
        UpdateRoundUI(1); // La primera ronda es 1
    }

    private void UpdateEnemiesKilledUI(int enemiesKilled)
    {
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = enemiesKilledPrefix + enemiesKilled.ToString();
        }
    }

    private void UpdateRoundUI(int currentRound)
    {
        if (currentRoundText != null)
        {
            currentRoundText.text = roundPrefix + currentRound.ToString();
        }
    }

    private void OnDestroy()
    {
        // Desuscribirse de los eventos para evitar errores
        GameManager.OnEnemiesKilledChanged.RemoveListener(UpdateEnemiesKilledUI);
        GameManager.OnRoundStarted.RemoveListener(UpdateRoundUI);
    }
}