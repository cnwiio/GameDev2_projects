using TarodevController;
using UnityEngine;
public sealed class MoveState : IPlayerState
{

    private readonly PlayerStateMachine _sm;
    private readonly ScriptableStats _stats;
    private readonly PlayerMovement _pm;
    private Vector2 _frameVelocity;
    public MoveState(ScriptableStats stats, PlayerStateMachine sm, PlayerMovement pm)
    {
        _sm = sm;
        _stats = stats;
        _pm = pm;
    }
    public void Enter()
    {
        if (_pm.IsDebug)
            Debug.Log("Enter: Move");
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

        // Transition to Idle State
        if (_pm.frameVelocity.x == 0)
        {
            _sm.ChangeState(new IdleState(_stats, _sm, _pm));
            return;
        }
    }

    public void FixedTick(float dt)
    {
    }

    //private void ApplyMovement() => _pc.rb.linearVelocity = _frameVelocity;

    public void Exit()
    {
    }
}