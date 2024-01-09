using Godot;
using PuzzlePlatformer.audio.scripts;

namespace PuzzlePlatformer.ui.hud;

public partial class Hud : Control
{
	private VBoxContainer _notificationContainer;
	private Label _subtitleLabel;
	private Timer _subtitleTimer;
		
	public override void _Ready()
	{
		HudManager.Instance.NotificationSent += WriteNotification;
		VoiceManager.Instance.ShowSubtitle += ShowSubtitle;

		_notificationContainer = GetNode<VBoxContainer>("NotificationContainer");
		_subtitleLabel = GetNode<Label>("Subtitle");
		_subtitleTimer = GetNode<Timer>("SubtitleTimer");
	}

	private void WriteNotification(string message, bool isError)
	{
		var notification = new Notification(message);
		_notificationContainer.AddChild(notification);
	}

	private void ShowSubtitle(string subtitle)
	{
		_subtitleLabel.Text = subtitle;
		_subtitleTimer.Timeout += RemoveSubtitle;
		_subtitleTimer.Start();
		return;

		void RemoveSubtitle()
		{
			_subtitleLabel.Text = "";
			_subtitleTimer.Timeout -= RemoveSubtitle;
		}
	}
}
