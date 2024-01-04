using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.litescript_two.Runtime.Values;

namespace PuzzlePlatformer.objects.interactable;

public abstract partial class Interactable : Node2D
{
    [Export] public Texture2D Sprite;
    [Export(PropertyHint.MultilineText)] public string Description;
    public abstract Dictionary<string, string> Properties { get; }
    public abstract Dictionary<string, string> Methods { get; }

    public abstract Dictionary<string, IRuntimeValue> GetProperties();
    
    protected private AnimationPlayer AnimationPlayer;
    protected private AudioStreamPlayer2D AudioStreamPlayer;

    protected abstract void UpdateProperties();

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        AudioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        GetNode<Sprite2D>("Sprite2D").Texture = Sprite;
    }
}

internal static class InteractableExtensions
{
    public static string Highlight(this string str)
    {
        return "[color=#b2cbcf]" + str + "[/color]";
    }
    
    public static string Darken(this string str)
    {
        return "[color=#47585c]" + str + "[/color]";
    }
}