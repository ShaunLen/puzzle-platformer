using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.ui.key_hint;

namespace PuzzlePlatformer.ui.menus.main_menu;

public partial class MainMenu : Node2D
{
	[Export] private MarginContainer _titleContainer;
	[Export] private Player _player;
	[Export] private KeyHint _keyHintWalk;
	private Timer _timer;
	private bool _introFinished;
	private bool _animating = true;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		_timer = GetNode<Timer>("Timer");
		InputManager.InputEnabled = false;
		_timer.Timeout += () =>
		{
			_introFinished = true;
			InputManager.InputEnabled = true;
		};
		
		CreateTween().TweenProperty(_titleContainer, "position:y", 0, 2);
		CreateTween().TweenProperty(_player, "position:x", 96, 2);
	}

	public override void _Process(double delta)
	{
		if (_animating)
		{
			if(!_introFinished)
				_player.GetNode<AnimationPlayer>("AnimationPlayer").Play("walk_right");
			else
			{
				_player.GetNode<AnimationPlayer>("AnimationPlayer").Play("idle_right");
				CreateTween().TweenProperty(_keyHintWalk, "modulate:a", 1, 0.5);
				_animating = false;
			}

			return;
		}
		
		if(InputManager.GetWalkAxis() != 0)
			CreateTween().TweenProperty(_keyHintWalk, "modulate:a", 0, 0.5);
	}
}
