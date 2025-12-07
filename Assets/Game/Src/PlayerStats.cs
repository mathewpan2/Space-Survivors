using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Health health;        // hook your existing Health component
    public float moveSpeed = 5f;
    
    public float gunDamage = 1;
    public float gunFireRate = 0.15f;
    public float bulletSpeed = 15f;

    public float swordDamage = 1;
    public float swordAttackSpeed = 1f; 
    public float swordRange = 1f;

    public int extraShots = 0;          // number of additional bullets fired
    public float extraShotAngle = 15f;  // angle spread between extra shots (degrees) 

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
    }

    public void ApplyUpgrade(PlayerUpgrade upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.MaxHealth:
                // you may need to add a method like this to your Health script
                int add = Mathf.RoundToInt(upgrade.amount);
                health.IncreaseMaxHealth(add);
                break;

            case UpgradeType.MoveSpeed:
                moveSpeed += upgrade.amount;
                break;

            case UpgradeType.GunDamage:
                gunDamage += upgrade.amount;
                break;

            case UpgradeType.GunFireRate:
                // apply to your attack logic
                gunFireRate -= upgrade.amount;
                break;

            case UpgradeType.BulletSpeed:
                // apply to your attack logic
                bulletSpeed += upgrade.amount;
                break;

            case UpgradeType.SwordDamage:
                swordDamage += upgrade.amount;
                break;

            case UpgradeType.SwordAttackSpeed:
                // apply to your attack logic
                swordAttackSpeed -= upgrade.amount;
                break;

            case UpgradeType.SwordRange:
                // apply to your attack logic
                swordRange += upgrade.amount;
                break;

            case UpgradeType.ExtraShot:
                extraShots += Mathf.RoundToInt(upgrade.amount);
                break;
        }

        Debug.Log($"Applied upgrade: {upgrade.title}");
    }
}
