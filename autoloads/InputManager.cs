using System;
using System.Linq;
using Godot;

namespace PuzzlePlatformer.autoloads;

public partial class InputManager : Node
{
    /* Enums */
    public enum Action
    {
        MoveLeft,
        MoveRight,
        Jump,
        Dash,
        ToggleFullscreen,
        Interact,
        Escape,
        ToggleCode,
        RunCode,
        ClearConsole,
        ZoomIn,
        ZoomOut,
        HighlightInteractables
    }
    
    /* Properties */
    public static bool InputEnabled { get; set; } = true;

    /* Override Methods */
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if(IsActionJustPressed(Action.ToggleFullscreen, true))
            DisplayServer.WindowSetMode(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
    }

    /* Methods */
    public static bool IsActionPressed(Action action, bool bypassInputEnabled = false) => (InputEnabled || bypassInputEnabled) && Input.IsActionPressed(action.ToInputMapName());

    public static bool IsActionJustPressed(Action action, bool bypassInputEnabled = false) => (InputEnabled || bypassInputEnabled) && Input.IsActionJustPressed(action.ToInputMapName());

    public static bool IsAnyActionPressed(bool bypassInputEnabled = false, params Action[] actions)
    {
        return actions.Any(action => IsActionPressed(action) && (InputEnabled || bypassInputEnabled));
    }
    
    public static bool IsAnyActionJustPressed(bool bypassInputEnabled = false, params Action[] actions)
    {
        return actions.Any(action => IsActionJustPressed(action) && (InputEnabled || bypassInputEnabled));
    }

    public static float GetAxis(Action negativeAction, Action positiveAction, bool bypassInputEnabled = false)
    {
        if (InputEnabled || bypassInputEnabled)
            return Input.GetAxis(negativeAction.ToInputMapName(), positiveAction.ToInputMapName());

        return 0;
    }
    
    public static float GetWalkAxis(bool bypassInputEnabled = false)
    {
        if (InputEnabled || bypassInputEnabled)
            return Input.GetAxis(Action.MoveLeft.ToInputMapName(), Action.MoveRight.ToInputMapName());

        return 0;
    }
}

internal static class InputExtensions
{
    public static string ToInputMapName(this InputManager.Action action)
    {
        return action switch
        {
            InputManager.Action.MoveLeft => "move_left",
            InputManager.Action.MoveRight => "move_right",
            InputManager.Action.Jump => "jump",
            InputManager.Action.Dash => "dash",
            InputManager.Action.ToggleFullscreen => "toggle_fullscreen",
            InputManager.Action.Interact => "interact",
            InputManager.Action.Escape => "escape",
            InputManager.Action.ToggleCode => "toggle_code",
            InputManager.Action.RunCode => "run_code",
            InputManager.Action.ClearConsole => "clear_console",
            InputManager.Action.ZoomIn => "zoom_in",
            InputManager.Action.ZoomOut => "zoom_out",
            InputManager.Action.HighlightInteractables => "highlight_interactables",
            _ => ""
        };
    }
}