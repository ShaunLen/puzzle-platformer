using Godot;

namespace PuzzlePlatformer.entities.common;

public partial class State : Node
{
    public StateMachine StateMachine;

    public virtual void Enter()
    {
        if(StateMachine.EnableDebugLogs) GD.Print("Entered " + this.Name + " state.");
    }

    public virtual void Exit()
    {
        if(StateMachine.EnableDebugLogs) GD.Print("Exited " + this.Name + " state.");
    }

    public virtual State Process(double delta)
    {
        return null;
    }

    public virtual State PhysicsProcess(double delta)
    {
        return null;
    }
}