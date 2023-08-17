using System;
using Godot;
using PuzzlePlatformer.ui.hud;

namespace PuzzlePlatformer.autoloads;

public partial class HudManager : Node
{
    public static HudManager Instance { get; private set; }

    private VBoxContainer _notificationContainer;

    /* Overrides */
    
    public override void _Ready()
    {
        Instance = this;
        _notificationContainer = GetTree().GetFirstNodeInGroup("HUD").GetNode<VBoxContainer>("NotificationContainer");
    }
    
    /* Public Methods */
    
    public void WriteNotification(string message)
    {
        var notification = new Notification(message);
        _notificationContainer.AddChild(notification);
    }

    public void WriteErrorNotification(string message)
    {
        var notification = new Notification(message);
        _notificationContainer.AddChild(notification);
    }
}