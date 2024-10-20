using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public int damage = 25; // Damage dealt by the projectile
    public float speed = 1300f; // Speed of the projectile in units per second

    private Rigidbody rb;

    private void Start()
    {
        // Ensure the Rigidbody is set up correctly
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;  // Set to false to allow physics-based movement
        rb.useGravity = false;  // Disable gravity if not needed for projectile movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // For fast-moving objects

        // Set the initial velocity of the projectile without Time.deltaTime
        rb.velocity = transform.forward * speed; // Removed Time.deltaTime
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit has a ZombieAIController
        ZombieAIController zombieController = other.GetComponent<ZombieAIController>();
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (zombieController != null)
        {
            Debug.Log($"Projectile hit a zombie, dealing {damage} damage.");
            zombieController.TakeDamage(damage);
            Destroy(gameObject);  // Destroy the projectile after the hit
        }
        else if (enemyHealth != null)
        {
            Debug.Log($"Projectile hit an enemy with EnemyHealth, dealing {damage} damage.");
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);  // Destroy the projectile after the hit
        }
        else
        {
            Debug.Log("Projectile hit an object that is not an enemy.");
        }
    }
}
