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
    public Transform firePoint; // added for gun shooting out gun tip
    public GameObject gun; // added for pulling out/stowing gun
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

        // Fire bullets only if gun is visible
        if (isFiring && gun.activeSelf && Time.time >= nextFireTime)
        {
            ShootAtMouse();
            nextFireTime = Time.time + fireRate;
        }

        // Flip sprite based on horizontal direction
        if (moveInput.x != 0 && moveInput.y == 0)
            spriteRenderer.flipX = moveInput.x > 0;


        // Aim gun at mouse
        if (gun != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector3 direction = mousePos - gun.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            gun.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Flip detection
            bool facingLeft = angle > 90f || angle < -90f;

            // Offsets
            Vector3 rightOffset = new Vector3(0.90f, -0.55f, 0f);  // tweak these
            Vector3 leftOffset = new Vector3(-0.90f, -0.55f, 0f); // tweak these

            // Apply sprite flip
            SpriteRenderer sr = gun.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipY = facingLeft;

            // Apply gun position offset
            gun.transform.localPosition = facingLeft ? leftOffset : rightOffset;
        }









        // Animator parameters
        if (anim != null)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
            anim.SetFloat("Speed", moveInput.sqrMagnitude);
            // anim.SetBool("IsFiring", isFiring);
        }



        // Toggle gun visibility when pressing 1
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            if (gun != null)
                gun.SetActive(!gun.activeSelf);
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

        // Direction from gun tip to mouse
        Vector2 direction = ((Vector2)(worldPos - firePoint.position)).normalized;

        // Spawn bullet at firepoint with proper rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = direction * bulletSpeed;
    }
}
