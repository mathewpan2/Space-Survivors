using UnityEngine;
using UnityEngine.UI;

public class PlayerExpUI : MonoBehaviour
{
    public Experience playerExp;
    public Slider slider;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
        if (!playerExp)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerExp = p.GetComponent<Experience>();
        }
    }

    void OnEnable()
    {
        if (playerExp && slider)
        {
            slider.minValue = 0;
            slider.maxValue = playerExp.Max;
            slider.value = playerExp.Current;
            playerExp.onExpChanged.AddListener(UpdateBar);
        }
        else
        {
            Debug.LogWarning($"[playerExpUI] Missing refs: playerExp={playerExp}, slider={slider}");
        }
    }

    void OnDisable()
    {
        if (playerExp) playerExp.onExpChanged.RemoveListener(UpdateBar);
    }

    void UpdateBar(int current, int max)
    {
        if (!slider) return;
        if (slider.maxValue != max) slider.maxValue = max;
        slider.value = current;
    }

}
