using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.boxes.heavy_box;

public partial class HeavyBox : RigidBody2D
{
	[Export] private HealthComponent _healthComponent;
	[Export] private AudioStreamPlayer2D _audioPlayer;
	private Sprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		
		if(LevelManager.Instance.CodelessLevel)
			(_sprite.Material as ShaderMaterial)?.SetShaderParameter("progress", 0);

		_healthComponent.Died += () =>
		{
			AudioManager.Instance.PlaySound(AudioManager.Sound.Sizzle, _audioPlayer);
			_audioPlayer.Finished += QueueFree;
		};
	}

	public override void _Process(double delta)
	{
		if (LevelManager.Instance.CodelessLevel) return;
		
		var currentProgress = (float) (_sprite.Material as ShaderMaterial)?.GetShaderParameter("progress");
		(_sprite.Material as ShaderMaterial)?.SetShaderParameter("progress", currentProgress-(0.3*delta));
	}
}
