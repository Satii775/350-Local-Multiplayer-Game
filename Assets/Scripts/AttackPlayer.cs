using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    public Transform player;                // Reference to the player
    public float detectRange = 25f;         // Detection range
    public float moveSpeed = 2f;            // Approach speed
    public float rotationSpeed = 3f;        // Rotation speed
    public float stopDistance = 5f;         // Distance to stop moving towards player
    public float shootInterval = 2f;        // Time between shots
    public float projectileSpeed = 365.76f; // Speed of projectiles
    public GameObject[] projectilePrefabs;  // Array of projectile prefabs
    public float horizontalOffset = 0f;     // Horizontal offset from player for the shot direction
    public float MOA = 1f;                  // Minute of Angle dispersion, 1 MOA = 1 inch at 100 yards

    private float nextShootTime = 0f;       // Time to shoot next
    private int currentProjectileIndex = 0; // Current projectile index in array

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing!");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If player is within range, proceed with behavior
        if (distanceToPlayer <= detectRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }

            if (Time.time >= nextShootTime)
            {
                ShootAtPlayer();
                nextShootTime = Time.time + shootInterval;
            }
        }
    }

    // Move towards the player
    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        RotateTowardsPlayer();
    }

    // Rotate to face the player
    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Shoot a projectile towards the player with an optional horizontal offset and MOA-based dispersion
    void ShootAtPlayer()
    {
        if (projectilePrefabs.Length == 0)
        {
            Debug.LogError("No projectile prefabs assigned to AttackPlayer!");
            return;
        }

        // Determine shooting direction with MOA dispersion
        Vector3 shootDirection = GetShootDirectionWithOffsetAndMOA();

        // Ensure projectile index is within array bounds
        if (currentProjectileIndex >= projectilePrefabs.Length)
        {
            currentProjectileIndex = 0;
        }

        // Instantiate and shoot projectile
        GameObject projectilePrefab = projectilePrefabs[currentProjectileIndex];
        GameObject projectileInstance = Instantiate(
            projectilePrefab,
            transform.position + transform.forward * 1.5f, // Adjust position to spawn in front of enemy
            Quaternion.LookRotation(shootDirection)
        );

        // Apply velocity to projectile
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody component!");
        }

        // Assign this GameObject as the shooter
        Projectile_A projectileScript = projectileInstance.GetComponent<Projectile_A>();
        if (projectileScript != null)
        {
            projectileScript.firedBy = this.gameObject;
        }
        else
        {
            Debug.LogWarning("Projectile does not have a Projectile_A script.");
        }

        Debug.Log("Enemy shot projectile: " + projectilePrefab.name);

        // Cycle to the next projectile
        currentProjectileIndex = (currentProjectileIndex + 1) % projectilePrefabs.Length;
    }

    // Calculate shooting direction with an adjustable horizontal offset and MOA-based dispersion
    Vector3 GetShootDirectionWithOffsetAndMOA()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the right vector relative to the enemy's forward direction
        Vector3 rightOffset = transform.right * horizontalOffset;

        // Apply horizontal offset
        Vector3 adjustedDirection = directionToPlayer + rightOffset;

        // Apply realistic MOA-based dispersion
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Convert MOA to radians and then calculate the spread in Unity units
        // 1 MOA ≈ 0.000290888 radians (1 inch at 100 yards)
        float dispersionRadius = Mathf.Tan(MOA * 0.000290888f) * distanceToPlayer;

        // Random offset within the cone
        float randomOffsetX = Random.Range(-dispersionRadius, dispersionRadius);
        float randomOffsetY = Random.Range(-dispersionRadius, dispersionRadius);

        Vector3 dispersionOffset = new Vector3(randomOffsetX, randomOffsetY, 0f);
        adjustedDirection += transform.TransformDirection(dispersionOffset); // Transform local offset to world space

        return adjustedDirection.normalized;
    }
}
