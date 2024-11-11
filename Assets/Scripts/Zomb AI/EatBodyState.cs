using UnityEngine;
using UnityEngine.AI;

public class EatBodyState : State
{
    private GameObject targetBody;  // The body the zombie will eat
    private bool isEating;

    private float bodyDetectionRange = 10f;  // Detection range for bodies
    private float stoppingDistance = 1f;     // Distance to stop near the body

    public EatBodyState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
        : base(controller, navMeshAgent, animator)
    {
        isEating = false;
    }

    public override void Enter()
    {
        Debug.Log("Entering Eat Body State");

        // Find the nearest body within detection range
        targetBody = DetectBodyInRange();

        if (targetBody == null)
        {
            Debug.LogWarning("No body detected. Returning to Wander state.");
            _controller.ChangeState(new WanderState(_controller, _navMeshAgent, _animator));
            return;
        }

        // Move towards the detected body
        _navMeshAgent.SetDestination(targetBody.transform.position);
        _navMeshAgent.stoppingDistance = stoppingDistance;  // Stop 1 meter from the body
        _controller.SetIsWalking(true);  // Set walking animation
    }

    public override void Execute()
    {
        // Check if the player is detected, if so, switch to FollowPlayerState
        if (_controller.IsPlayerInSight())
        {
            Debug.Log("Player detected! Switching to FollowPlayerState.");
            _controller.ChangeState(new FollowPlayerState(_controller, _navMeshAgent, _animator, _controller.chaseSpeed));
            return;  // Stop executing EatBodyState logic
        }

        // Check if the zombie has reached the body
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !isEating)
        {
            // Start eating the body once within range
            isEating = true;
            _navMeshAgent.isStopped = true;  // Stop the zombie from moving
            Debug.Log("Zombie is now eating the body.");
            _animator.SetTrigger("Eat");  // Play eating animation
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Eat Body State");
        _navMeshAgent.isStopped = false;  // Resume movement
        _controller.SetIsWalking(false);  // Reset walking animation
    }

    // Method to detect a body within the specified range
    private GameObject DetectBodyInRange()
    {
        GameObject[] bodies = GameObject.FindGameObjectsWithTag("Body");
        GameObject nearestBody = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject body in bodies)
        {
            float distanceToBody = Vector3.Distance(_controller.transform.position, body.transform.position);
            if (distanceToBody <= bodyDetectionRange)
            {
                if (distanceToBody < nearestDistance)
                {
                    nearestDistance = distanceToBody;
                    nearestBody = body;
                }
            }
        }

        return nearestBody;
    }
}
