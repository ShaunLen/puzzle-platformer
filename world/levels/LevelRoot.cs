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
    [Export(PropertyHint.Range, "1,2,0.2")]  public float DefaultZoom { get; set; }
    [Export] private Node2D RespawnPosition;
    [Export] private protected TileMap GroundTilemap;
    
    public Vector2 LevelBounds;
    
    public List<requirements.Requirement> Requirements { get; set; }

    public override void _Ready()
    {
        LevelBounds = GroundTilemap.GetUsedRect().Size * GroundTilemap.CellQuadrantSize;
        DefineRequirements();
        AddInitialCode();
    }
    
    public Vector2 GetRespawnPosition()
    {
        return RespawnPosition.Position;
    }

    private void AddInitialCode()
    {
        // If settings.showInitialCode = true...
        CodeManager.Instance.SetCode(InitialCode);
    }
    
    protected abstract void DefineRequirements();
}