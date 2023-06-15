using Godot;
using System;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.common;
using PuzzlePlatformer.entities.player.states;

public partial class WalkState : GroundedState
{
    public override State PhysicsProcess(double delta)
    {
        HandleHorizontalMovement(delta);
        
        if(Player.Velocity.X == 0)
            return StateMachine.GetState(typeof(IdleState));

        return null;
    }
}
