using TarodevController;
using UnityEngine;
public sealed class JumpState : IPlayerState
{

    private readonly PlayerStateMachine _sm;
    private readonly ScriptableStats _stats;
    private readonly PlayerMovement _pm;
    private Vector2 _frameVelocity;
    public JumpState(ScriptableStats stats, PlayerStateMachine sm, PlayerMovement pm)
    {
        _sm = sm;
        _stats = stats;
        _pm = pm;
    }
    public void Enter()
    {
        if (_pm.IsDebug)
            Debug.Log("Enter: Jump");
    }
    public void Tick(float dt)
    {
        // Transition to Fall State
        if (_pm.frameVelocity.y <= 0 && !_pm.grounded)
        {
            _sm.ChangeState(new FallState(_stats, _sm, _pm));
            return;
        }

        if(_pm.grounded)
        {
            // Transition to Idle State
            if (_pm.frameVelocity.x == 0)
            {
                _sm.ChangeState(new IdleState(_stats, _sm, _pm));
                return;
            }
            // Transition to Move State
            if (_pm.frameVelocity.x != 0)
            {
                _sm.ChangeState(new MoveState(_stats, _sm, _pm));
                return;
            }
        }
    }

    public void FixedTick(float dt)
    {
    }
    public void Exit()
    {
    }
}