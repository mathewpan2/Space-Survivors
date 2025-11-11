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
        Debug.Log("taking damage stay "+ hp);
        if (hp) hp.TakeDamage(damage);
        nextTickTime = Time.time + tickInterval;
    }

    // If you use triggers instead of collisions, also add:
    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextTickTime) return;
        // Debug.Log($"LayerMask value={playerLayer.value}, playerLayerBit={(1 << other.gameObject.layer)}, overlap={(playerLayer.value & (1 << other.gameObject.layer))}");

        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;
        var hp = other.GetComponent<Health>();
        Debug.Log("taking damage trigger: " + hp);
        if (hp) hp.TakeDamage(damage);
        nextTickTime = Time.time + tickInterval;
    }
}
