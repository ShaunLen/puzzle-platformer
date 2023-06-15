using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class IdleState : GroundedState
{
    public override void Enter()
    {
        base.Enter();
        Player.Velocity = Vector2.Zero;
    }

    public override State PhysicsProcess(double delta)
    {
        if (InputManager.GetAxis(InputManager.Action.MoveLeft, InputManager.Action.MoveRight) != 0)
            return StateMachine.GetState(typeof(WalkState));

        return null;
    }
}