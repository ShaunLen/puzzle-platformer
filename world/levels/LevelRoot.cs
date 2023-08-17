using System.Collections.Generic;
using Godot;

namespace PuzzlePlatformer.world.levels;

[GlobalClass]
public abstract partial class LevelRoot : Node2D
{
    [ExportCategory("Level Information")]
    [Export] public string LevelName { get; set; }
    [Export(PropertyHint.MultilineText)] public string LevelDesc { get; set; }
    
    [Export] protected private TileMap GroundTilemap; 
    
    public Vector2 LevelBounds { get; protected set; }
    
    public List<requirements.Requirement> Requirements { get; set; }

    public override void _Ready()
    {
        LevelBounds = GroundTilemap.GetUsedRect().Size * GroundTilemap.CellQuadrantSize;
        DefineRequirements();
    }

    protected abstract void DefineRequirements();
}