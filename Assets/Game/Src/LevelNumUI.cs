using UnityEngine;
using TMPro;

public class LevelNumUI : MonoBehaviour
{
    public Experience playerExp;
    public TMP_Text levelText;

    void Awake()
    {
        if (!playerExp)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerExp = p.GetComponent<Experience>();
        }

        if (!levelText)
            levelText = GetComponentInChildren<TMP_Text>();
    }

    void OnEnable()
    {
        if (playerExp)
        {
            playerExp.onLevelUp.AddListener(UpdateLevelText);
            UpdateLevelText(playerExp.Level); // initialize
        }
    }

    void OnDisable()
    {
        if (playerExp)
            playerExp.onLevelUp.RemoveListener(UpdateLevelText);
    }

    void UpdateLevelText(int newLevel)
    {
        if (levelText)
            levelText.text = "" + newLevel;
    }
}
