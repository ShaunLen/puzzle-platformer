using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.ui.hud;
using PuzzlePlatformer.ui.key_hint;

namespace PuzzlePlatformer.world.levels.intro_levels.intro_level_1;

public partial class IntroLevelOne : LevelRoot
{
	[Export] private ColorRect IntroContainer;
	[Export] private Label IntroText;
	[Export] private KeyHint _keyHintWalk;
	[Export] private KeyHint _keyHintJump;
	[Export] private KeyHint _keyHintJumpHold;
	[Export] private HitboxComponent _jumpHintTrigger;
	[Export] private HitboxComponent _jumpHoldHintTrigger;
	
	public override void _Ready()
	{
		base._Ready();
		// IntroContainer.Show();

		// InputManager.InputEnabled = false;
		//
		// var textTween = CreateTween();
		// textTween.TweenProperty(IntroText, "visible_characters", 5, 0.3);
		// textTween.TweenProperty(IntroText, "visible_characters", 12, 0.4);
		// textTween.TweenProperty(IntroText, "visible_characters", 14, 0.7);
		// textTween.TweenProperty(IntroText, "visible_characters", 26, 0.6);
		// textTween.TweenProperty(IntroText, "visible_characters", 28, 0.4);
		// textTween.TweenProperty(IntroText, "visible_characters", 29, 0.4);
		// textTween.TweenProperty(IntroText, "visible_characters", 30, 0.7);
		// textTween.TweenProperty(IntroText, "visible_characters", 31, 0.6);
		// textTween.TweenProperty(IntroText, "visible_characters", 32, 0.4);
		// textTween.TweenProperty(IntroContainer, "modulate:a", 0, 2);
		// textTween.TweenProperty(IntroText, "modulate:a", 0, 1.5);
		//
		// textTween.Finished += () =>
		// {
		// 	InputManager.InputEnabled = true;
		// 	HudManager.Instance.AddSubtitle("Please proceed to the end of the corridor to begin the Calisthenics Assessment and Kinetic Evaluation.", 3);
		// };

		TweenKeyHint(_keyHintWalk, 1);
		
		_jumpHintTrigger.PlayerEntered += () =>
		{
			TweenKeyHint(_keyHintJump, 1);
			_jumpHintTrigger.QueueFree();
		};

		_jumpHoldHintTrigger.PlayerEntered += () =>
		{
			TweenKeyHint(_keyHintJumpHold, 1);
			_jumpHoldHintTrigger.QueueFree();
		};
	}

	public override void _Process(double delta)
	{
		if(InputManager.GetWalkAxis() != 0)
			TweenKeyHint(_keyHintWalk, 0);

		if (InputManager.IsActionJustPressed(InputManager.Action.Jump))
		{
			TweenKeyHint(_keyHintJump, 0);
			TweenKeyHint(_keyHintJumpHold, 0);
		}
	}

	private void TweenKeyHint(KeyHint keyHint, int finalValue) => CreateTween().TweenProperty(keyHint, "modulate:a", finalValue, 0.5);

	// UNUSED
	protected override void DefineRequirements()
	{
	}
}
