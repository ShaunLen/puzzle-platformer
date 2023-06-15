using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class FallState : InAirState
{
    private Vector2 _velocity;
    private float _fallGravity;
    
    public override void Enter()
    {
        base.Enter();
        _fallGravity = (-2 * Stats.JumpHeight) / Mathf.Pow(Stats.JumpTimeToGround, 2) * -1;
    }

    public override State PhysicsProcess(double delta)
    {
        _velocity.Y += _fallGravity * (float) delta;
        _velocity.Y = Mathf.Min(_velocity.Y, Stats.MaxFallSpeed);

        Player.Velocity = new Vector2(Player.Velocity.X, _velocity.Y);

        return Player.IsOnFloor() ? StateMachine.GetState<IdleState>() : null;
    }
}