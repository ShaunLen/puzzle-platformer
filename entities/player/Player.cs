using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.entities.common;
using PuzzlePlatformer.resources;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    /* Signals */
    [Signal] public delegate void StatsUpdatedEventHandler();
    
    /* Exports */
    [Export] public PlayerStats Stats;
    [Export] private float _respawnDelay = 0.5f;
    
    /* Variables */
    private StateMachine _stateMachine;
    private RemoteTransform2D _remoteTransform;
    private HealthComponent _healthComponent;
    
    /* Properties */
    public Timer JumpBufferTimer { get; private set; }
    public bool IsJumpQueued { get; set; }

    /* Override Methods */
    
    public override void _Ready()
    {
        StoreNodes();
        
        /* Emit Signals */
        EmitSignal(SignalName.StatsUpdated);

        /* Signal Connections */
        _healthComponent.Died += HandleDeath;
        
        /* Initialise State Machine */
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _stateMachine.Init();
        
        /* Initialise Components */
        _healthComponent.Init(Stats.Health);
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
    
    /* Private Methods */
    private void StoreNodes()
    {
        JumpBufferTimer = GetNode<Timer>("JumpBufferTimer");
        _healthComponent = GetNode<HealthComponent>("HealthComponent");
        _remoteTransform = GetNode<RemoteTransform2D>("PlayerTransform");
        _remoteTransform.RemotePath = GetTree().GetFirstNodeInGroup("Camera").GetPath();
    }
    
    private void OnJumpBufferTimerTimeout()
    {
        IsJumpQueued = false;
    }

    private void HandleDeath()
    {
        Visible = false;
        InputManager.InputEnabled = false;
        Position = GameManager.Instance.GetRespawnPosition();
        
        var t = GetTree().CreateTimer(_respawnDelay);
        t.Timeout += () =>
        {
            Visible = true;
            InputManager.InputEnabled = true;
        };
    }
}