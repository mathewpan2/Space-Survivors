using UnityEngine;

public enum UpgradeType
{
    MaxHealth,
    MoveSpeed,

    GunDamage,
    GunFireRate,
    BulletSpeed,

    SwordDamage,
    SwordAttackSpeed,
    SwordRange,

    ExtraShot
    // add more as needed
}

[CreateAssetMenu(menuName = "Game/Player Upgrade")]
public class PlayerUpgrade : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    public Sprite icon;

    public UpgradeType type;
    public float amount;    // e.g. +20 HP, +0.5 speed, etc.
}
