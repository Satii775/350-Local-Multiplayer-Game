using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;   // Maximum health
    private int currentHealth;    // Current health
    private bool isDead = false;  // Is the enemy dead?
    public float timeBeforeDestruction = 0.5f; // Time before destroying the object after death

    private Rigidbody rb;         // Reference to Rigidbody for physics interactions

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        // Reduce the enemy's health by the damage amount
        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage! Current health: " + currentHealth);

        // If the health drops to zero or below, trigger death
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy has died!");

        // Disable any movement or AI scripts
        WanderAround wanderScript = GetComponent<WanderAround>();
        AttackPlayer attackScript = GetComponent<AttackPlayer>();
        if (wanderScript != null) wanderScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

        // Enable physics so the enemy can fall naturally, if desired
        if (rb != null)
        {
            rb.isKinematic = false; // Allow the rigidbody to be affected by physics
        }

        // Destroy the enemy object after a short delay
        Destroy(gameObject, timeBeforeDestruction);
    }
}
