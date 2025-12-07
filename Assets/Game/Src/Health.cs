using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 5;
    float current;

    public UnityEvent onDie;
    public UnityEvent<float,float> onHealthChanged = new UnityEvent<float,float>();
    private SpriteFlash flashEffect;

    private int expDrop = 5;

    private Experience playerExp;

    void Awake() { 
        current = maxHealth; onHealthChanged.Invoke(current, maxHealth);
        flashEffect = GetComponent<SpriteFlash>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerExp = player.GetComponent<Experience>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0 || current <= 0) return;
        current = Mathf.Max(0, current - amount);
        onHealthChanged.Invoke(current, maxHealth);
        flashEffect?.FlashRed();
        if (current == 0) Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;
        current = Mathf.Min(maxHealth, current + amount);
        onHealthChanged.Invoke(current, maxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        current = Mathf.Min(current + amount, maxHealth);
        onHealthChanged?.Invoke(current, maxHealth);
    }

    void Die()
    {
        onDie?.Invoke();

        if (playerExp != null)
        {
            playerExp.AddExp(expDrop);
        }
        Destroy(gameObject);
    }

    public float Current => current;
    public float Max => maxHealth;
}
