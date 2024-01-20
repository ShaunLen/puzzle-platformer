using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.ui.splash;

public partial class Splash : Label
{
	[Export] private ColorRect _circle;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		
		var callable = new Callable(this, MethodName.SetCircleSize);
		
		var tween = CreateTween();
		tween.TweenProperty(this, "position:y", 389, 3).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenInterval(1);
		tween.TweenMethod(Callable.From<float>(SetCircleSize), 1.05f, 0.154f, 1.6).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenInterval(1);
		tween.TweenMethod(Callable.From<float>(SetCircleSize), 0.154f, 0, 0.25).SetEase(Tween.EaseType.In);
		tween.TweenInterval(1.5);
		tween.Finished += () => GameManager.Instance.ChangeScene(GameManager.Scene.MainMenu);
	}

	private void SetCircleSize(float size) => (_circle.Material as ShaderMaterial)?.SetShaderParameter("circle_size", size);
}
