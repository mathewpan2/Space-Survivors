using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpController : MonoBehaviour
{
    // Start is called before the first frame update

    int expToNextLevel = 100;

    int currentLevel = 1;

    int currentExp = 0;


    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);

        Debug.Log("Leveled up to " + currentLevel);
    }
}
