using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public float stopDistance = 0.25f;

    [Header("Movement")]
    public float speed = 2.5f;
    public float acceleration = 12f;
    public float turnLerp = 12f;

    [Header("Visuals")]
    [Tooltip("If true, also include inactive children when searching for SpriteRenderers (only used if not using Y-rotation flip).")]
    public bool includeInactiveSprites = true;
    [Tooltip("If your art faces RIGHT by default, set true. If it faces LEFT, set false.")]
    public bool facesRightByDefault = false;
    [Tooltip("Flip by rotating the whole object 180° around Y (best for nested/child prefabs).")]
    public bool useYRotationFlip = true;

    public Animator animator;

    Rigidbody2D rb;
    Vector2 vel;

    // cache all sprite renderers (only used if useYRotationFlip == false)
    readonly List<SpriteRenderer> sprites = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (!useYRotationFlip)
        {
            RefreshSpriteRenderers();
            if (sprites.Count == 0)
                Debug.LogWarning($"{name}: No SpriteRenderers found. If this is intentional, keep useYRotationFlip=true.");
        }

        if (!animator)
            animator = GetComponentInChildren<Animator>(true);
    }

    public void RefreshSpriteRenderers()
    {
        sprites.Clear();
        var found = GetComponentsInChildren<SpriteRenderer>(includeInactiveSprites);
        sprites.AddRange(found);
    }

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // Normalize initial facing
        if (useYRotationFlip)
            transform.localRotation = facesRightByDefault ? Quaternion.identity
                                                          : Quaternion.Euler(0, 180, 0);
        else
        {
            // For flipX path, initialize all sprites' facing
            bool initialFlipX = !facesRightByDefault; // flipX=true means face LEFT
            for (int i = 0; i < sprites.Count; i++)
                if (sprites[i]) sprites[i].flipX = initialFlipX;
        }
    }

    void FixedUpdate()
    {
        if (!player) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;

        Vector2 desired = dist > stopDistance ? toPlayer.normalized * speed : Vector2.zero;

        vel = Vector2.MoveTowards(rb.velocity, desired, acceleration * Time.fixedDeltaTime);
        rb.velocity = Vector2.Lerp(rb.velocity, vel, turnLerp * Time.fixedDeltaTime);

        // if (animator) animator.SetFloat("Speed", rb.velocity.magnitude);

        // Determine horizontal intent with a small deadzone
        float moveX = Mathf.Abs(rb.velocity.x) > 0.05f ? rb.velocity.x :
                      Mathf.Abs(desired.x)      > 0.05f ? desired.x : 0f;

        if (Mathf.Abs(moveX) <= 0.05f) return;

        if (useYRotationFlip)
        {
            // Rotation-based flip (mirrors entire hierarchy, great for child prefabs like weapons)
            bool wantRight = moveX > 0f;
            if (!facesRightByDefault) wantRight = !wantRight;

            transform.localRotation = wantRight ? Quaternion.identity
                                                : Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // Per-sprite flipX path
            bool wantFacingRight = facesRightByDefault ? (moveX >= 0f) : (moveX < 0f);
            bool flipX = !wantFacingRight; // flipX=true -> face left visually
            for (int i = 0; i < sprites.Count; i++)
                if (sprites[i]) sprites[i].flipX = flipX;
        }
    }
}
