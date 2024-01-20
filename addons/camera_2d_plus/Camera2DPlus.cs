using System;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.world;
using PuzzlePlatformer.world.levels;

namespace PuzzlePlatformer.addons.camera_2d_plus;

[GlobalClass]
public partial class Camera2DPlus : Camera2D
{
	[ExportGroup("Zoom Smoothing")] 
	[Export] private bool _zoomSmoothingEnabled = true;
	[Export(PropertyHint.Range, "1,5")] private int _zoomSmoothingSpeed = 2;

	[ExportGroup("Offset Smoothing")] 
	[Export] private bool _offsetSmoothingEnabled = true;
	[Export(PropertyHint.Range, "1,10")] private int _offsetSmoothingSpeed = 10;

	[ExportGroup("Limits")]
	[Export] private bool _useLevelBounds = true;
	
	/* Targets */
	private Vector2 _targetOffset;
	private Vector2 _targetZoom;

	private bool _initialised;
	private float _defaultZoom;
	private Vector2 _storedZoom;
	private int _storedZoomSpeed;

	/* Override Methods */
	
	public override void _Ready()
	{
		_defaultZoom = LevelManager.Instance.CurrentLevel.DefaultZoom;
		_targetOffset = Offset;
		_targetZoom = new Vector2(_defaultZoom, _defaultZoom);
		_offsetSmoothingSpeed *= 20;

		if (_useLevelBounds)
			GetTree().CurrentScene.Ready += SetLimits;
	}

	public override void _Process(double delta)
	{
		if (InputManager.IsActionJustPressed(InputManager.Action.ZoomIn) && _targetZoom < new Vector2(2, 2))
			_targetZoom = new Vector2(_targetZoom.X + 0.2f, _targetZoom.Y + 0.2f);
		
		if (InputManager.IsActionJustPressed(InputManager.Action.ZoomOut) && _targetZoom > new Vector2(1, 1))
			_targetZoom = new Vector2(_targetZoom.X - 0.2f, _targetZoom.Y - 0.2f);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		if (!_initialised)
			PositionSmoothingEnabled = false;

		if (Offset != _targetOffset)
		{
			if (_offsetSmoothingEnabled && _initialised)
				SmoothOffset(delta);
			else
				Offset = _targetOffset;
		}
		
		if (Zoom != _targetZoom)
		{
			if (_zoomSmoothingEnabled && _initialised)
				SmoothZoom(delta);
			else
				Zoom = _targetZoom;
		}

		_initialised = true;
		PositionSmoothingEnabled = true;
	}
	
	/* Public Methods */

	public void GrabFocus(Vector2 position, float targetZoom, int zoomSpeed)
	{
		_storedZoom = _targetZoom;
		_storedZoomSpeed = _zoomSmoothingSpeed;
		_targetZoom = new Vector2(targetZoom, targetZoom);
		_zoomSmoothingSpeed = zoomSpeed;
		Position = position;
	}

	public void ReleaseFocus()
	{
		_targetZoom = _storedZoom;
		_zoomSmoothingSpeed = _storedZoomSpeed;
	}

	/* Private Methods */
	
	private void SmoothOffset(double delta)
	{
		var d = (float) delta * _offsetSmoothingSpeed;
		Offset = new Vector2(Mathf.MoveToward(Offset.X, _targetOffset.X, d), Mathf.MoveToward(Offset.Y, _targetOffset.Y, d));
	}

	private void SmoothZoom(double delta)
	{
		var d = (float) delta * _zoomSmoothingSpeed;
		Zoom = new Vector2(Mathf.MoveToward(Zoom.X, _targetZoom.X, d), Mathf.MoveToward(Zoom.Y, _targetZoom.Y, d));
	}

	private void SetLimits()
	{
		var bounds = GetNode<LevelRoot>(GetTree().CurrentScene.GetPath()).LevelBounds;

		LimitLeft = 0;
		LimitBottom = 0;
		LimitRight = (int) bounds.X;
		LimitTop = (int) -bounds.Y;
	}
}
