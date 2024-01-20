using System.Linq;
using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.entities.common;

[GlobalClass]
public partial class StateMachine : Node
{
    [Signal] public delegate void ChangeAnimationEventHandler(string animationName);
    [Signal] public delegate void PlaySoundEventHandler(AudioManager.Sound sound);
    [Signal] public delegate void StopSoundEventHandler();
    
    [ExportGroup("Debug")]
    [Export] public bool EnterStateLog;
    [Export] public bool ExitStateLog;
    
    [ExportGroup("States")]
    [Export] private State[] _states;
    [Export] private State _startingState;
    public State CurrentState { get; private set; }
    private State _previousState;

    public void Init()
    {
        foreach (var state in GetChildren().OfType<State>())
            state.StateMachine = this;
        
        ChangeState(_startingState);
    }
    
    private void ChangeState(State newState)
    {
        if (newState is null || !newState.Enabled)
        {
            GD.Print(this.Name + " attempted to transition to non-existent or disabled state.");
            return;
        }
            
        CurrentState?.Exit();
        _previousState = CurrentState;
        CurrentState = newState;
        CurrentState.Enter();
    }
    
    public void Process(double delta)
    {
        CurrentState.Process(delta);
    }

    public void PhysicsProcess(double delta)
    {
        CurrentState.PhysicsProcess(delta);
    }

    public void ChangeState<TState>()
    {
        var newState = _states.FirstOrDefault(s => s is TState);
        if(newState is not null) 
            ChangeState(newState);
    }
    
    public State GetState<TState>()
    {
        return _states.FirstOrDefault(s => s is TState);
    }

    public bool IsPreviousState<TState>()
    {
        return _previousState.GetType() == typeof(TState) || _previousState.GetType().IsSubclassOf(typeof(TState));
    }

    public void SetAnimation(string animationName) => EmitSignal(SignalName.ChangeAnimation, animationName);
    public void PlayAudio(AudioManager.Sound sound) => EmitSignal(SignalName.PlaySound, (int) sound);
    public void StopAudio()
    {
        EmitSignal(SignalName.StopSound);
    }
}