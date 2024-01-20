using Godot;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.ui.hud;

public partial class Hud : Control
{
	private VBoxContainer _notificationContainer;
	private VBoxContainer _subtitleContainer;
	private Timer _subtitleTimer;
		
	public override void _Ready()
	{
		if(!LevelManager.Instance.CodelessLevel)
			HudManager.Instance.NotificationSent += WriteNotification;
		
		HudManager.Instance.ShowSubtitle += ShowSubtitle;

		_notificationContainer = GetNode<VBoxContainer>("NotificationContainer");
		_subtitleContainer = GetNode<VBoxContainer>("SubtitleContainer");
		_subtitleTimer = GetNode<Timer>("SubtitleTimer");
	}

	private void WriteNotification(string message, bool isError)
	{
		var notification = new Notification(message);
		_notificationContainer.AddChild(notification);
	}

	private void ShowSubtitle(string text, double voiceDuration, float showTime = 3)
	{
		var subtitle = new Subtitle(text, voiceDuration, showTime);
		_subtitleContainer.AddChild(subtitle);
	}
}
