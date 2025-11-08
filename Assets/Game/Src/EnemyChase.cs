using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;                // drag Player or leave empty (auto-find by tag "Player")
    public float stopDistance = 0.25f;      // how close we allow

    [Header("Movement")]
    public float speed = 2.5f;              // units/sec at top speed
    public float acceleration = 12f;        // how fast we reach speed
    public float turnLerp = 12f;            // smoothing for direction changes

    [Header("Visuals (optional)")]
    public SpriteRenderer spriteToFlip;     // assign if you want automatic left/right flip
    public Animator animator;               // optional; sets "Speed" float

    Rigidbody2D rb;
    Vector2 vel;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;                   // top-down
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (!player) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;

        // desired velocity
        Vector2 desired = Vector2.zero;
        if (dist > stopDistance)
            desired = toPlayer.normalized * speed;

        // smooth accelerate/turn
        vel = Vector2.MoveTowards(rb.velocity, desired, acceleration * Time.fixedDeltaTime);
        rb.velocity = Vector2.Lerp(rb.velocity, vel, turnLerp * Time.fixedDeltaTime);

        // optional visuals
        if (spriteToFlip) spriteToFlip.flipX = rb.velocity.x < -0.01f;
        if (animator) animator.SetFloat("Speed", rb.velocity.magnitude);
    }
}
