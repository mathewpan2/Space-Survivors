using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 1f;          // 🔹 this projectile's damage
    public float lifeSeconds = 3f;
    public LayerMask hitLayers;     // set to Enemy layer(s) in Inspector

    void Start()
    {
        Destroy(gameObject, lifeSeconds);
    }

    // Optional: helper to initialize from stats
    public void InitFromStats(PlayerStats stats)
    {
        if (stats != null)
            damage = stats.gunDamage;   // or bulletDamage, whatever you named it
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other projectiles
        if (other.GetComponent<ProjectileDamage>() != null)
            return;
        
        // ignore if not in hitLayers
        if (((1 << other.gameObject.layer) & hitLayers) == 0)
            return;

        var hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
