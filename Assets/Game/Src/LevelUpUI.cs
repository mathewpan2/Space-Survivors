using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public Experience playerExp;
    public PlayerStats playerStats;

    public GameObject panel;
    public LevelUpOptionUI[] optionSlots;         // size 3
    public List<PlayerUpgrade> allUpgrades;       // auto-loaded from Resources

    void Awake()
    {
        if (!panel) panel = gameObject;
        panel.SetActive(false);

        // Auto-load all upgrades from Resources/Upgrades folder
        LoadAllUpgrades();

        if (!playerExp)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerExp = p.GetComponent<Experience>();
        }
        if (!playerStats)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerStats = p.GetComponent<PlayerStats>();
        }
    }

    void LoadAllUpgrades()
    {
        var loaded = Resources.LoadAll<PlayerUpgrade>("Upgrades");
        allUpgrades = new List<PlayerUpgrade>(loaded);
        Debug.Log($"[LevelUpUI] Auto-loaded {allUpgrades.Count} upgrades from Resources/Upgrades");
    }

    void OnEnable()
    {
        if (playerExp)
            playerExp.onLevelUp.AddListener(OnLevelUp);
    }

    void OnDisable()
    {
        if (playerExp)
            playerExp.onLevelUp.RemoveListener(OnLevelUp);
    }

    void OnLevelUp(int newLevel)
    {
        ShowChoices();
    }

    void ShowChoices()
    {
        Debug.Log("showing choices");
        if (allUpgrades == null || allUpgrades.Count == 0)
        {
            Debug.LogWarning("[LevelUpUI] No upgrades assigned.");
            return;
        }

        // pause game
        Time.timeScale = 0f;

        panel.SetActive(true);

        // pick 3 distinct random upgrades (or fewer if list small)
        var chosen = PickRandomUpgrades(3);

        for (int i = 0; i < optionSlots.Length; i++)
        {
            if (i < chosen.Count)
            {
                optionSlots[i].gameObject.SetActive(true);
                optionSlots[i].Setup(chosen[i], OnUpgradeChosen);
            }
            else
            {
                optionSlots[i].gameObject.SetActive(false);
            }
        }
    }

    List<PlayerUpgrade> PickRandomUpgrades(int count)
    {
        var list = new List<PlayerUpgrade>(allUpgrades);
        var result = new List<PlayerUpgrade>();

        for (int i = 0; i < count && list.Count > 0; i++)
        {
            int idx = Random.Range(0, list.Count);
            result.Add(list[idx]);
            list.RemoveAt(idx);
        }

        return result;
    }

    void OnUpgradeChosen(PlayerUpgrade upgrade)
    {
        if (playerStats && upgrade)
            playerStats.ApplyUpgrade(upgrade);

        // resume game + hide
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
