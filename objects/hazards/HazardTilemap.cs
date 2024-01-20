using Godot;
using PuzzlePlatformer.components;

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
                hitbox.BodyEntered += body =>
                {
                    foreach (var node in body.GetChildren())
                    {
                        if(node is HealthComponent healthComponent)
                            healthComponent.Kill();
                    }
                };
            // hitbox.PlayerEntered += GetTree().GetFirstNodeInGroup("Player").GetNode<HealthComponent>("HealthComponent").Kill;
        }
    }
}