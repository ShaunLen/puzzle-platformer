using Godot;

namespace PuzzlePlatformer.ui.hud;

public partial class Subtitle : Label
{
	private float _showTime;
	private double _voiceDuration;
	
	public Subtitle(string text, double voiceDuration, float showTime)
	{
		Text = text;
		HorizontalAlignment = HorizontalAlignment.Center;
		VerticalAlignment = VerticalAlignment.Center;
		VisibleCharacters = 0;
		Theme = ResourceLoader.Load("res://ui/themes/hud_theme.tres") as Theme;
		_showTime = showTime;
		_voiceDuration = voiceDuration;
	}
	
	public override void _Ready()
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "visible_characters", Text.Length, _voiceDuration);
		tween.TweenInterval(_showTime);
		tween.TweenProperty(this, "visible_characters", 0, 0.5);
		tween.Finished += QueueFree;
	}
}
