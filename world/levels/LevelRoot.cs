using System.Collections.Generic;
using Godot;
using PuzzlePlatformer.autoloads;

namespace PuzzlePlatformer.world.levels;

[GlobalClass]
public abstract partial class LevelRoot : Node2D
{
    [ExportCategory("Level Information")]
    [Export] public string LevelName { get; set; }
    [Export(PropertyHint.MultilineText)] public string LevelDesc { get; set; }
    [Export(PropertyHint.MultilineText)] public string InitialCode { get; set; }
    
    [Export] protected private TileMap GroundTilemap;

    public Vector2 LevelBounds;
    
    public List<requirements.Requirement> Requirements { get; set; }

    public override void _Ready()
    {
        LevelBounds = GroundTilemap.GetUsedRect().Size * GroundTilemap.CellQuadrantSize;
        DefineRequirements();
        AddInitialCode();
    }

    private void AddInitialCode()
    {
        // If settings.showInitialCode = true...
        CodeManager.Instance.SetCode(InitialCode);
    }
    
    protected abstract void DefineRequirements();
}