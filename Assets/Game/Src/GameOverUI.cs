using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Health playerHealth;
    public GameObject gameOverPanel;

    void Start()
    {
        if (!playerHealth)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }
        playerHealth.onDie.AddListener(OnPlayerDie);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void OnPlayerDie()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;  // pause
    }

    // Hook this to the Restart button OnClick
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Optional: Quit to menu…
    public void Quit() { Application.Quit(); }
}
