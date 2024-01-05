using Godot;
using PuzzlePlatformer.components;

namespace PuzzlePlatformer.objects.win_button;

public partial class WinButton : Node2D
{
	private AnimationPlayer _animationPlayer;
	private HitboxComponent _hitbox;
	private bool _playerNearby = false;

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
}
