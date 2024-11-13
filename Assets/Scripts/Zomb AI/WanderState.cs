using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    private float wanderTimer;
    private float wanderRadius = 20f;  // Area within which the zombie wanders
    private float timer;

    public WanderState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
        : base(controller, navMeshAgent, animator)
    {
        _navMeshAgent.speed = _controller.wanderSpeed;
        timer = wanderTimer;
    }

    public override void Enter()
    {
        Debug.Log("Entering Wander State");
        _controller.SetIsWalking(true);

        if (_controller.hasDied == true)
        {
            _controller.hasDied = false;

            _controller.animator.gameObject.GetComponent<Animator>().enabled = true;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.updatePosition = true;
            _navMeshAgent.updateRotation = true;
        }
    }

    public override void Execute()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(_controller.transform.position, wanderRadius, -1);
            _navMeshAgent.SetDestination(newPos);
            timer = 0;
        }

        // Check if the player is in range to switch to FollowPlayerState
        if (_controller.IsPlayerInRange(_controller.playerDetectionRange))
        {
            _controller.ChangeState(new FollowPlayerState(_controller, _navMeshAgent, _animator, _controller.chaseSpeed));  // Pass only 4 arguments
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Wander State");
        _controller.SetIsWalking(false);
    }

    // Helper method to find a random point within a radius
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
