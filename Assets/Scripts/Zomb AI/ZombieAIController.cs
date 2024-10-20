using UnityEngine;
using UnityEngine.AI;

public class ZombieAIController : MonoBehaviour
{
    private State _currentState;           // The current state the zombie is in
    public NavMeshAgent navMeshAgent;      // The NavMeshAgent that handles zombie movement
    public Animator animator;              // Animator to handle zombie animations
    public float health = 100f;            // Zombie's health

    // Adjustable parameters for behaviors
    public float chaseSpeed = 4.5f;        // Speed when chasing the player
    public float visionRange = 15f;        // Range within which the zombie can spot the player
    public float wanderRadius = 20f;       // Radius for random wandering
    public float wanderTimer = 5f;         // Time between each wander action
    public float wanderSpeed = 2.0f;       // Speed when wandering
    public float attackRange = 1.5f;       // Distance within which the zombie can attack
    public float damage = 10f;             // Damage dealt per attack
    public float attacksPerSecond = 1f;    // Number of attacks per second
    public float playerDetectionRange = 10f; // Detection range for player
    public float bodyDetectionRange = 10f;  // Detection range for bodies

    private GameObject player;             // Cached reference to the player

    void Start()
    {
        // Cache player reference to avoid repeated FindWithTag calls
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Please ensure there is a GameObject tagged 'Player'.");
            return;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Start with the Wander state
        ChangeState(new WanderState(this, navMeshAgent, animator));
    }

    void Update()
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

    // Detect whether the player is within the zombie's vision range
    public bool IsPlayerInSight()
    {
        // Ensure the player object is still in the scene
        if (player == null) return false;

        // Get the direction to the player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Calculate the angle between the zombie's forward direction and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Define the field of view (FOV) of the zombie, in this case, 270 degrees
        float fieldOfView = 270f;

        // Check if the player is within the FOV
        if (angleToPlayer > fieldOfView / 2)
        {
            return false; // Player is outside the zombie's field of view
        }

        // Check if the player is within range and there's nothing blocking the line of sight
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Ensure the player is within the zombie's vision range
        if (distanceToPlayer > visionRange)
        {
            return false; // Player is too far away
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, visionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;  // Player is in sight
            }
        }

        return false;  // Player is not in sight
    }

    // Detect whether a body is nearby and within range
    public bool IsBodyNearby()
    {
        GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");
        foreach (GameObject body in bodies)
        {
            float distanceToBody = Vector3.Distance(transform.position, body.transform.position);
            if (distanceToBody <= bodyDetectionRange)  // Check if the body is within the detection range
            {
                Debug.Log("Body detected within range.");
                return true;  // Body is nearby
            }
        }
        return false;  // No bodies nearby
    }

    public bool IsWithinAttackRange()
    {
        // Ensure the player object is still in the scene
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= attackRange;
    }

    // Switch to AttackPlayerState when the zombie is in range to attack
    public void ChangeToAttackState()
    {
        ChangeState(new AttackPlayerState(this, navMeshAgent, animator, attackRange, damage, attacksPerSecond));
    }

    public bool IsPlayerInRange(float range)
    {
        // Ensure the player object is still in the scene
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Return true if the player is within the given range
        return distanceToPlayer <= range;
    }

    // Animator control methods
    public void SetIsWalking(bool value)
    {
        animator.SetBool("isWalking", value);
    }

    public void SetIsAttacking(bool value)
    {
        animator.SetBool("isAttacking", value);
    }

    public void SetIsIdle(bool value)
    {
        animator.SetBool("isIdle", value);
    }

    public void TriggerFallForward()
    {
        animator.SetTrigger("isFallingForward");
    }

    public void TriggerFallBack()
    {
        animator.SetTrigger("isFallingBack");
    }

    public void SetIsDead(bool value)
    {
        animator.SetBool("isDead", value);
    }

    // Method for the zombie to take damage
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Zombie took {amount} damage. Health left: {health}");

        if (health <= 0)
        {
            ChangeState(new DieState(this, navMeshAgent, animator));  // Trigger death state when health is zero
        }
    }
}
