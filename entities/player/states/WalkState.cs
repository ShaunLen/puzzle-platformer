using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.player.states;

public partial class WalkState : GroundedState
{
    private bool _facingLeft;
    
    public override void Enter()
    {
        base.Enter();
        if(InputManager.GetWalkAxis() != 0)
            StateMachine.PlayAudio(AudioManager.Sound.Footsteps);
    }

    public override void Exit()
    {
        base.Exit();
        StateMachine.StopAudio();
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        HandleHorizontalMovement(delta);
        
        if(Player.Velocity.X == 0)
            StateMachine.ChangeState<IdleState>();

        if (InputManager.GetWalkAxis() == 0)
            return;

        switch (_facingLeft)
        {
            case true when InputManager.GetWalkAxis() > 0 && Player.Stats.WalkSpeed - Mathf.Abs(Player.Velocity.X) == 0:
                _facingLeft = false;
                Player.EmitDustEffectLeft();
                break;
            case false when InputManager.GetWalkAxis() < 0 && Player.Stats.WalkSpeed - Mathf.Abs(Player.Velocity.X) == 0:
                _facingLeft = true;
                Player.EmitDustEffectRight();
                break;
        }
        
        StateMachine.SetAnimation(InputManager.GetWalkAxis() < 0 ? "walk_left" : "walk_right");
    }
}