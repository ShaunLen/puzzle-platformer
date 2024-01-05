using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.interactable.button;

public partial class ButtonObj : Interactable
{
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();

	private bool _isPressed;

	private HitboxComponent _hitbox;

	public override void _Ready()
	{
		base._Ready();
		
		_hitbox = GetNode<HitboxComponent>("HitboxComponent");

		_hitbox.PlayerEntered += () =>
		{
			AnimationPlayer.Play("button_press");
			AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonPress, AudioStreamPlayer);
			_isPressed = true;
			UpdateProperties();
		};
		_hitbox.PlayerExited += () =>
		{
			AnimationPlayer.PlayBackwards("button_press");
			AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonRelease, AudioStreamPlayer);
			_isPressed = false;
			UpdateProperties();
		};
		
		Properties.Add("IsPressed", "Set to '" + "true".Highlight() + "' if button is pressed, and '" + "false".Highlight() + "' if button is not pressed.\n" +
									"Value type: boolean".Darken());
	}
	
	/* Overrides */
	
	public override Dictionary<string, IRuntimeValue> GetProperties()
	{
		var properties = new Dictionary<string, IRuntimeValue>();
		
		// Properties
		properties.Add("IsPressed", new BooleanValue(_isPressed));

		return properties;
	}

	protected override void UpdateProperties()
	{
		var obj = LevelManager.Instance.Environment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsPressed"] = new BooleanValue(_isPressed);
	}
}
