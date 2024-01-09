using System;
using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.objects.boxes.heavy_box;
using PuzzlePlatformer.world;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.objects.interactable.fabricator;

public partial class Fabricator : Interactable
{
	[Signal] public delegate void FinishedFabricatingEventHandler();
	public override Dictionary<string, string> Properties { get; } = new();
	public override Dictionary<string, string> Methods { get; } = new();

	private bool _isFabricating;
	private bool _isOpening;
	private bool _containsBox;
	
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioStreamPlayer;
	private HitboxComponent _hitboxComponent;
	private Sprite2D _sprite;
	private Timer _hatchTimer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_hatchTimer = GetNode<Timer>("HatchTimer");
		
		Properties.Add("IsFabricating", "Set to '" + "true".Highlight() + "' when fabricating, and '" + "false".Highlight() + "' when not.\n" +
								 "Value type: boolean".Darken());
		
		Properties.Add("IsOpen", "Set to '" + "true".Highlight() + "' when hatch is open, and '" + "false".Highlight() + "' when hatch is closed.\n" +
								 "Value type: boolean".Darken());
		
		Properties.Add("ContainsBox", "Whether or not the fabricator contains a box.\n" +
								 "Value type: boolean".Darken());
		
		Methods.Add("Fabricate()", "Fabricates a box.\n" +
							  "Return type: null".Darken());
		
		Methods.Add("Open()", "Opens the hatch.\n" +
							   "Return type: null".Darken());
		
		_animationPlayer.AnimationStarted += name =>
		{
			if(name == "open_hatch" || name == "close_hatch")
				AudioManager.Instance.PlaySound(AudioManager.Sound.Door, _audioStreamPlayer);
		}; 
		
		_animationPlayer.AnimationFinished += name =>
		{
			if (name == "open_hatch" || name == "close_hatch")
				AudioManager.Instance.PlaySound(AudioManager.Sound.DoorStop, _audioStreamPlayer);
		};

		_hitboxComponent.HeavyBoxEntered += () => _containsBox = true;
		_hitboxComponent.HeavyBoxExited += () => _containsBox = false;
	}
	
	/* Methods */

	private IRuntimeValue Fabricate(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();

		if (_isOpening)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Fabricate()' while hatch is opening!");
			throw new Exception();
		}

		if (_containsBox)
		{
			CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Fabricate()'- fabricator already contains a box!");
			throw new Exception();
		}
			
		_isFabricating = true;

		var defaultSpeed = _animationPlayer.SpeedScale;
		_animationPlayer.SpeedScale = 1f;
		_animationPlayer.Play("start_fabricator");
		_animationPlayer.Queue("fabricate");
		
		AudioManager.Instance.PlaySound(AudioManager.Sound.Fabricate, _audioStreamPlayer);
		
		var heavyBoxScene = GD.Load<PackedScene>("res://objects/boxes/heavy_box/heavy_box.tscn");
		var heavyBoxInstance = heavyBoxScene.Instantiate();
		AddChild(heavyBoxInstance);

		var heavyBox = (HeavyBox)heavyBoxInstance;
		heavyBox.Freeze = true;
		heavyBox.Position = new Vector2(heavyBox.Position.X, heavyBox.Position.Y + 5);

		_audioStreamPlayer.Finished += StopFabricating;

		return new NullValue();

		void StopFabricating()
		{
			if (!_isFabricating) return;

			heavyBox.Freeze = false;
			
			_animationPlayer.SpeedScale = defaultSpeed;
			_animationPlayer.Stop();
			_sprite.Frame = 15;

			_isFabricating = false;
			_audioStreamPlayer.Finished -= StopFabricating;
			EmitSignal(SignalName.FinishedFabricating);
		}
	}
	
	private IRuntimeValue Open(List<IRuntimeValue> args, Env env)
	{
		UpdateProperties();
		
		if (_isFabricating)
		{
			FinishedFabricating += OpenHatch;
		}
		else
			OpenHatch();
		
		return new NullValue();

		void OpenHatch()
		{
			_isOpening = true;
		
			_animationPlayer.Play("open_hatch");

			_hatchTimer.Timeout += CloseHatch;
			_hatchTimer.Start();
			FinishedFabricating -= OpenHatch;
			return;

			void CloseHatch()
			{
				_animationPlayer.Play("close_hatch");
				_containsBox = false;
				_isOpening = false;
				_hatchTimer.Timeout -= CloseHatch;
			}
		}
	}
	
	/* Overrides */

	public override Dictionary<string, IRuntimeValue> GetProperties()
	{
		var properties = new Dictionary<string, IRuntimeValue>();
		
		// Methods
		properties.Add("Fabricate", new NativeFunctionValue(Fabricate));
		properties.Add("Open", new NativeFunctionValue(Open));
		
		// Properties
		properties.Add("IsFabricating", new BooleanValue(_isFabricating));
		properties.Add("IsOpening", new BooleanValue(_isOpening));
		properties.Add("ContainsBox", new BooleanValue(_containsBox));

		return properties;
	}

	protected override void UpdateProperties()
	{
		var obj = CodeManager.Instance.Environment.LookupVar(Name) as ObjectValue;
		obj!.Properties["IsFabricating"] = new BooleanValue(_isFabricating);
		obj!.Properties["IsOpening"] = new BooleanValue(_isOpening);
		obj!.Properties["ContainsBox"] = new BooleanValue(_containsBox);
	}
}
