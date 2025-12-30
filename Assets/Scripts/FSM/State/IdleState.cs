using TarodevController;
using UnityEngine;
public sealed class IdleState : IPlayerState
{

    private readonly PlayerStateMachine _sm;
    private readonly ScriptableStats _stats;
    private readonly PlayerMovement _pm;
    public IdleState(ScriptableStats stats, PlayerStateMachine sm, PlayerMovement pm)
    {
        _sm = sm;
        _stats = stats;
        _pm = pm;
    }
    public void Enter()
    {
        if (_pm.IsDebug)
            Debug.Log("Enter: Idle");
    }
    public void Tick(float dt)
    {
        if (!_pm.grounded)
        {
            // Transition to Fall State
            if (_pm.frameVelocity.y <= 0)
            {
                _sm.ChangeState(new FallState(_stats, _sm, _pm));
                return;
            }

            // Transition to Jump State
            _sm.ChangeState(new JumpState(_stats, _sm, _pm));
            return;
        }

        // Transition to Move State
        if (_pm.frameVelocity.x != 0)
        {
            _sm.ChangeState(new MoveState(_stats, _sm, _pm));
            return;
        }
    }

    public void FixedTick(float dt)
    {
    }

    public void Exit()
    {
    }
}