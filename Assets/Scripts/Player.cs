using UnityEngine;
using UnityEngine.SceneManagement;  // Include this to access SceneManager

public class Player : MonoBehaviour
{
    public static Player instance;   // Singleton instance
    public float health = 100f;      // Player's health

    private void Awake()
    {
        // Ensure there is only one instance of the Player
        if (instance == null)
        {
            instance = this;  // Assign this instance to the static variable
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // Destroy any duplicates
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Player takes {damage} damage. Health left: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Reload the current active scene to reset the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
