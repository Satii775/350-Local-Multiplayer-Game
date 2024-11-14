using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : State
{
    private float timeBeforeDestruction = 1.2f; // Time to wait before destroying the zombie

    public IdleState(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
        : base(controller, navMeshAgent, animator) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
    }


    public override void Execute()
    {
        
    }

    private void FinalizeDeath()
    {

    }

    public override void Exit()
    {

    }
}
