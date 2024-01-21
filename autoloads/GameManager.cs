using Godot;
using static PuzzlePlatformer.autoloads.InputManager.Action;

namespace PuzzlePlatformer.autoloads;

public partial class GameManager : Node
{
    [Signal] public delegate void SceneRestartedEventHandler();
    [Signal] public delegate void PauseToggledEventHandler(bool isPaused);
    public Scene CurrentScene { get; set; }
    public string PlayerWrittenCode { get; set; }

    public enum Scene
    {
        MainMenu,
        IntroLevelOne,
        IntroLevelTwo,
        LevelOne,
        ExitGame
    }
    
    public static GameManager Instance { get; private set; }
    public int LevelRestarts;
    public bool InIntro;
    public bool CanRestart;
    private bool _inMenu;
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

    public bool InMenu
    {
        get => _inMenu;
        set
        {
            _inMenu = value;
            Input.MouseMode = _inMenu ? Input.MouseModeEnum.Confined : Input.MouseModeEnum.ConfinedHidden;
            Input.WarpMouse(GetTree().Root.Size / 2);
        }
    }

    /* Override Methods */
    public override void _Ready()
    {
        InMenu = true;
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (InMenu) return;
        
        if (InputManager.IsActionJustPressed(Escape, true))
            GamePaused = !GamePaused;

        if (InputManager.IsActionJustPressed(RestartLevel) && CanRestart && !_gamePaused)
            ReloadScene();
    }
    
    /* Public Methods */
    public void ChangeScene(Scene scene)
    {
        if (scene == Scene.ExitGame) GetTree().Quit();

        CurrentScene = scene;
        PlayerWrittenCode = "";
        
        var packedScene = GD.Load<PackedScene>(scene.ToPath());
        GetTree().ChangeSceneToPacked(packedScene);
        InMenu = scene == Scene.MainMenu;
        InIntro = scene < Scene.LevelOne;
        CanRestart = scene > Scene.IntroLevelOne;
        LevelRestarts = 0;
    }

    public void ReloadScene()
    {
        LevelRestarts++;
        GetTree().ReloadCurrentScene();
        EmitSignal(SignalName.SceneRestarted);
    }
}

internal static class GameManagerExtensions
{
    public static string ToPath(this GameManager.Scene scene)
    {
        return scene switch
        {
            GameManager.Scene.MainMenu => "res://ui/menus/main_menu/main_menu.tscn",
            GameManager.Scene.IntroLevelOne => "res://world/levels/intro_levels/intro_level_1/intro_level_1.tscn",
            GameManager.Scene.IntroLevelTwo => "res://world/levels/intro_levels/intro_level_2/intro_level_2.tscn",
            GameManager.Scene.LevelOne => "res://world/levels/level_1/level_one.tscn",
            _ => ""
        };
    }
}