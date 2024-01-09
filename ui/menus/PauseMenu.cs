using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.ui.menus;

public partial class PauseMenu : Control
{
	private Button _resumeButton, _optionsButton, _quitButton;
	
	public override void _Ready()
	{
		_resumeButton = GetNode<Button>("MenuContainer/ResumeButton");
		_optionsButton = GetNode<Button>("MenuContainer/OptionsButton");
		_quitButton = GetNode<Button>("MenuContainer/QuitButton");

		Visible = false;
		GameManager.Instance.PauseToggled += OnPauseToggled;
		_resumeButton.Pressed += () => GameManager.Instance.GamePaused = false;
		_quitButton.Pressed += () => GetTree().Quit();
	}

	public override void _ExitTree()
	{
		GameManager.Instance.PauseToggled -= OnPauseToggled;
	}

	private void OnPauseToggled(bool paused)
	{
		Visible = paused;
	}
}
