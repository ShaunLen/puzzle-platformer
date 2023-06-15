using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class JumpState : InAirState
{
    private Vector2 _velocity;
    private float _jumpVelocity;
    private float _jumpGravity;
    
    private void OnPlayerReady()
    {
        _jumpVelocity = (2 * Stats.JumpHeight) / Stats.JumpTimeToApex * -1;
        _jumpGravity = (-2 * Stats.JumpHeight) / Mathf.Pow(Stats.JumpTimeToApex, 2) * -1;
    }

    public override void Enter()
    {
        base.Enter();
        _velocity.Y = _jumpVelocity;
        
        if(!InputManager.IsActionPressed(InputManager.Action.Jump))
            _velocity.Y /= 2;
    }

    public override State PhysicsProcess(double delta)
    {
        return base.PhysicsProcess(delta);
    }
}