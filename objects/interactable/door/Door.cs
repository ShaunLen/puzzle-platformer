using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript.Runtime;
using PuzzlePlatformer.litescript.Runtime.Values;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.objects.interactable.door;

public partial class Door : Interactable
{
    public override Dictionary<string, string> Properties { get; set; } = new();
    public override Dictionary<string, string> Methods { get; set; } = new();
    
    public bool IsOpen;
    
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer2D _audioStreamPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        
        _animationPlayer.AnimationStarted += _ => AudioManager.Instance.PlaySound(AudioManager.Sound.Door, _audioStreamPlayer); 
        _animationPlayer.AnimationFinished += _ => AudioManager.Instance.PlaySound(AudioManager.Sound.DoorStop, _audioStreamPlayer);
        
        Properties.Add("IsOpen", "Set to 'true' when door is open, and 'false' when door is closed.");
        
        Methods.Add("Open()", "Opens the door.");
        Methods.Add("Close()", "Closes the door.");
    }
    
    /* Methods */

    private IRuntimeValue Open(List<IRuntimeValue> args, Env env)
    {
        UpdateProperties();

        if (IsOpen)
        {
            CodeManager.Instance.ConsoleWriteError("Can't call method '" + Name + ".Open()' - door is already open!");
            return new NullValue();
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
            return new NullValue();
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

    public override void UpdateProperties()
    {
        var obj = LevelManager.Instance.Environment.LookupVar(Name) as ObjectValue;
        obj!.Properties["IsOpen"] = new BooleanValue(IsOpen);
    }
}