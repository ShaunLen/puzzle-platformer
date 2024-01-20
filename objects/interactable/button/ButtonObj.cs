using System;
using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.objects.interactable.button;

public partial class ButtonObj : Interactable
{
	[Flags]
	public enum ObjectsThatCanPress : byte
	{
		None = 0,
		Player = 1,
		Box = 2,
		HeavyBox = 4
	}
	
	[Export(PropertyHint.Flags, "Player,Box,HeavyBox")]
	public ObjectsThatCanPress CanPress { get; set; }
	
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();

	private bool _isPressed;
	private HitboxComponent _hitbox;

	public override void _Ready()
	{
		base._Ready();
		
		_hitbox = GetNode<HitboxComponent>("HitboxComponent");

		if ((CanPress & ObjectsThatCanPress.Player) != 0)
		{
			_hitbox.PlayerEntered += PressButton;
			_hitbox.PlayerExited += ReleaseButton;
		}
		
		if ((CanPress & ObjectsThatCanPress.HeavyBox) != 0)
		{
			_hitbox.HeavyBoxEntered += PressButton;
			_hitbox.HeavyBoxExited += ReleaseButton;
		}
		
		Properties.Add("IsPressed", "Set to '" + "true".Highlight() + "' if button is pressed, and '" + "false".Highlight() + "' if button is not pressed.\n" +
									"Value type: boolean".Darken());
	}

	/* Private Methods */

	private void PressButton()
	{
		AnimationPlayer.Play("button_press");
		AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonPress, AudioStreamPlayer);
		_isPressed = true;
		UpdateProperties();
	}

	private void ReleaseButton()
	{
		AnimationPlayer.PlayBackwards("button_press");
		AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonRelease, AudioStreamPlayer);
		_isPressed = false;
		UpdateProperties();
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
		var obj = CodeManager.Instance.GlobalEnvironment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsPressed"] = new BooleanValue(_isPressed);
	}
}
