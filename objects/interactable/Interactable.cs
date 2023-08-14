using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.litescript.Runtime.Values;

namespace PuzzlePlatformer.objects.interactable;

public abstract partial class Interactable : Node2D
{
    public int AccessCount = 0;

    public abstract Dictionary<string, IRuntimeValue> GetProperties();

    public abstract void UpdateProperties();
}