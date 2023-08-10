using Godot;

namespace PuzzlePlatformer.autoloads;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    /* Signals */
    [Signal] public delegate void PauseToggledEventHandler(bool isPaused);

    /* Backing Fields */
    private bool _gamePaused;
    
    /* Properties */
    public bool TerminalOpen { get; set; }

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
        if (InputManager.IsActionJustPressed(InputManager.Action.Escape, true))
            GamePaused = !GamePaused;
    }
    
    
    /* Public Methods */
    public Vector2 GetRespawnPosition()
    {
        return GetTree().GetFirstNodeInGroup("Level").GetNode<Node2D>("RespawnPos").Position;
    }
}