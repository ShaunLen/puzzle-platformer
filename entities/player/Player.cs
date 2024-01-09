using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.entities.common;
using PuzzlePlatformer.resources;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class Player : CharacterBody2D
{
	/* Signals */
	[Signal] public delegate void StatsUpdatedEventHandler();
	
	/* Exports */
	[Export] public PlayerStats Stats;
	[Export] private float _respawnDelay = 0.5f;
	
	/* Fields */
	private StateMachine _stateMachine;
	private RemoteTransform2D _remoteTransform;
	private HealthComponent _healthComponent;
	private AnimationPlayer _animationPlayer;
	
	/* Properties */
	public Timer JumpBufferTimer { get; private set; }
	public bool IsJumpQueued { get; set; }
	public bool IsFacingLeft { get; set; }

	/* Override Methods */
	
	public override void _Ready()
	{
		StoreNodes();
		
		/* Emit Signals */
		EmitSignal(SignalName.StatsUpdated);
		
		/* Initialise State Machine */
		_stateMachine = GetNode<StateMachine>("StateMachine");
		_stateMachine.Init();
		
		/* Signal Connections */
		_healthComponent.Died += HandleDeath;
		_stateMachine.ChangeAnimation += ChangeAnimation;
		
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
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
		Position = LevelManager.Instance.CurrentLevel.GetRespawnPosition();
		
		var t = GetTree().CreateTimer(_respawnDelay);
		t.Timeout += () =>
		{
			Visible = true;
			InputManager.InputEnabled = true;
		};
	}

	private void ChangeAnimation(string animationName)
	{
		GD.Print(animationName);

		switch (animationName)
		{
			case "walk_left" or "walk_right":
				IsFacingLeft = animationName == "walk_left";
				break;
			case "idle":
				_animationPlayer.Play(IsFacingLeft ? "idle_left" : "idle_right");
				return;
		}

		_animationPlayer.Play(animationName);
	}
}
