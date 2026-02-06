public interface IPlayerState
{
    void Enter();
    void Tick(float dt);
    void FixedTick(float dt);
    void Exit();
}