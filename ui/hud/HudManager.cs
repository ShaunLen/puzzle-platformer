using Godot;

namespace PuzzlePlatformer.ui.hud;

public partial class HudManager : Node
{
    [Signal] public delegate void NotificationSentEventHandler(string message, bool isError);
    public static HudManager Instance { get; private set; }
    public override void _Ready() => Instance = this;
    public void SendNotification(string message, bool isError = false) => EmitSignal(SignalName.NotificationSent, message, isError);
}