using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Slider slider;

    void Start()
    {
        if (!playerHealth)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }
        if (!slider) slider = GetComponentInChildren<Slider>();

        if (playerHealth && slider)
        {
            slider.minValue = 0;
            slider.maxValue = playerHealth.Max;
            slider.value = playerHealth.Current;
            playerHealth.onHealthChanged.AddListener(UpdateBar);
        }
    }

    void UpdateBar(int current, int max)
    {
        if (slider)
        {
            if (slider.maxValue != max) slider.maxValue = max;
            slider.value = current;
        }
    }
}
