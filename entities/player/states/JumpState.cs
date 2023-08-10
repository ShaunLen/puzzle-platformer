using System.Reflection.Metadata;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class JumpState : InAirState
{
    private Vector2 _velocity;
    private float _jumpVelocity;
    private float _jumpGravity;

    public override void _Ready()
    {
        base._Ready();
        
        /* Signal Connections */
        Player.StatsUpdated += OnPlayerStatsUpdated;
    }

    private void OnPlayerStatsUpdated()
    {
        Enabled = Stats.JumpEnabled;
        _jumpVelocity = (2 * Stats.JumpHeight) / Stats.JumpTimeToApex * -1;
        _jumpGravity = (-2 * Stats.JumpHeight) / Mathf.Pow(Stats.JumpTimeToApex, 2) * -1;
    }

    public override void Enter()
    {
        base.Enter();
        Player.IsJumpQueued = false;
        _velocity.Y = _jumpVelocity;
        
        if(!InputManager.IsActionPressed(InputManager.Action.Jump))
            _velocity.Y /= 2;
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        HandleHorizontalMovement(delta);
        
        _velocity.Y += (float) (_jumpGravity * delta);
        
        if((InputManager.InputEnabled || false) && Input.IsActionJustReleased(InputManager.Action.Jump.ToInputMapName()))
            _velocity.Y /= Stats.JumpCutMultiplier;
        
        Player.Velocity = new Vector2(Player.Velocity.X, _velocity.Y);
        
        if(Player.IsOnCeiling())
            Player.Velocity = new Vector2(Player.Velocity.X, 0.1f);
        
        if(Player.Velocity.Y > 0)
            StateMachine.ChangeState<FallState>();
    }
}