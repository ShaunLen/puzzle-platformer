using Godot;

namespace PuzzlePlatformer.entities.common;

public partial class State : Node
{
    [Export] public bool Enabled = true;
    public StateMachine StateMachine;

    public virtual void Enter()
    {
        if(StateMachine.EnterStateLog) GD.Print("Entered " + this.Name + " state.");
    }

    public virtual void Exit()
    {
        if(StateMachine.ExitStateLog) GD.Print("Exited " + this.Name + " state.");
    }

    public virtual void Process(double delta)
    {
    }

    public virtual void PhysicsProcess(double delta)
    {
    }
}