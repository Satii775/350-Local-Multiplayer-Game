using UnityEngine;
using UnityEngine.AI;

public class DieState : State
{
    private bool hasDied = false;  // Track if death has already been processed
    private float timeBeforeDestruction = 1.2f; // Time to wait before destroying the zombie

    public DieState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
        : base(controller, navMeshAgent, animator) { }

    public override void Enter()
    {
        Debug.Log("Entering Die State");

        // Mark the zombie as dead in the AI controller
        _controller.SetIsDead(true);

        // Stop the NavMeshAgent and all movement
        _navMeshAgent.isStopped = true;
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        // Ensure the animation triggers only if it hasn't been triggered already
        if (!hasDied)
        {
            hasDied = true;

            // Play one of the fall animations (forward or backward randomly)
            if (Random.Range(0, 2) == 0)
            {
                _controller.TriggerFallForward();
            }
            else
            {
                _controller.TriggerFallBack();
            }

            // Schedule destruction of the zombie object after 0.5 seconds
            GameObject.Destroy(_controller.gameObject, timeBeforeDestruction);
        }
    }

    public override void Execute()
    {
        // No longer checking animation state to ensure destruction happens after 0.5 seconds.
    }

    private void FinalizeDeath()
    {
        // Additional cleanup logic if needed before destruction.
        Debug.Log("Finalizing death: Zombie will be destroyed after a short delay.");
    }

    public override void Exit()
    {
        // This state should never exit since the zombie is dead
        Debug.LogWarning("Zombie can't exit the Die state.");
    }
}
