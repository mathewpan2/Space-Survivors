using System;

public interface IHealth
{
    public void TakeDamage(int damage);

    public void Heal(int healAmount);

    public void Die();

}