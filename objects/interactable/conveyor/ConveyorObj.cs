using System;
using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.world;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;
using ValueType = PuzzlePlatformer.litescript_two.Runtime.Values.ValueType;

namespace PuzzlePlatformer.objects.interactable.conveyor;

public partial class ConveyorObj : Interactable
{
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();

	[Export(PropertyHint.Range, "10,200,10")] private int _moveSpeed = 50;
	[Export] private bool _moveOnStart;
	private bool _isMoving;
	private bool _moveBackwards;

	[Export] private StaticBody2D _body;
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;

	public override void _Ready()
	{
		base._Ready();

		_body.ConstantLinearVelocity = new Vector2(0, 0);
		
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		
		Properties.Add("IsMoving", "Set to '" + "true".Highlight() + "' if conveyor is moving, and '" + "false".Highlight() + "' if conveyor is not moving.\n" +
									"Value type: boolean".Darken());
		
		Methods.Add("Start()", "Starts the conveyor.\n" +
							  "Return type: null".Darken());
		
		Methods.Add("Stop()", "Stops the conveyor.\n" +
							   "Return type: null".Darken());
		
		Methods.Add("Reverse()", "Flips the conveyor movement direction.\n" +
							   "Return type: null".Darken());
		
		Methods.Add("SetSpeed()", "Sets the conveyor move speed.\n" +
								 "Args: (Number) Speed | ".Darken() +
								 "Return type: null".Darken());

		if (_moveOnStart)
			Start(null, null);
	}
	
	/* Methods */

	private IRuntimeValue Start(List<IRuntimeValue> args, Env env)
	{
		if(!LevelManager.Instance.CodelessLevel)
			UpdateProperties();

		if (_isMoving)
		{
			CodeManager.Instance.ConsoleWriteError($"Can't call method {Name}.Start()' - conveyor is already started!");
			throw new Exception();
		}
			
		_isMoving = true;

		_animationPlayer.SpeedScale = _moveSpeed / 10f;

		if (_moveBackwards)
		{
			_animationPlayer.PlayBackwards("conveyor_move");
			_body.ConstantLinearVelocity = new Vector2(-_moveSpeed, 0);
		}
		else
		{
			_animationPlayer.Play("conveyor_move");
			_body.ConstantLinearVelocity = new Vector2(_moveSpeed, 0);
		}

		return new NullValue();
	}
	
	private IRuntimeValue Stop(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();

		if (!_isMoving)
		{
			CodeManager.Instance.ConsoleWriteError($"Can't call method {Name}.Stop()' - conveyor is already stopped!");
			throw new Exception();
		}
			
		_isMoving = false;
		_body.ConstantLinearVelocity = new Vector2(0, 0);
		_animationPlayer.Pause();

		return new NullValue();
	}
	
	private IRuntimeValue Reverse(List<IRuntimeValue> args, Env env)
	{
		_moveBackwards = !_moveBackwards;

		if (!_isMoving) return new NullValue();
			
		_animationPlayer.Pause();
		
		if(_moveBackwards)
			_animationPlayer.PlayBackwards("conveyor_move");
		else
			_animationPlayer.Play("conveyor_move");
		
		_body.ConstantLinearVelocity = _moveBackwards ? new Vector2(-_moveSpeed, 0) : new Vector2(_moveSpeed, 0);
		
		return new NullValue();
	}
	
	private IRuntimeValue SetSpeed(List<IRuntimeValue> args, Env env)
	{
		if (args.Count != 1)
		{
			CodeManager.Instance.ConsoleWriteError($"Method {Name}.SetSpeed()' expects 1 argument, got {args.Count}.");
			throw new Exception();
		}

		if (args[0].Type != ValueType.Number)
		{
			CodeManager.Instance.ConsoleWriteError($"Method {Name}.SetSpeed()' expects a number argument.");
			throw new Exception();
		}

		var speed = ((NumberValue)args[0]).Value;

		if (speed is < 10 or > 500)
		{
			CodeManager.Instance.ConsoleWriteError($"Method {Name}.SetSpeed()' expects speed between 10 and 500.");
			throw new Exception();
		}

		_moveSpeed = (int) ((NumberValue)args[0]).Value;
		_animationPlayer.SpeedScale = _moveSpeed / 10f;

		if (!_isMoving) return new NullValue();
		
		_body.ConstantLinearVelocity = _moveBackwards ? new Vector2(-_moveSpeed, 0) : new Vector2(_moveSpeed, 0);

		return new NullValue();
	}
	
	/* Overrides */
	
	public override Dictionary<string, IRuntimeValue> GetProperties()
	{
		var properties = new Dictionary<string, IRuntimeValue>();
		
		// Methods
		properties.Add("Start", new NativeFunctionValue(Start));
		properties.Add("Stop", new NativeFunctionValue(Stop));
		properties.Add("Reverse", new NativeFunctionValue(Reverse));
		properties.Add("SetSpeed", new NativeFunctionValue(SetSpeed));
		
		// Properties
		properties.Add("IsMoving", new BooleanValue(_isMoving));

		return properties;
	}

	protected override void UpdateProperties()
	{
		var obj = CodeManager.Instance.GlobalEnvironment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsMoving"] = new BooleanValue(_isMoving);
	}
}
