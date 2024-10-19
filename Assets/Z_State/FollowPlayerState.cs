using UnityEngine;
using UnityEngine.AI;

public class FollowPlayerState : State
{
    public FollowPlayerState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator, float chaseSpeed)
        : base(controller, navMeshAgent, animator)
    {
        _navMeshAgent.speed = chaseSpeed; // Set the chase speed
        _navMeshAgent.isStopped = false;  // Ensure the agent is not stopped
    }

    public override void Enter()
    {
        Debug.Log("Entering Follow Player State");
        _controller.SetIsWalking(true);  // Set walking/running animation
    }

    public override void Execute()
    {
        // Move toward the player
        MoveToPlayer();

        // Check if the player is within attack range
        if (_controller.IsPlayerInRange(_controller.attackRange))  // Use the new IsPlayerInRange() method
        {
            _controller.ChangeToAttackState();  // Switch to the attack state when in range
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Follow Player State");
        _controller.SetIsWalking(false);  // Reset walking/running animation
    }

    private void MoveToPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _navMeshAgent.SetDestination(player.transform.position);  // Set destination to the player's position
        }
    }
}
