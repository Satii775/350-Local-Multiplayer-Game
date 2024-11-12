using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CubeMan : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 2500;
    private int currentHealth;
    private bool isDead = false;

    [Header("Attack Settings")]
    public float bossDamage = 20f;
    public float contactDamage = 25f;
    public GameObject[] projectilePrefabs;
    public float projectileSpeed = 365.76f;
    public float shootInterval = 2f;
    public float horizontalOffset = 0f;
    public float MOA = 1f;

    [Header("Stage Settings")]
    public float stage1Height = 8f;
    public float stage1DropDelay = 0.5f;
    public float stage2Distance = 7f;
    public float orbitSpeed = 5f;
    public float maxDistanceFromPlayer = 25f;  // Max allowed distance before CubeMan corrects position
    private int stage = 1;

    [Header("Destruction Settings")]
    public float timeBeforeDestruction = 0.5f;

    private Transform player;
    private Player playerScript;
    private Rigidbody rb;
    private bool isDropping = false;
    private float nextShootTime = 0f;
    private bool isFalling = false;
    private Vector3 fallPosition;
    private bool phase2Activated = false;
    private int currentProjectileIndex = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }

        // Set up Rigidbody to reduce knockback effects
        rb = GetComponent<Rigidbody>();
        rb.mass = 100f;  // Increase mass to make CubeMan harder to push
        rb.drag = 5f;    // Add drag for additional resistance to knockback
    }

    private void Update()
    {
        if (isDead) return;

        CorrectPositionIfTooFar();

        switch (stage)
        {
            case 1:
                HandleStage1();
                break;
            case 2:
                HandleStage2();
                break;
            case 3:
                HandleStage3();
                break;
        }

        if (Time.time >= nextShootTime)
        {
            ShootAtPlayer();
            nextShootTime = Time.time + shootInterval;
        }
    }

    // Checks if CubeMan is too far from the player and corrects position aggressively
    private void CorrectPositionIfTooFar()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, 0.1f);
            Debug.Log("CubeMan is returning aggressively to the player.");
        }
    }

    private void HandleStage1()
    {
        if (isDropping) return;

        Vector3 targetPosition = player.position + Vector3.up * stage1Height;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3f); // Faster approach

        if (!isDropping && Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            isDropping = true;
            Invoke(nameof(DropOntoPlayer), stage1DropDelay);
        }
    }

    private void DropOntoPlayer()
    {
        // Directly set position to player's position for accurate landing
        transform.position = new Vector3(player.position.x, player.position.y + 1f, player.position.z);

        if (Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            ApplyDamageToPlayer(bossDamage);
        }
        isDropping = false;
    }

    private void HandleStage2()
    {
        if (isFalling)
        {
            StartCoroutine(DramaticFall(fallPosition));
            isFalling = false;
        }
        else
        {
            OrbitAroundPlayer();
        }
    }

    private void OrbitAroundPlayer()
    {
        if (player == null) return;

        float anglePerSecond = (orbitSpeed * 360) / 60f;
        Vector3 offset = Quaternion.Euler(0, anglePerSecond * Time.deltaTime, 0) * (transform.position - player.position);
        transform.position = player.position + offset.normalized * stage2Distance;
        transform.LookAt(player.position);
    }

    private IEnumerator DramaticFall(Vector3 fallTarget)
    {
        float fallDuration = 2f;
        float fallStartTime = Time.time;

        while (Time.time - fallStartTime < fallDuration)
        {
            Vector3 fallPosition = Vector3.Lerp(transform.position, fallTarget, (Time.time - fallStartTime) / fallDuration);
            transform.position = fallPosition;
            yield return null;
        }
    }

    private void HandleStage3()
    {
        transform.Rotate(Vector3.up * 500 * Time.deltaTime);
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefabs.Length == 0 || player == null)
        {
            Debug.LogWarning("No projectiles assigned or player missing.");
            return;
        }

        Vector3 shootDirection = GetShootDirectionWithOffsetAndMOA();

        if (currentProjectileIndex >= projectilePrefabs.Length)
        {
            currentProjectileIndex = 0;
        }

        GameObject projectilePrefab = projectilePrefabs[currentProjectileIndex];
        GameObject projectileInstance = Instantiate(
            projectilePrefab,
            transform.position + transform.forward * 1.5f,
            Quaternion.LookRotation(shootDirection)
        );

        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody component!");
        }

        currentProjectileIndex = (currentProjectileIndex + 1) % projectilePrefabs.Length;
    }

    private Vector3 GetShootDirectionWithOffsetAndMOA()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 rightOffset = transform.right * horizontalOffset;
        Vector3 adjustedDirection = directionToPlayer + rightOffset;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float dispersionRadius = Mathf.Tan(MOA * 0.000290888f) * distanceToPlayer;
        float randomOffsetX = Random.Range(-dispersionRadius, dispersionRadius);
        float randomOffsetY = Random.Range(-dispersionRadius, dispersionRadius);

        Vector3 dispersionOffset = new Vector3(randomOffsetX, randomOffsetY, 0f);
        adjustedDirection += transform.TransformDirection(dispersionOffset);

        return adjustedDirection.normalized;
    }

    private void ApplyDamageToPlayer(float damage)
    {
        if (playerScript != null)
        {
            playerScript.TakeDamage(damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Projectile projectileScript = collision.gameObject.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                TakeDamage(projectileScript.damage);
                Destroy(collision.gameObject); // Destroy the projectile on impact
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth <= 1500 && !phase2Activated)
        {
            phase2Activated = true;
            stage = 2;
            fallPosition = transform.position;
            isFalling = true;
        }
        else if (currentHealth <= 750 && stage == 2)
        {
            stage = 3;
        }
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        Destroy(gameObject, timeBeforeDestruction);
    }
}

