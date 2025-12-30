using UnityEngine;
public sealed class PlayerStateMachine
{
    private IPlayerState _current;
    public IPlayerState Current => _current;
    public void ChangeState(IPlayerState next)
    {
        if (next == null) return;
        if (ReferenceEquals(_current, next)) return;
        _current?.Exit();
        _current = next;
        _current.Enter();
    }
    public void Tick(float dt)
    {
        _current?.Tick(dt);
    }
    public void FixedTick(float dt)
    {
        _current?.FixedTick(dt);
    }
}