using Godot;

namespace PuzzlePlatformer.ui.hud;

public partial class Hud : Control
{
	private VBoxContainer _notificationContainer;
		
	public override void _Ready()
	{
		HudManager.Instance.NotificationSent += WriteNotification;

		_notificationContainer = GetNode<VBoxContainer>("NotificationContainer");
	}

	private void WriteNotification(string message, bool isError)
	{
		var notification = new Notification(message);
		_notificationContainer.AddChild(notification);
	}
}
