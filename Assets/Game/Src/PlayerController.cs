using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public PlayerStats stats;            // 🔹 new: central stats

    private Vector2 moveInput;

    [Header("Gun")]
    public GameObject bulletPrefab;
    public Transform firePoint;          // gun tip
    public GameObject gun;               // gun object
    private bool isFiring = false;
    private float nextFireTime = 0f;

    [Header("Sword")]
    public GameObject sword;
    public Transform swordPoint;
    public LayerMask enemyLayers;
    private float nextSwordTime = 0f;
    private bool isSwordActive = false;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private Animator playerAnim;

    void Awake()
    {
        if (myRigidbody == null) myRigidbody = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (anim == null) anim = GetComponent<Animator>();
        if (!stats) stats = GetComponent<PlayerStats>();

        if (!stats)
            Debug.LogError("[Movement] Missing PlayerStats on player!");

        // Ensure weapons are hidden at start
        if (gun != null) gun.SetActive(false);
        if (sword != null) sword.SetActive(false);

        isSwordActive = false;
    }

    void Update()
    {
        if (Mouse.current != null)
            isFiring = Mouse.current.leftButton.isPressed;
        else
            isFiring = false;

        // 🔹 Use stats.fireRate (seconds between shots)
        if (isFiring && gun != null && gun.activeSelf && Time.time >= nextFireTime && stats != null)
        {
            ShootAtMouse();
            nextFireTime = Time.time + stats.gunFireRate;
        }

        // Flip sprite based on horizontal direction
        if (moveInput.x != 0 && moveInput.y == 0)
            spriteRenderer.flipX = moveInput.x > 0;

        // Aim gun at mouse
        if (gun != null && Mouse.current != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector3 direction = mousePos - gun.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gun.transform.rotation = Quaternion.Euler(0, 0, angle);

            bool facingLeft = angle > 90f || angle < -90f;

            Vector3 rightOffset = new Vector3(0.90f, -0.55f, 0f);
            Vector3 leftOffset  = new Vector3(-0.90f, -0.55f, 0f);

            SpriteRenderer sr = gun.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipY = facingLeft;

            gun.transform.localPosition = facingLeft ? leftOffset : rightOffset;
        }

        // Aim sword weapon at mouse
        if (sword != null && isSwordActive && Mouse.current != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0f;

            Vector3 direction = mousePos - sword.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle);

            bool facingLeft = angle > 90f || angle < -90f;

            Vector3 rightOffset = new Vector3(1.0f, 0f, 0f);
            Vector3 leftOffset  = new Vector3(-1.0f, 0f, 0f);

            SpriteRenderer sr = sword.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipY = facingLeft;

            sword.transform.localPosition = facingLeft ? leftOffset : rightOffset;
        }

        // 🔹 Sword attack using stats.swordCooldown
        if (isSwordActive && Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            stats != null && Time.time >= nextSwordTime)
        {
            SwordAttack();
            nextSwordTime = Time.time + stats.swordAttackSpeed;
        }

        if (playerAnim != null)
        {
            playerAnim.SetFloat("MoveX", moveInput.x);
            playerAnim.SetFloat("MoveY", moveInput.y);
            playerAnim.SetFloat("Speed", moveInput.sqrMagnitude);
        }

        // Toggle gun visibility (1)
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            if (gun != null)
            {
                gun.SetActive(true);
                if (sword != null)
                {
                    sword.SetActive(false);
                    isSwordActive = false;
                }
            }
        }

        // Toggle sword weapon visibility (2)
        if (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            if (sword != null)
            {
                sword.SetActive(true);
                isSwordActive = true;

                if (gun != null)
                    gun.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (myRigidbody == null || stats == null) return;
        myRigidbody.velocity = moveInput * stats.moveSpeed;   // 🔹 use stats.moveSpeed
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
    }

    void ShootAtMouse()
    {
        if (bulletPrefab == null || firePoint == null || Mouse.current == null || stats == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Vector2 baseDirection = ((Vector2)(worldPos - firePoint.position)).normalized;
        
        // Fire main bullet + extra shots
        int totalShots = 1 + stats.extraShots;
        
        for (int i = 0; i < totalShots; i++)
        {
            // Calculate angle offset: center shot has no offset, others spread symmetrically
            float angleOffset = 0f;
            if (totalShots > 1)
            {
                // Spread shots evenly: e.g., 3 shots = -15°, 0°, +15°
                float halfSpread = (totalShots - 1) * stats.extraShotAngle / 2f;
                angleOffset = -halfSpread + i * stats.extraShotAngle;
            }
            
            // Rotate direction by angle offset
            Vector2 direction = RotateVector(baseDirection, angleOffset);
            
            // Calculate rotation to face the direction of travel
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);
            
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

            if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
                rb.velocity = direction * stats.bulletSpeed;

            if (bullet.TryGetComponent<ProjectileDamage>(out var proj))
            {
                proj.damage = stats.gunDamage;
            }
        }
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    void SwordAttack()
    {
        if (anim != null)
            anim.SetTrigger("Sword");

        if (stats == null || swordPoint == null) return;

        // 🔹 use stats.swordRange
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            swordPoint.position,
            stats.swordRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            var hp = enemy.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(stats.swordDamage);  // 🔹 use stats.swordDamage
                Debug.Log("Sword hit " + enemy.name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (swordPoint == null) return;
        // in editor, stats might be null, so fall back
        float range = stats ? stats.swordRange : 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(swordPoint.position, range);
    }
}
