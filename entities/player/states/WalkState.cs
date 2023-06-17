namespace PuzzlePlatformer.entities.player.states;

public partial class WalkState : GroundedState
{
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        HandleHorizontalMovement(delta);
        
        if(Player.Velocity.X == 0)
            StateMachine.ChangeState<IdleState>();
    }
}