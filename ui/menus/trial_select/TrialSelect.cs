using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.ui.menus.trial_select;

public partial class TrialSelect : Control
{
	[Export] private Button _sceneBtn;
	
	public override void _Ready()
	{
		_sceneBtn.Pressed += () => GameManager.Instance.ChangeScene(GameManager.Scene.LevelOne);
	}
}
