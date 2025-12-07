using UnityEngine;
using UnityEngine.Events;

public class Experience : MonoBehaviour
{
    [SerializeField] int level = 1;
    [SerializeField] int currentExp = 0;
    [SerializeField] int ExpToNextLevel = 10;

    // (currentExp, ExpToNextLevel)
    public UnityEvent<int, int> onExpChanged;

    // new: notify when we level up
    public UnityEvent<int> onLevelUp;   // sends new level

    public int Current => currentExp;
    public int Max => ExpToNextLevel;
    public int Level => level;

    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        currentExp += amount;

        while (currentExp >= ExpToNextLevel)
        {
            currentExp -= ExpToNextLevel;
            level++;

            Debug.Log("level up");
            // simple scaling curve
            ExpToNextLevel = Mathf.RoundToInt(ExpToNextLevel * 1.3f);

            // fire level up event
            onLevelUp?.Invoke(level);
        }

        onExpChanged?.Invoke(currentExp, ExpToNextLevel);
    }
}
