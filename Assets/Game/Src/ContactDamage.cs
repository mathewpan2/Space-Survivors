using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    public int damage = 1;
    public float tickInterval = 0.5f;   // damage every X seconds while touching
    public LayerMask playerLayer;

    float nextTickTime;

    void OnCollisionStay2D(Collision2D col)
    {
        if (Time.time < nextTickTime) return;
        if (((1 << col.gameObject.layer) & playerLayer) == 0) return;

        var hp = col.gameObject.GetComponent<Health>();
        if (hp) hp.TakeDamage(damage);
        nextTickTime = Time.time + tickInterval;
    }

    // If you use triggers instead of collisions, also add:
    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextTickTime) return;
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;

        var hp = other.GetComponent<Health>();
        if (hp) hp.TakeDamage(damage);
        nextTickTime = Time.time + tickInterval;
    }
}
