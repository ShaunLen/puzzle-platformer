using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class FallState : InAirState
{
    [Export] private Timer _coyoteTimer;
    
    private Vector2 _velocity;
    private float _fallGravity;

    public override void _Ready()
    {
        base._Ready();
        
        /* Signal Connections */
        Player.StatsUpdated += OnPlayerStatsUpdated;
    }

    private void OnPlayerStatsUpdated()
    {
        _fallGravity = (-2 * Stats.JumpHeight) / Mathf.Pow(Stats.JumpTimeToGround, 2) * -1;
    }
    
    public override void Enter()
    {
        base.Enter();
        _coyoteTimer.Start();
        _velocity.Y = Player.Velocity.Y;
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        HandleHorizontalMovement(delta);
        
        if(InputManager.IsActionJustPressed(InputManager.Action.Jump))
        {
            if (CanJump())
                StateMachine.ChangeState<JumpState>();
            else
                QueueJump();
        }
            

        _velocity.Y += _fallGravity * (float) delta;
        _velocity.Y = Mathf.Min(_velocity.Y, Stats.MaxFallSpeed);

        Player.Velocity = new Vector2(Player.Velocity.X, _velocity.Y);

        if (Player.IsOnFloor())
            StateMachine.ChangeState<WalkState>();
    }

    private bool CanJump()
    {
        return !_coyoteTimer.IsStopped() && StateMachine.IsPreviousState<GroundedState>();
    }
}