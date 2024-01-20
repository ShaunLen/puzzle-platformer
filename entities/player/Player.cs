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

	[ExportCategory("Components")] 
	[Export] private AudioStreamPlayer2D _footstepsAudioPlayer;
	[Export] private AudioStreamPlayer2D _otherAudioPlayer;
	[Export] private GpuParticles2D _dustParticles;
	[Export] private GpuParticles2D _dustParticlesLeft;
	[Export] private GpuParticles2D _dustParticlesRight;
	
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
		
		/* Initialise Components */
		_healthComponent.Init(Stats.Health);
		
		/* Signal Connections */
		_healthComponent.Died += HandleDeath;
		_stateMachine.ChangeAnimation += ChangeAnimation;
		_stateMachine.PlaySound += PlaySound;
		_stateMachine.StopSound += StopSound;
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
	
	/* Public Methods */

	public void EmitDustEffect(bool landing = false)
	{
		_dustParticles.Restart();

		if (landing)
		{
			_dustParticles.Lifetime = 0.35;
			_dustParticles.Amount = 40;
			_dustParticles.Modulate = new Color(1, 1, 1, 0.5f);
		}
		else
		{
			_dustParticles.Lifetime = 0.5;
			_dustParticles.Amount = 100;
			_dustParticles.Modulate = new Color(1, 1, 1, 1);
		}
		
		_dustParticles.Emitting = true;
	}

	public void EmitDustEffectRight()
	{
		_dustParticlesRight.Restart();;
		_dustParticlesRight.Emitting = true;
	}

	public void EmitDustEffectLeft()
	{
		_dustParticlesLeft.Restart();;
		_dustParticlesLeft.Emitting = true;
	}
	
	/* Private Methods */
	private void StoreNodes()
	{
		JumpBufferTimer = GetNode<Timer>("JumpBufferTimer");
		_healthComponent = GetNode<HealthComponent>("HealthComponent");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_remoteTransform = GetNode<RemoteTransform2D>("PlayerTransform");
		
		if(!GameManager.Instance.InMenu)
			_remoteTransform.RemotePath = GetTree().GetFirstNodeInGroup("Camera").GetPath();
	}
	
	private void OnJumpBufferTimerTimeout()
	{
		IsJumpQueued = false;
	}

	private void HandleDeath()
	{
		AudioManager.Instance.PlaySound(AudioManager.Sound.Sizzle, _otherAudioPlayer);
		
		var timer = GetTree().CreateTimer(_respawnDelay);
		timer.Timeout += () => GameManager.Instance.ReloadScene();
	}

	private void ChangeAnimation(string animationName)
	{
		switch (animationName)
		{
			case "walk_left" or "walk_right":
				IsFacingLeft = animationName == "walk_left";
				break;
			case "idle_left" or "idle_right":
				IsFacingLeft = animationName == "idle_left";
				break;
			case "idle":
				_animationPlayer.Play(IsFacingLeft ? "idle_left" : "idle_right");
				return;
		}

		_animationPlayer.Play(animationName);
	}

	private void PlaySound(AudioManager.Sound sound)
	{
		var audioPlayer = sound == AudioManager.Sound.Footsteps ? _footstepsAudioPlayer : _otherAudioPlayer;
		
		AudioManager.Instance.StopSound(audioPlayer);
		AudioManager.Instance.PlaySound(sound, audioPlayer);
	}

	private void StopSound()
	{
		AudioManager.Instance.StopSound(_footstepsAudioPlayer);
		AudioManager.Instance.StopSound(_otherAudioPlayer);
	}
}
