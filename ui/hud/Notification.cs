using Godot;

namespace PuzzlePlatformer.ui.hud;

public partial class Notification : Label
{
    public Notification(string message)
    {
        Text = message;
        Theme = ResourceLoader.Load("res://ui/themes/hud_theme.tres") as Theme;
    }
    
    public override void _Ready()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, 5);
        tween.Finished += QueueFree;
    }
}