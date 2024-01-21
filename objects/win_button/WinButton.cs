using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;

namespace PuzzlePlatformer.objects.win_button;

public partial class WinButton : Node2D
{
	[Signal] public delegate void PressedEventHandler();
	
	private AnimationPlayer _animationPlayer;
	private HitboxComponent _hitbox;
	private bool _playerNearby;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_hitbox = GetNode<HitboxComponent>("HitboxComponent");

		_hitbox.PlayerEntered += () =>
		{
			_animationPlayer.Play("glass_slide");
			_playerNearby = true;
		};

		_hitbox.PlayerExited += () =>
		{
			_animationPlayer.PlayBackwards("glass_slide");
			_playerNearby = false;
		};
	}

	public override void _Process(double delta)
	{
		if (_playerNearby && InputManager.IsActionJustPressed(InputManager.Action.Interact))
			EmitSignal(SignalName.Pressed);
	}
}
