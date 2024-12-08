using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAIController : MonoBehaviour
{
    [Header("Zombie State Machine")]
    private State _currentState;           // The current state the zombie is in
    public NavMeshAgent navMeshAgent;      // The NavMeshAgent that handles zombie movement
    public Animator animator;              // Animator to handle zombie animations

    [Header("Zombie Stats")]
    public float health = 100f;            // Zombie's health

    // Adjustable parameters for behaviors
    public float chaseSpeed = 4.5f;        // Speed when chasing the player
    public float visionRange = 1000f;        // Range within which the zombie can spot the player
    public float wanderRadius = 20f;       // Radius for random wandering
    public float wanderTimer = 5f;         // Time between each wander action
    public float wanderSpeed = 2.0f;       // Speed when wandering
    public float attackRange = 3f;       // Distance within which the zombie can attack
    public float damage = 2f;             // Damage dealt per attack
    public float attacksPerSecond = 1f;    // Number of attacks per second
    public float playerDetectionRange = 10f; // Detection range for player
    public float bodyDetectionRange = 10f;  // Detection range for bodies

    [Header("Game Objects")]
    public GameObject[] players;             // Cached reference to the player
    public GameObject Zombie;             // Cached reference to the Zombie
    public GameObject Zomb_Spawner;

    [Header("MISC")]
    public bool hasDied = false;
    public bool isIdle = true;

    void Start()
    {
        // Cache player reference to avoid repeated FindWithTag calls
        players = GameObject.FindGameObjectsWithTag("Player");

        Zomb_Spawner = GameObject.FindWithTag("Zomb Manager");

        Zombie = this.gameObject;

        if (Zombie == null)
        {
            Debug.LogError("Unable to grab Zombie GameObject");
            return;
        }

        if (players == null)
        {
            Debug.LogError("Players not found in the scene. Please ensure there is a GameObject tagged 'Player'.");
            return;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Start with the Wander state
        ChangeState(new IdleState(this, navMeshAgent, animator));
    }

    void Update()
    {
        if (isIdle == false)
        {

            if (_currentState != null)
            {
                _currentState.Execute();  // Run the current state's logic
            }

            // Check health and transition to DieState if needed
            if (health <= 0)
            {
                ChangeState(new DieState(this, navMeshAgent, animator));
            }

            // ** PRIORITIZE PLAYER DETECTION FIRST **
            if (IsPlayerInSight())  // Check if the player is in sight
            {
                // If player is in sight, chase them and stop checking for bodies
                if (!(_currentState is FollowPlayerState))
                {
                    ChangeState(new FollowPlayerState(this, navMeshAgent, animator, chaseSpeed));
                    return; // Stop further checks once player is detected
                }
            }
            // ** Check for Bodies Only If No Player is Detected **
            else if (IsBodyNearby() && !(_currentState is FollowPlayerState))  // If no player detected, check for bodies
            {
                if (!(_currentState is EatBodyState))
                {
                    ChangeState(new EatBodyState(this, navMeshAgent, animator));  // Transition to eating the body
                }
            }
        }
    }

    public void SpawnZombies()
    {
        health = 100f;
        ChangeState(new WanderState(this, navMeshAgent, animator));
        isIdle = false;
    }

    // Method to change the current state
    public void ChangeState(State newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = newState;
        _currentState.Enter();
    }

    // Detect whether any player is within the zombie's vision range
    public bool IsPlayerInSight()
    {
        if (players == null || players.Length == 0) return false;

        foreach (var player in players)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float fieldOfView = 360;

            if (angleToPlayer <= fieldOfView)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceToPlayer <= visionRange)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, visionRange))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    // Detect whether a body is nearby and within range
    public bool IsBodyNearby()
    {
        GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");
        foreach (GameObject body in bodies)
        {
            float distanceToBody = Vector3.Distance(transform.position, body.transform.position);
            if (distanceToBody <= bodyDetectionRange)
            {
                Debug.Log("Body detected within range.");
                return true;
            }
        }
        return false;
    }

    public bool IsWithinAttackRange()
    {
        if (players == null || players.Length == 0) return false;

        foreach (var player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                return true;
            }
        }
        return false;
    }

    // Switch to AttackPlayerState when the zombie is in range to attack
    public void ChangeToAttackState()
    {
        ChangeState(new AttackPlayerState(this, navMeshAgent, animator, attackRange, damage, attacksPerSecond));
    }

    public bool IsPlayerInRange(float range)
    {
        if (players == null || players.Length == 0) return false;

        foreach (var player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= range)
            {
                return true;
            }
        }
        return false;
    }

    // Animator control methods
    public void SetIsWalking(bool value)
    {
        animator.SetBool("isWalking", value);
        animator.SetBool("isDead", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", false);
    }

    public void SetIsAttacking(bool value)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isAttacking", value);
        animator.SetBool("isIdle", false);
    }

    public void SetIsIdle(bool value)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", value);
    }

    public void TriggerFallForward(bool value)
    {
        animator.SetTrigger("isFallingForward");
    }

    public void TriggerFallBack(bool value)
    {
        animator.SetTrigger("isFallingBack");
    }

    public void SetIsDead(bool value)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", value);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", false);
    }

    // Method for the zombie to take damage
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Zombie took {amount} damage. Health left: {health}");

        if (health <= 0)
        {
            ChangeState(new DieState(this, navMeshAgent, animator));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            // Check if the projectile has a "Projectile" component to retrieve damage information
            Projectile projectileScript = collision.gameObject.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                TakeDamage(projectileScript.damage);  // Apply the damage from the projectile
                Destroy(collision.gameObject);  // Destroy the projectile on impact
                Debug.Log($"Zombie hit by PlayerProjectile. Damage taken: {projectileScript.damage}. Current health: {health}");
            }
            else
            {
                Debug.LogWarning("PlayerProjectile collided, but no Projectile script was found.");
            }
        }
    }
}
