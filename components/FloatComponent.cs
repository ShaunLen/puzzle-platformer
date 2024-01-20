using Godot;

namespace PuzzlePlatformer.components;

[GlobalClass]
public partial class FloatComponent : Node2D
{
	[Export] private float _amplitude = 1f;
	[Export] private float _frequency = 1f;
	private Vector2 _defaultPosition;
	private float _time;


	public override void _Ready() => _defaultPosition = ((Control)GetParent()).Position;

	public override void _Process(double delta)
	{
		_time += (float)delta * _frequency;
		((Control)GetParent()).Position = _defaultPosition + new Vector2(0, Mathf.Sin(_time) * _amplitude);
	}
}
