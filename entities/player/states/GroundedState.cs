using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class GroundedState : BaseState
{
    public override void Enter()
    {
        base.Enter();
        if(Player.IsJumpQueued)
            StateMachine.ChangeState<JumpState>();
    }

    public override void PhysicsProcess(double delta)
    {
        if (!Player.IsOnFloor()) 
            StateMachine.ChangeState<FallState>();
        
        if (InputManager.IsActionJustPressed(InputManager.Action.Jump)) 
            StateMachine.ChangeState<JumpState>();
    }

    protected virtual void HandleHorizontalMovement(double delta)
    {
        if (InputManager.GetWalkAxis() != 0)
            Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, InputManager.GetWalkAxis() * Stats.WalkSpeed, (Stats.WalkAcceleration * 10) * (float) delta),
                Player.Velocity.Y);
        else
            Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, 0, (Stats.WalkDeceleration * 10) * (float) delta), Player.Velocity.Y);
    }
}