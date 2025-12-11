using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip winSound;

    public GameObject victoryPanel;
    // Start is called before the first frame update
    void Start()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
    }

    public void showUI()
    {
        if (victoryPanel) victoryPanel.SetActive(true);
        audioSource.PlayOneShot(winSound);

    }

    public void hideUI()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Back()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("mainmenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
