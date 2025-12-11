using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Health playerHealth;
    public GameObject gameOverPanel;

    [Header("Pause Panel")]
    public GameObject pauseMenuPanel;  
    public GameObject LevelUpPanel; 
    public AudioSource audioSource;
    public AudioClip loseSound;
    void Start()
    {
        if (!playerHealth)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) {
                playerHealth = p.GetComponent<Health>();
            }
        }

        playerHealth.onDie.AddListener(OnPlayerDie);

        if (gameOverPanel) 
            gameOverPanel.SetActive(false);

        if (pauseMenuPanel) 
            pauseMenuPanel.SetActive(false);
    }

    void OnPlayerDie()
    {
        Debug.Log("player die");
        if (LevelUpPanel) 
            LevelUpPanel.SetActive(false);
        if (gameOverPanel) 
            gameOverPanel.SetActive(true);
        audioSource.PlayOneShot(loseSound);

        Time.timeScale = 0f;  // freeze game
    }

    // Hook this to the Restart button OnClick
    public void Restart()
    {
        if (pauseMenuPanel) 
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Back()
    {
        if (pauseMenuPanel) 
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene("mainmenu");
    }

    public void Quit() 
    { 
        Application.Quit(); 
    }
}
