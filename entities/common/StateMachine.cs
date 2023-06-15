using System;
using System.Linq;
using Godot;

namespace PuzzlePlatformer.entities.common;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export] public bool EnableDebugLogs;
    [Export] private State[] _states;
    [Export] private State _startingState;
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

    public void Init()
    {
        foreach (var state in GetChildren().OfType<State>())
            state.StateMachine = this;
        
        ChangeState(_startingState);
    }
    
    private void ChangeState(State newState)
    {
        CurrentState?.Exit();
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState.Enter();
    }
    
    public void Process(double delta)
    {
        State newState = CurrentState.Process(delta);
        if(newState is not null)
            ChangeState(newState);
    }

    public void PhysicsProcess(double delta)
    {
        State newState = CurrentState.PhysicsProcess(delta);
        if(newState is not null)
            ChangeState(newState);
    }

    public State GetState<TState>()
    {
        return _states.FirstOrDefault(s => s is TState);
    }

    public void ChangeState<TState>()
    {
        var newState = _states.FirstOrDefault(s => s is TState);
        if(newState is not null) 
            ChangeState(newState);
    }
}