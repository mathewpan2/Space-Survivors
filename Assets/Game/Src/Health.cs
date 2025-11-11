using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    int current;

    public UnityEvent onDie;
    public UnityEvent<int,int> onHealthChanged = new UnityEvent<int,int>();
    private SpriteFlash flashEffect;

    void Awake() { 
        current = maxHealth; onHealthChanged.Invoke(current, maxHealth); 
        flashEffect = GetComponent<SpriteFlash>();
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("taking damage: " + current);
        if (amount <= 0 || current <= 0) return;
        current = Mathf.Max(0, current - amount);
        onHealthChanged.Invoke(current, maxHealth);
        flashEffect?.FlashRed();
        if (current == 0) Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        current = Mathf.Min(maxHealth, current + amount);
        onHealthChanged.Invoke(current, maxHealth);
    }

    void Die()
    {
        onDie?.Invoke();
        Destroy(gameObject);
    }

    public int Current => current;
    public int Max => maxHealth;
}
