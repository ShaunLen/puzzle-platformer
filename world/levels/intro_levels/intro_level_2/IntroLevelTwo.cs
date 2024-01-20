using Godot;
using PuzzlePlatformer.components;
using PuzzlePlatformer.ui.key_hint;

namespace PuzzlePlatformer.world.levels.intro_levels.intro_level_2;

public partial class IntroLevelTwo : LevelRoot
{
	[Export] private Node2D _boxSpawnPos;
	[Export] private Timer _boxSpawnDelayTimer;
	[Export] private KeyHint _keyHintRestart;
	[Export] private HitboxComponent _restartHintTrigger;

	public override void _Ready()
	{
		base._Ready();

		_restartHintTrigger.PlayerEntered += () =>
		{
			TweenKeyHint(_keyHintRestart, 1);
			_restartHintTrigger.QueueFree();;
		};
		
		SpawnBox();
		_boxSpawnDelayTimer.Timeout += SpawnBox;
		return;

		void SpawnBox()
		{
			var heavyBoxScene = GD.Load<PackedScene>("res://objects/boxes/heavy_box/heavy_box.tscn");
			var heavyBoxInstance = heavyBoxScene.Instantiate();
			AddChild(heavyBoxInstance);
			((Node2D)heavyBoxInstance).Position = _boxSpawnPos.Position;
			_boxSpawnDelayTimer.Start();
		}
	}

	private void TweenKeyHint(KeyHint keyHint, int finalValue) => CreateTween().TweenProperty(keyHint, "modulate:a", finalValue, 0.5);

	// UNUSED
	protected override void DefineRequirements()
	{
	}
}
