using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 1;
    public float lifeSeconds = 3f;
    public LayerMask hitLayers;  // set to Enemy layer(s) in Inspector

    void Start() => Destroy(gameObject, lifeSeconds);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;
        var hp = other.GetComponent<Health>();
        if (hp) hp.TakeDamage(damage);
        Debug.Log("hit");
        Destroy(gameObject);
    }
}
