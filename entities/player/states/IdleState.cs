using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class IdleState : GroundedState
{
    public override void Enter()
    {
        base.Enter();
        Player.Velocity = Vector2.Zero;
        StateMachine.SetAnimation("idle");
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        
        if (InputManager.GetAxis(InputManager.Action.MoveLeft, InputManager.Action.MoveRight) != 0)
            StateMachine.ChangeState<WalkState>();
    }
}