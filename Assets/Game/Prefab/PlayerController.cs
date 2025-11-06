using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer; // add this
    public float moveSpeed = 5f;
    private Vector2 moveInput;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.15f;
    private bool isFiring = false;
    private float nextFireTime = 0f;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    void Awake()
    {
        if (myRigidbody == null) myRigidbody = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Read input
        if (Mouse.current != null)
            isFiring = Mouse.current.leftButton.isPressed;
        else
            isFiring = false;

        // Fire bullets
        if (isFiring && Time.time >= nextFireTime)
        {
            ShootAtMouse();
            nextFireTime = Time.time + fireRate;
        }

        // Flip sprite based on horizontal direction
        if (moveInput.x != 0 && moveInput.y==0)
            spriteRenderer.flipX = moveInput.x > 0;

        // Animator parameters
        if (anim != null)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
            anim.SetFloat("Speed", moveInput.sqrMagnitude);
            anim.SetBool("IsFiring", isFiring);
        }
    }

    void FixedUpdate()
    {
        if (myRigidbody == null) return;
        myRigidbody.velocity = moveInput * moveSpeed;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
    }

    void ShootAtMouse()
    {
        if (bulletPrefab == null || Mouse.current == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Vector2 direction = ((Vector2)(worldPos - transform.position)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = direction * bulletSpeed;
    }
}
