using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    int current;

    public UnityEvent onDie;
    public UnityEvent<int,int> onHealthChanged; // (current, max)

    void Awake() { current = maxHealth; onHealthChanged?.Invoke(current, maxHealth); }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || current <= 0) return;
        current = Mathf.Max(0, current - amount);
        onHealthChanged?.Invoke(current, maxHealth);
        if (current == 0) Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        current = Mathf.Min(maxHealth, current + amount);
        onHealthChanged?.Invoke(current, maxHealth);
    }

    void Die()
    {
        onDie?.Invoke();
        Destroy(gameObject);
    }

    public int Current => current;
    public int Max => maxHealth;
}
