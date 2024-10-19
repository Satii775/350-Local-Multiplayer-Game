using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public float detectionRange = 20f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float knockbackDistance = 1f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 365.76f; // 1200 fps converted to meters per second
    public Transform player;

    private Vector3 wanderTarget;
    private float timer;
    private bool isChasing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = wanderTimer;
        SetNewWanderTarget();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange || isChasing)
        {
            // Chase the player
            isChasing = true;
            ChasePlayer();

            // Shoot back at the player if in range
            if (distanceToPlayer <= detectionRange / 2)
            {
                ShootProjectile();
            }
        }
        else
        {
            // Random wandering
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                SetNewWanderTarget();
                timer = 0;
            }

            Wander();
        }
    }

    void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y; // Keep the same height
        wanderTarget = randomDirection;
    }

    void Wander()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
        rb.MovePosition(transform.position + direction * Time.deltaTime);
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * Time.deltaTime);
    }

    public void TakeDamage(Vector3 hitDirection)
    {
        health--;
        Debug.Log("Enemy hit! Health remaining: " + health);

        // Apply knockback
        Vector3 knockback = hitDirection.normalized * knockbackDistance;
        rb.AddForce(knockback, ForceMode.Impulse);
        Debug.Log("Knockback applied: " + knockback);

        // Detect the player if not already chasing
        if (!isChasing)
        {
            isChasing = true;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, transform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = (player.position - transform.position).normalized * projectileSpeed;
        projectileRb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Vector3 hitDirection = transform.position - other.transform.position;
            TakeDamage(hitDirection);
            Destroy(other.gameObject); // Destroy the projectile after hit
        }
    }
}
