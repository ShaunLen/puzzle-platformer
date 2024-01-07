using Godot;

namespace PuzzlePlatformer.objects.boxes.heavy_box;

public partial class HeavyBox : RigidBody2D
{
	private Sprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		var currentProgress = (float) (_sprite.Material as ShaderMaterial)?.GetShaderParameter("progress");
		(_sprite.Material as ShaderMaterial)?.SetShaderParameter("progress", currentProgress-(0.3*delta));
	}
}
