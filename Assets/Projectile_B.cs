using UnityEngine;

public class Projectile_B : MonoBehaviour
{
    public int damage = 50;               // Damage dealt by the projectile
    public float speed = 30.48f;          // Speed in Unity units per second (approx. 100 ft per second)
    public GameObject firedBy;            // Reference to the GameObject that fired the projectile
    public GameObject explosionPrefab;    // Explosion effect prefab

    private Transform player;
    private Rigidbody rb;

    private void Start()
    {
        // Locate the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Initialize Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Projectile_B is missing a Rigidbody component.");
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = false;

        // Aim the projectile at the player and set its velocity
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * speed;
            Debug.Log("Projectile fired towards player at velocity: " + rb.velocity);
        }
        else
        {
            Debug.LogWarning("Player not found; projectile will not move.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hit the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);  // Apply damage to the player
                Debug.Log("Projectile hit player, dealing " + damage + " damage.");
            }
            Explode();
        }
        else if (collision.gameObject != firedBy) // Ensure it doesn't collide with the entity that fired it
        {
            Debug.Log("Projectile collided with " + collision.gameObject.name);
            Explode();  // Explode on any other collision
        }
    }

    void Explode()
    {
        // Instantiate the explosion effect, if assigned
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Projectile exploded.");
        }
        Destroy(gameObject);  // Destroy the projectile after the explosion
    }
}
