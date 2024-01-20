using Godot;

namespace PuzzlePlatformer.ui.hud;

public partial class HudManager : Node
{
    [Signal] public delegate void NotificationSentEventHandler(string message, bool isError);
    [Signal] public delegate void ShowSubtitleEventHandler(string text, double voiceDuration, float showTime);
    public static HudManager Instance { get; private set; }
    public override void _Ready() => Instance = this;
    public void SendNotification(string message, bool isError = false) => EmitSignal(SignalName.NotificationSent, $"| {message.ToUpper()}", isError);
    public void AddSubtitle(string text, double voiceDuration, float showTime = 3) => EmitSignal(SignalName.ShowSubtitle, text, voiceDuration, showTime);
}