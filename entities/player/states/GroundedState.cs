using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class GroundedState : BaseState
{
    protected virtual void HandleHorizontalMovement(double delta)
    {
        if (InputManager.GetWalkAxis() != 0)
            Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, InputManager.GetWalkAxis() * Stats.WalkSpeed, (Stats.WalkAcceleration * 10) * (float) delta),
                Player.Velocity.Y);
        else
            Player.Velocity = new Vector2(Mathf.MoveToward(Player.Velocity.X, 0, (Stats.WalkDeceleration * 10) * (float) delta), Player.Velocity.Y);
    }
}