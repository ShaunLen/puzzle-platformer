using System;
using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.interactable.door;

public partial class Door : Interactable
{
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();
	
	public bool IsOpen;
	
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		
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

		if (IsOpen)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Open()' - door is already open!");
			throw new Exception();
		}
			
		IsOpen = true;
		_animationPlayer.Play("door_open");

		return new NullValue();
	}
	
	private IRuntimeValue Close(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();
		
		if (!IsOpen)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Close()' - door is already closed!");
			throw new Exception();
		}
			
		IsOpen = false;
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
		properties.Add("IsOpen", new BooleanValue(IsOpen));

		return properties;
	}

	protected override void UpdateProperties()
	{
		var obj = LevelManager.Instance.Environment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsOpen"] = new BooleanValue(IsOpen);
	}
}
