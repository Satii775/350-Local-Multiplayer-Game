using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class Player : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 500f; // Set maximum health to 500
    private float currentHealth;                     // Current health

    private float enemyCollisionTimer = 0.5f; // Timer to track damage intervals
    private const float damageInterval = 0.5f; // Damage interval in seconds
    private const float enemyCollisionDamage = 50f; // Damage per interval when in contact with enemy

    private GameObject lostMenu;

    private GameObject player;

    private GameObject Manager;

    [SerializeField] private TextMeshProUGUI HealhtText;

    private void Awake()
    {
        ResetHealth(); // Initialize health to maxHealth

        player = this.gameObject;

        Manager = GameObject.FindWithTag("Manager");

        lostMenu = GameObject.FindWithTag("Lost menu");
        if (lostMenu != null)
        {
            lostMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Lost Menu not found in the scene.");
        }
    }

    private void Update()
    {
        HealhtText.text = "Health: " + currentHealth.ToString();
        // Decrease the timer while in Update
        if (enemyCollisionTimer < damageInterval)
        {
            enemyCollisionTimer += Time.deltaTime;
        }
    }

    // Public method to apply damage to the player
    public void TakeDamage(float damage)
    {
        if (damage <= 0 || currentHealth <= 0) return; // Ignore non-positive damage values or if player is already dead

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            TriggerDamageFeedback();
        }
    }

    // Method to handle player death and reset the scene
    private void Die()
    {
        currentHealth = 0;

        Manager.GetComponent<Player_Manager>().PlayerDied(player);

        transform.position = new Vector3(37, 1.97f, -555.79f);
        // Debug.Log("Player has died. Resetting scene...");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // if (lostMenu != null)
        // {
        //     lostMenu.SetActive(true);
        // }
        // else
        // {
        //     Debug.LogWarning("Lost Menu not found in the scene.");
        // }
        // Time.timeScale = 0f;
    }

    // Optional: Trigger damage feedback (e.g., visual or audio effect)
    private void TriggerDamageFeedback()
    {
        Debug.Log("Player received damage feedback.");
    }

    // Public method to heal the player (optional)
    public void Heal(float healAmount)
    {
        if (healAmount <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"Player healed by {healAmount}. Health is now {currentHealth}");
    }

    // Public method to reset player health (e.g., on respawn)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log("Player health reset to max.");
    }

    public void RespawnPlayer()
    {
        currentHealth = 50;
        Debug.Log("Player health reset to max.");
        
        transform.position = new Vector3(36.48f, 1.97f, -547.53f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Reset the timer on initial contact with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyCollisionTimer = 0f;
        }
        HandleDamageSource(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        // Check if we are in continuous contact with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Apply damage every half-second
            if (enemyCollisionTimer >= damageInterval)
            {
                TakeDamage(enemyCollisionDamage);
                enemyCollisionTimer = 0f; // Reset timer after applying damage
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Reset the timer when no longer in contact with the enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyCollisionTimer = damageInterval;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle other damage sources
        HandleDamageSource(other.gameObject);
    }

    // Method to handle different sources of damage based on tags
    private void HandleDamageSource(GameObject source)
    {
        if (source.CompareTag("EnemyProjectile"))
        {
            Projectile_B projectileScript = source.GetComponent<Projectile_B>();
            if (projectileScript != null)
            {
                TakeDamage(projectileScript.damage);
                Destroy(source); // Destroy the projectile after hitting the player
            }
        }
        else if (source.CompareTag("Boss"))
        {
            float bossContactDamage = 50f;
            TakeDamage(bossContactDamage);
            Debug.Log("Player took contact damage from the Boss.");
        }
    }
}
