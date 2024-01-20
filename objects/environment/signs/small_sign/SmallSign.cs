using Godot;

namespace PuzzlePlatformer.objects.environment.signs.small_sign;

public partial class SmallSign : RootSign
{
	public enum SignAnimation
	{
		Idle,
		Arrow
	}

	[Export] private SignAnimation _animation;
	
	public override void _Process(double delta)
	{
		if (AnimationPlayer.IsPlaying()) return;
		if (GD.RandRange(0, 1000) > 1) return;
		
		AnimationPlayer.Play(_animation.ToAnimationName());
	}
}

internal static class SignExtensions
{
	public static string ToAnimationName(this SmallSign.SignAnimation animation)
	{
		return animation switch
		{
			SmallSign.SignAnimation.Arrow => "arrow",
			_ => "idle"
		};
	}
}
