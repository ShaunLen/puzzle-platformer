using Godot;
using PuzzlePlatformer.components;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.environment.cctv;

public partial class Cctv : Node
{
	// [Export] private HitboxComponent _hitboxComponent;
	[Export] private RigidBody2D _rigidBody;

	// public override void _Ready()
	// {
		// _hitboxComponent.PlayerEntered += () => CallDeferred(nameof(DestroyCamera));
	// }

	public override void _Process(double delta)
	{
		_rigidBody.LookAt(LevelManager.Instance.PlayerPosition);
	}

	// private void DestroyCamera()
	// {
	// 	GD.Print("Player entered");
	// 	_rigidBody.Freeze = false;
	// }
}
