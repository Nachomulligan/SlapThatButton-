
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private string enemiesKilledPrefix = "Enemies Killed: ";

    private void Start()
    {
        GameManager.OnEnemiesKilledChanged.AddListener(UpdateEnemiesKilledUI);
        UpdateEnemiesKilledUI(0); // Inicializar UI
    }

    private void UpdateEnemiesKilledUI(int enemiesKilled)
    {
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = enemiesKilledPrefix + enemiesKilled.ToString();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnEnemiesKilledChanged.RemoveListener(UpdateEnemiesKilledUI);
    }
}
