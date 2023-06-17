using System.Runtime.CompilerServices;
using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void StatsUpdatedEventHandler();
    
    [Export] public resources.PlayerStats Stats;
    public Timer JumpBufferTimer;
    public bool IsJumpQueued;
    private StateMachine _stateMachine;

    public override void _Ready()
    {
        EmitSignal(SignalName.StatsUpdated);
        JumpBufferTimer = GetNode<Timer>("JumpBufferTimer");
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _stateMachine.Init();
    }
    
    public override void _Process(double delta)
    {
        _stateMachine.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _stateMachine.PhysicsProcess(delta);
        MoveAndSlide();
    }
    
    private void OnJumpBufferTimerTimeout()
    {
        IsJumpQueued = false;
    }
}