using System;
using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.objects.interactable.door;

public partial class Door : Interactable
{
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();

	private bool _isOpen;
	
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;
	private LightOccluder2D _lightOccluder2D;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_lightOccluder2D = GetNode<LightOccluder2D>("LightOccluder2D");

		_lightOccluder2D.Occluder.Polygon = new[]
			{ new Vector2(-12, -48), new Vector2(-12, 48), new Vector2(12, 48), new Vector2(12, -48) };

		_animationPlayer.AnimationStarted += _ => AudioManager.Instance.PlaySound(AudioManager.Sound.Door, _audioStreamPlayer); 
		_animationPlayer.AnimationFinished += _ => AudioManager.Instance.PlaySound(AudioManager.Sound.DoorStop, _audioStreamPlayer);
		
		Properties.Add("IsOpen", "Set to '" + "true".Highlight() + "' when door is open, and '" + "false".Highlight() + "' when door is closed.\n" +
								 "Value type: boolean".Darken());
		
		Methods.Add("Open()", "Opens the door.\n" +
							  "Return type: null".Darken());
		
		Methods.Add("Close()", "Closes the door.\n" +
							   "Return type: null".Darken());
	}
	
	/* Methods */

	private IRuntimeValue Open(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();

		if (_isOpen)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Open()' - door is already open!");
			throw new Exception();
		}
			
		_isOpen = true;
		_animationPlayer.Play("door_open");

		return new NullValue();
	}
	
	private IRuntimeValue Close(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();
		
		if (!_isOpen)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Close()' - door is already closed!");
			throw new Exception();
		}
			
		_isOpen = false;
		_animationPlayer.PlayBackwards("door_open");
		
		return new NullValue();
	}
	
	/* Overrides */

	public override Dictionary<string, IRuntimeValue> GetProperties()
	{
		var properties = new Dictionary<string, IRuntimeValue>();
		
		// Methods
		properties.Add("Open", new NativeFunctionValue(Open));
		properties.Add("Close", new NativeFunctionValue(Close));
		
		// Properties
		properties.Add("IsOpen", new BooleanValue(_isOpen));

		return properties;
	}

	protected override void UpdateProperties()
	{
		var obj = CodeManager.Instance.GlobalEnvironment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsOpen"] = new BooleanValue(_isOpen);
	}
}
