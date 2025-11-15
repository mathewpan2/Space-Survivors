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


    [Header("Melee Weapon")]
    public GameObject meleeWeapon;    // Your scythe/weapon GameObject
    public Transform meleePoint;      // Point in front of weapon where hit is detected
    public float meleeRange = 1f;     // Radius of melee hit
    public int meleeDamage = 1;       // Damage dealt by melee attack
    public LayerMask enemyLayers;     // Enemy layers to hit
    public float meleeCooldown = 0.5f;// Delay between attacks
    private float nextMeleeTime = 0f; // Internal timer
    private bool isMeleeActive = false; // Whether melee is active



    [Header("Animation")]
    [SerializeField] private Animator anim;

    [SerializeField] private Animator playerAnim;

    void Awake()
    {
        if (myRigidbody == null) myRigidbody = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();

        // Ensure weapons are hidden at start
        if (gun != null) gun.SetActive(false);
        if (meleeWeapon != null) meleeWeapon.SetActive(false);

        // Ensure melee is marked as inactive internally
        isMeleeActive = false;
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

        // Aim melee weapon at mouse
        if (meleeWeapon != null && isMeleeActive)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector3 direction = mousePos - meleeWeapon.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            meleeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Flip detection
            bool facingLeft = angle > 90f || angle < -90f;

            // Offsets (tweak for your sprite)
            Vector3 rightOffset = new Vector3(1.0f, 0f, 0f); // farther to the right
            Vector3 leftOffset = new Vector3(-1.0f, 0f, 0f); // farther to the left


            // Apply sprite flip
            SpriteRenderer sr = meleeWeapon.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipY = facingLeft;

            // Apply local position offset
            meleeWeapon.transform.localPosition = facingLeft ? leftOffset : rightOffset;
        }


        

        // Melee attack when active
        if (isMeleeActive && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextMeleeTime)
        {
            MeleeAttack();
            nextMeleeTime = Time.time + meleeCooldown;
        }



        // Animator parameters
        if (anim != null)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
            anim.SetFloat("Speed", moveInput.sqrMagnitude);
            // anim.SetBool("IsFiring", isFiring);
        }

        if (playerAnim != null)
        {
            playerAnim.SetFloat("MoveX", moveInput.x);
            playerAnim.SetFloat("MoveY", moveInput.y);
            playerAnim.SetFloat("Speed", moveInput.sqrMagnitude);
        }



        // Toggle gun visibility when pressing 1
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            if (gun != null)
            {
                gun.SetActive(true);            // Show gun
                if (meleeWeapon != null)        // Hide sword
                {
                    meleeWeapon.SetActive(false);
                    isMeleeActive = false;
                }
            }
        }

        // Toggle melee weapon visibility when pressing 2
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            if (meleeWeapon != null)
            {
                meleeWeapon.SetActive(true);    // Show sword
                isMeleeActive = true;

                if (gun != null)                // Hide gun
                    gun.SetActive(false);
            }
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




    void MeleeAttack()
    {
        if (anim != null)
            anim.SetTrigger("Melee");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleePoint.position, meleeRange, enemyLayers);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            var hp = enemy.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(meleeDamage);
                Debug.Log("Melee hit " + enemy.name);
            }
        }
    }



    void OnDrawGizmosSelected()
    {
        if (meleePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
    }



}
