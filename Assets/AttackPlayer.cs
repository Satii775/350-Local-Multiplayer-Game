using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    public Transform player;               // Reference to the player
    public float detectRange = 25f;        // Detection range
    public float moveSpeed = 2f;           // Slow approach speed
    public float rotationSpeed = 3f;       // Speed of rotation
    public float stopDistance = 5f;        // Minimum distance to stop moving toward player
    public float shootInterval = 2f;       // Time between shots
    public float projectileSpeed = 365.76f;// Projectile speed
    public GameObject projectilePrefab;    // Projectile prefab
    public LayerMask wallLayer;            // Layer mask for walls

    private float nextShootTime = 0f;      // Time to track when to shoot next
    private bool playerDetected = false;   // Is the player detected?

    void Update()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within detection range
        if (distanceToPlayer <= detectRange)
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }

        // If the player is detected, approach and shoot
        if (playerDetected)
        {
            // Move toward the player if we're not too close
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }

            // Shoot at the player if it's time to shoot
            if (Time.time >= nextShootTime)
            {
                ShootAtPlayer();
                nextShootTime = Time.time + shootInterval;
            }
        }
    }

    // Move towards the player while avoiding obstacles
    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Only move if there is no wall in front
        if (!IsWallInFront())
        {
            // Move towards the player
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;

            // Rotate smoothly to face the player
            RotateTowardsPlayer();
        }
    }

    // Rotate smoothly to face the player
    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Check if a wall is in front of the enemy
    bool IsWallInFront()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, wallLayer))
        {
            return hit.collider != null && hit.collider.CompareTag("Wall");
        }
        return false;
    }

    // Shoot a projectile at the player
    void ShootAtPlayer()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Calculate the direction to the player and shoot
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = directionToPlayer * projectileSpeed;

        Debug.Log("Enemy shot at the player.");
    }

    // Reaction to being shot by the player
    public void OnShotByPlayer()
    {
        // Trigger aggressive behavior if the enemy is shot
        playerDetected = true;
        Debug.Log("Enemy has been shot and is now aggressive.");
    }
}
