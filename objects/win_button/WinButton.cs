using Godot;

namespace PuzzlePlatformer.objects.win_button;

public partial class WinButton : Node2D
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
}