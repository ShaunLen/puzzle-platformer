using Godot;
using static PuzzlePlatformer.autoloads.AudioManager.Sound;
using static PuzzlePlatformer.autoloads.InputManager.Action;

namespace PuzzlePlatformer.autoloads;

public partial class GameManager : Node
{
    [Signal] public delegate void PauseToggledEventHandler(bool isPaused);
    public static GameManager Instance { get; private set; }
    public int LevelRestarts;
    
    private bool _gamePaused;

    public bool GamePaused
    {
        get => _gamePaused;
        set
        {
            _gamePaused = value;
            GetTree().Paused = _gamePaused;
            Input.MouseMode = _gamePaused ? Input.MouseModeEnum.Confined : Input.MouseModeEnum.ConfinedHidden;
            Input.WarpMouse(GetTree().Root.Size / 2);
            EmitSignal(SignalName.PauseToggled, value);
        }
    }

    /* Override Methods */
    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (InputManager.IsActionJustPressed(Escape, true))
            GamePaused = !GamePaused;

        if (InputManager.IsActionJustPressed(RestartLevel) && !_gamePaused)
        {
            LevelRestarts++;
            GetTree().ReloadCurrentScene();
            GD.Print(LevelRestarts);
        }
    }
}