using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public EnemySpawner enemySpawner;

    public float survivalTime = 120f;

    private float remainingTime;

    public TextMeshProUGUI timerText;

    public VictoryUI victoryUI;

        void OnTimerEnd()
    {
        Debug.Log("Survival Time Over!");
        timerText.text = "0:00";
        enemySpawner.StopSpawning();
        Time.timeScale = 0f;  
        if (victoryUI.victoryPanel) victoryUI.showUI();
    }


    // Start is called before the first frame update
    void Start()
    {
        enemySpawner.StartSpawning();
        remainingTime = survivalTime;
    }


    // Update is called once per frame
    void Update()
    {

           if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            // Format the timer as x:xx
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

            // Check if the timer has reached zero
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                OnTimerEnd();
            }
        }
        
    }
}
