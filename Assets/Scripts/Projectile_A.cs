using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile_A : MonoBehaviour
{
    public int damage = 25;          // Basic damage
    public float speed = 300f;       // Projectile speed
    public GameObject firedBy;       // Reference to the object that fired this projectile

    private Rigidbody rb;

    private void Start()
    {
        // Ensure Rigidbody is set up and apply initial velocity
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if projectile hits the player
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);  // Apply damage
                Destroy(gameObject);              // Destroy projectile after hitting
            }
        }
        else
        {
            Destroy(gameObject);  // Destroy projectile on any other collision
        }
    }
}
