using System.Linq;
using Godot;

namespace PuzzlePlatformer.autoloads;

public partial class InputManager : Node
{
    public enum Action
    {
        MoveLeft,
        MoveRight,
        Jump,
        Dash,
        ToggleFullscreen,
        Interact,
        Close
    }

    private static bool _inputEnabled = true;

    public static void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
    }
    
    public static bool IsActionPressed(Action action, bool bypassInputEnabled = false) => (_inputEnabled || bypassInputEnabled) && Input.IsActionPressed(action.ToInputMapName());

    public static bool IsActionJustPressed(Action action, bool bypassInputEnabled = false) => (_inputEnabled || bypassInputEnabled) && Input.IsActionJustPressed(action.ToInputMapName());
    
    public static bool IsActionJustReleased(Action action, bool bypassInputEnabled = false) => (_inputEnabled || bypassInputEnabled) && Input.IsActionJustReleased(action.ToInputMapName());
   
    public static bool IsAnyActionPressed(bool bypassInputEnabled = false, params Action[] actions)
    {
        return actions.Any(action => IsActionPressed(action) && (_inputEnabled || bypassInputEnabled));
    }
    
    public static bool IsAnyActionJustPressed(bool bypassInputEnabled = false, params Action[] actions)
    {
        return actions.Any(action => IsActionJustPressed(action) && (_inputEnabled || bypassInputEnabled));
    }

    public static float GetAxis(Action negativeAction, Action positiveAction, bool bypassInputEnabled = false)
    {
        if (_inputEnabled || bypassInputEnabled)
            return Input.GetAxis(negativeAction.ToInputMapName(), positiveAction.ToInputMapName());

        return 0;
    }
    
    public static float GetWalkAxis(bool bypassInputEnabled = false)
    {
        if (_inputEnabled || bypassInputEnabled)
            return Input.GetAxis(Action.MoveLeft.ToInputMapName(), Action.MoveRight.ToInputMapName());

        return 0;
    }

    public override void _Process(double delta)
    {
        if(IsActionJustPressed(Action.ToggleFullscreen))
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
        }
    }
}

internal static class Extensions
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
            InputManager.Action.Close => "close",
            _ => ""
        };
    }
}