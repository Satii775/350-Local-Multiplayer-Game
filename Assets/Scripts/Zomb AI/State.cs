using UnityEngine;
using UnityEngine.AI;

public abstract class State
{
    protected ZombieAIController _controller;
    protected NavMeshAgent _navMeshAgent;
    protected Animator _animator;

    public State(ZombieAIController controller, NavMeshAgent navMeshAgent, Animator animator)
    {
        _controller = controller;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
