using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class WalkState : GroundedState
{
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        
        GD.Print("IN WALK STATE PHYSICS PROCESS");

        HandleHorizontalMovement(delta);
        
        if(Player.Velocity.X == 0)
            StateMachine.ChangeState<IdleState>();

        if (InputManager.GetWalkAxis() == 0)
            return;
        
        StateMachine.SetAnimation(InputManager.GetWalkAxis() < 0 ? "walk_left" : "walk_right");
    }
}