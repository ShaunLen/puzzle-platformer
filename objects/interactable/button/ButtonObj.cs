using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.components;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.litescript.Runtime.Values;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.interactable.button;

public partial class ButtonObj : Interactable
{
    public override Dictionary<string, string> Properties { get; set; } = new();
    public override Dictionary<string, string> Methods { get; set; } = new();
    
    public bool IsPressed;
    
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer2D _audioStreamPlayer;

    private HitboxComponent _hitbox;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _hitbox = GetNode<HitboxComponent>("HitboxComponent");

        _hitbox.PlayerEntered += () =>
        {
            _animationPlayer.Play("button_press");
            AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonPress, _audioStreamPlayer);
            IsPressed = true;
            UpdateProperties();
        };
        _hitbox.PlayerExited += () =>
        {
            _animationPlayer.PlayBackwards("button_press");
            AudioManager.Instance.PlaySound(AudioManager.Sound.ButtonRelease, _audioStreamPlayer);
            IsPressed = false;
            UpdateProperties();
        };
        
        Properties.Add("IsPressed", "Set to 'true' if button is pressed, and 'false' if button is not pressed.");
    }
    
    /* Overrides */
    
    public override Dictionary<string, IRuntimeValue> GetProperties()
    {
        var properties = new Dictionary<string, IRuntimeValue>();
        
        // Properties
        properties.Add("IsPressed", new BooleanValue(IsPressed));

        return properties;
    }

    public override void UpdateProperties()
    {
        var obj = LevelManager.Instance.Environment.LookupVar(Name) as ObjectValue;
        obj!.Properties["IsPressed"] = new BooleanValue(IsPressed);
    }
}