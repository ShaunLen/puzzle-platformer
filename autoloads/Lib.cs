using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace PuzzlePlatformer.autoloads;

public partial class Lib : Node
{
    public static Lib Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public Node GetNodeOfType<TClass>(Node parent)
    {
        var children = parent.GetChildren();
        return children.FirstOrDefault(child => child.GetType() == typeof(TClass));
    }
}