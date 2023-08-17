using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.litescript.Runtime.Values;

namespace PuzzlePlatformer.objects.interactable;

public abstract partial class Interactable : Node2D
{
    [Export(PropertyHint.MultilineText)] public string Description;
    public abstract Dictionary<string, string> Properties { get; set; }
    public abstract Dictionary<string, string> Methods { get; set; }

    public abstract Dictionary<string, IRuntimeValue> GetProperties();

    public abstract void UpdateProperties();
}