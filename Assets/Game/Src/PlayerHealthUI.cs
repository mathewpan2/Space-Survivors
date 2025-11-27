using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Slider slider;

    void Awake()
    {
        if (!slider) slider = GetComponentInChildren<Slider>(true);
        if (!playerHealth)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }
    }

    void OnEnable()
    {
        if (playerHealth && slider)
        {
            slider.minValue = 0;
            slider.maxValue = playerHealth.Max;
            slider.value = playerHealth.Current;
            playerHealth.onHealthChanged.AddListener(UpdateBar);
        }
        else
        {
            Debug.LogWarning($"[PlayerHealthUI] Missing refs: playerHealth={playerHealth}, slider={slider}");
        }
    }

    void OnDisable()
    {
        if (playerHealth) playerHealth.onHealthChanged.RemoveListener(UpdateBar);
    }

    void UpdateBar(float current, float max)
    {
        if (!slider) return;
        if (slider.maxValue != max) slider.maxValue = max;
        slider.value = current;
    }
}
