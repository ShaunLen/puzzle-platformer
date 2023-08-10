using Godot;

namespace PuzzlePlatformer.world.levels;

[GlobalClass]
public partial class LevelRoot : Node2D
{
    [Export] private TileMap _groundTilemap; 
    public Vector2 LevelBounds;

    public override void _Ready()
    {
        LevelBounds = _groundTilemap.GetUsedRect().Size * _groundTilemap.CellQuadrantSize;
    }
}