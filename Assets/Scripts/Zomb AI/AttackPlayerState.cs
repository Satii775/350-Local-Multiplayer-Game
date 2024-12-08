using UnityEngine;
using UnityEngine.AI;

public class AttackPlayerState : State
{
    private float attackCooldown = 0f;
    private float attackRange;
    private float damage;
    private float attacksPerSecond;

    public AttackPlayerState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator, float attackRange, float damage, float attacksPerSecond)
        : base(controller, navMeshAgent, animator)
    {
        this.attackRange = attackRange;
        this.damage = damage;
        this.attacksPerSecond = attacksPerSecond;
        attackCooldown = 1f / attacksPerSecond; // Set the attack cooldown based on attacks per second
    }

    public override void Enter()
    {
        Debug.Log("Entering Attack Player State");
        _controller.SetIsAttacking(true);  // Set attacking animation
        _navMeshAgent.isStopped = true;    // Stop movement when attacking

    }

    public override void Execute()
    {
        // Continuously attack the player
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            PerformAttack();
            attackCooldown = 1f / attacksPerSecond;  // Reset cooldown
        }

        // If the player is out of attack range, go back to following
        if (!_controller.IsPlayerInRange(attackRange))
        {
            _controller.ChangeState(new FollowPlayerState(_controller, _navMeshAgent, _animator, _controller.chaseSpeed));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Attack Player State");
        _controller.SetIsAttacking(false);  // Reset attacking animation
        _navMeshAgent.isStopped = false;    // Resume movement
    }

    // Perform the attack logic
    private void PerformAttack()
    {
        Debug.Log("Zombie is attacking!");

        // Play the attack animation
        _animator.SetTrigger("Attack");

        // Apply damage to the player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);  // Apply damage
                Debug.Log("Player took " + damage + " damage!");
            }
        }
    }
}
