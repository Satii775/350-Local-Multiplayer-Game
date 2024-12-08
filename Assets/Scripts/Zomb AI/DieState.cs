using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DieState : State
{
    private float timeBeforeDestruction = 1.2f; // Time to wait before destroying the zombie

    public DieState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
        : base(controller, navMeshAgent, animator) { }

    public override void Enter()
    {
        //Debug.Log("Entering Die State");

        // Stop the NavMeshAgent and all movement
        _navMeshAgent.isStopped = true;
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        // Ensure the animation triggers only if it hasn't been triggered already
        if (!_controller.hasDied)
        {
            _controller.hasDied = true;

            _controller.Zomb_Spawner.GetComponent<Zombies_Manager>().KillZombie();

            // Set the zombie as dead
            _controller.SetIsDead(true);

            // Play one of the fall animations (forward or backward randomly)
            if (Random.Range(0, 2) == 0)
            {
                _controller.TriggerFallForward(true);
            }
            else
            {
                _controller.TriggerFallBack(true);
            }

            // Schedule destruction of the zombie object after 0.5 seconds
            _controller.StartCoroutine(DelayedTeleport(timeBeforeDestruction));

            _controller.ChangeState(new IdleState(_controller, _navMeshAgent, _animator));
            
        }
    }

    IEnumerator DelayedTeleport(float delay)
    {
        yield return new WaitForSeconds(delay);
        FinalizeDeath();
    }

    public override void Execute()
    {
        // No longer checking animation state to ensure destruction happens after 0.5 seconds.
    }

    private void FinalizeDeath()
    {
        _controller.Zombie.transform.position = new Vector3(29.49126f, -17.84f, -526.9813f);
        _controller.animator.gameObject.GetComponent<Animator>().enabled = false;
    }

    public override void Exit()
    {
        // This state should never exit since the zombie is dead
        //Debug.LogWarning("Zombie can't exit the Die state.");
    }
}
