using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    public float timeBeforeDestruction = 0.5f;

    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Ignore damage if already dead

        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy has died!");

        var wanderScript = GetComponent<WanderAround>();
        var attackScript = GetComponent<AttackPlayer>();
        if (wanderScript != null) wanderScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

        if (rb != null) rb.isKinematic = false;

        Destroy(gameObject, timeBeforeDestruction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Get the Projectile component to retrieve the damage value
            Projectile projectileScript = collision.gameObject.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                TakeDamage(projectileScript.damage);  // Apply the damage from the projectile
                Debug.Log($"Enemy hit by PlayerProjectile. Damage taken: {projectileScript.damage}. Current health: {currentHealth}");
            }
            else
            {
                Debug.LogWarning("PlayerProjectile collided, but no Projectile script was found.");
            }

            // Destroy the projectile on impact
            Destroy(collision.gameObject);
        }
    }
}
