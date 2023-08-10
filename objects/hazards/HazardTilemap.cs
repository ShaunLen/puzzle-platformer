using Godot;
using PuzzlePlatformer.components;
using PuzzlePlatformer.entities.player;

namespace PuzzlePlatformer.objects.hazards;

[GlobalClass]
public partial class HazardTilemap : TileMap
{
    /* Override Methods */
    public override void _Ready()
    {
        base._Ready();

        foreach (var child in GetChildren())
        {
            if (child is HitboxComponent hitbox)
                hitbox.PlayerEntered += GetTree().GetFirstNodeInGroup("Player").GetNode<HealthComponent>("HealthComponent").Kill;
        }
    }
}