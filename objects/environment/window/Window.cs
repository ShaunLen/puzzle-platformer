using Godot;

namespace PuzzlePlatformer.objects.environment.window;

public partial class Window : Node
{
	private bool _walkedLeftLast = true;
	private AnimationPlayer _animationPlayer;

	public override void _Ready() => _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

	public override void _Process(double delta)
	{
		if (_animationPlayer.IsPlaying()) return;
		if (GD.RandRange(0, 1000) > 1) return;
		
		_animationPlayer.Play(_walkedLeftLast ? "walk_right" : "walk_left");
		_walkedLeftLast = !_walkedLeftLast;
	}
}
