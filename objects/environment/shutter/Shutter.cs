using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.ui.key_hint;

namespace PuzzlePlatformer.objects.environment.shutter;

public partial class Shutter : Node
{
	[Export] private bool _enabled;
	[Export] private GameManager.Scene _scene;
	[Export] private string _text;
	[Export] private Texture2D _keyHintIcon;
	
	private AnimationPlayer _animationPlayer;
	private AudioStreamPlayer2D _audioPlayer;
	private HitboxComponent _hitboxComponent;
	private KeyHint _keyHint;
	private bool _isOpen;
	
	/* Override Methods */
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		_keyHint = GetNode<KeyHint>("KeyHint");
		
		if (!_enabled)
		{
			_keyHint.QueueFree();
			_animationPlayer.PlayBackwards("open_shutter");
			AudioManager.Instance.PlaySound(AudioManager.Sound.Shutter, _audioPlayer);
			return;
		}
		
		_keyHint.KeyIcon = _keyHintIcon;
		_keyHint.Text = _text;
		_keyHint.ShowKey = false;

		_hitboxComponent.PlayerEntered += OpenShutter;
		_hitboxComponent.PlayerExited += CloseShutter;
	}

	public override void _Process(double delta)
	{
		if (!_isOpen || !_enabled) return;
		if (!InputManager.IsActionJustPressed(InputManager.Action.Interact)) return;

		_enabled = false;
		GameManager.Instance.ChangeScene(_scene);
	}

	/* Public Methods */
	
	private void OpenShutter()
	{
		if (!_enabled) return;

		_isOpen = true;
		_animationPlayer.Play("open_shutter");
		AudioManager.Instance.PlaySound(AudioManager.Sound.Shutter, _audioPlayer);
		_keyHint.ShowKey = true;
		_keyHint.Text = $" {_text}"; // space for gap
	}

	private void CloseShutter()
	{
		if (!_enabled) return;

		_isOpen = false;
		_animationPlayer.PlayBackwards("open_shutter");
		AudioManager.Instance.PlaySound(AudioManager.Sound.Shutter, _audioPlayer);
		_keyHint.ShowKey = false;
		_keyHint.Text = _text;
	}
}
