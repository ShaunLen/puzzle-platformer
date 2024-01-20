using Godot;
using System;
using PuzzlePlatformer.addons.camera_2d_plus;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.world;

public partial class CameraFocusPoint : Area2D
{
	[Signal] public delegate void PlayerEnteredEventHandler();
	[Signal] public delegate void PlayerExitedEventHandler();

	[Export] private Camera2DPlus _camera;
	private RemoteTransform2D _playerTransform;
	private Vector2 _cameraZoom;
	
	public override void _Ready()
	{
		_playerTransform = LevelManager.Instance.Player.GetNode<RemoteTransform2D>("PlayerTransform");

		BodyEntered += GrabFocus;
		BodyExited += ReleaseFocus;
	}

	private void GrabFocus(Node2D body)
	{
		if (body is not Player) return;
		
		_playerTransform.UpdatePosition = false;
		_playerTransform.UpdateRotation = false;
		_playerTransform.UpdateScale = false;

		_camera.GrabFocus(Position, 3, 5);
	}

	private void ReleaseFocus(Node2D body)
	{
		if (body is not Player) return;
		
		_playerTransform.UpdatePosition = true;
		_playerTransform.UpdateRotation = true;
		_playerTransform.UpdateScale = true;

		_camera.ReleaseFocus();
	}
}
