using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class GroundedState : BaseState
{
    public override State PhysicsProcess(double delta)
    {
        if (!Player.IsOnFloor()) 
            return StateMachine.GetState<FallState>();
        
        if (InputManager.IsActionJustPressed(InputManager.Action.Jump)) 
            return StateMachine.GetState<JumpState>();
        
        return null;
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