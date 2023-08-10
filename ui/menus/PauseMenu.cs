using Godot;
using PuzzlePlatformer.autoloads;
using Utils = Microsoft.VisualBasic.CompilerServices.Utils;

namespace PuzzlePlatformer.ui.menus;

public partial class PauseMenu : Control
{
    private Button _resumeButton, _optionsButton, _quitButton;
    
    public override void _Ready()
    {
        _resumeButton = GetNode<Button>("MenuContainer/ResumeButton");
        _optionsButton = GetNode<Button>("MenuContainer/OptionsButton");
        _quitButton = GetNode<Button>("MenuContainer/QuitButton");
        
        Hide();
        GameManager.Instance.PauseToggled += OnPauseToggled;
        _resumeButton.Pressed += () => GameManager.Instance.GamePaused = false;
        _quitButton.Pressed += () => GetTree().Quit();
    }

    private void OnPauseToggled(bool paused)
    {
        Visible = paused;
    }
}