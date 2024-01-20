using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class InAirState : BaseState
{
    protected virtual void HandleHorizontalMovement(double delta)
    {
        if (InputManager.GetWalkAxis() == 0)
        {
            Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, 0, (Stats.AirDeceleration * 10) * (float) delta), Player.Velocity.Y);
            return;
        }

        Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, InputManager.GetWalkAxis() * Stats.AirSpeed, (Stats.AirAcceleration * 10) * (float) delta),
            Player.Velocity.Y);
        
        StateMachine.SetAnimation(InputManager.GetWalkAxis() < 0 ? "idle_left" : "idle_right");
    }

    protected void QueueJump()
    {
        Player.IsJumpQueued = true;
        Player.JumpBufferTimer.Start();
    }
}