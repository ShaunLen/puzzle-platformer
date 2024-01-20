using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.ui.transitions.reload_level;

public partial class ReloadLevel : Control
{
	private bool _in = true;
	[Export] private ColorRect _colorRect;

	public override void _Ready()
	{
		if(GameManager.Instance.CanRestart)
			Show();
	}

	public override void _PhysicsProcess(double delta)
	{
		var res = (Vector2) (_colorRect.Material as ShaderMaterial)?.GetShaderParameter("res");

		(_colorRect.Material as ShaderMaterial)?.SetShaderParameter("res",
			_in ? new Vector2(res.X, res.Y += 6) : new Vector2(res.X, res.Y -= 6));

		switch (res.Y)
		{
			case > 40:
				_in = false;
				break;
			case < 10:
				QueueFree();
				break;
		}
	}
}
