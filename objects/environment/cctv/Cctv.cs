using Godot;
using PuzzlePlatformer.components;
using PuzzlePlatformer.litescript_two;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.environment.cctv;

public partial class Cctv : Node
{
	[Export] private HitboxComponent _hitboxComponent;
	[Export] private Node2D _lookAtOverride;
	[Export] private RigidBody2D _rigidBody;
	private Vector2 _lookatPos;

	// public override void _Ready()
	// {
	// 	_hitboxComponent.PlayerEntered += () => CallDeferred(nameof(DestroyCamera));
	// }

	public override void _Process(double delta)
	{
		_lookatPos = _lookAtOverride?.Position ?? LevelManager.Instance.PlayerPosition;
		_rigidBody.LookAt(_lookatPos);
	}

	// private void DestroyCamera()
	// {
	// 	GD.Print("Player entered CCTV");
	// 	_rigidBody.Freeze = false;
	// }
}
