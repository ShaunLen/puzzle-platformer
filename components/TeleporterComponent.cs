using Godot;
using PuzzlePlatformer.entities.player;

namespace PuzzlePlatformer.components;

[GlobalClass]
public partial class TeleporterComponent : Node2D
{
    [Export] private Player _player;
    [Export] private TeleporterComponent _linkedTeleporter;
    [Export] private Vector2 _teleportOffset;
    private HitboxComponent _detectionArea;

    public override void _Ready()
    {
        _detectionArea = GetNode<HitboxComponent>("HitboxComponent");
        
        if (_detectionArea == null)
        {
            GD.PrintErr("TeleporterComponent requires a HitboxComponent child.");
            GetTree().Quit();
        }
        
        if (_linkedTeleporter == null)
        {
            GD.PrintErr("TeleporterComponent requires a Linked Teleporter.");
            GetTree().Quit();
        }

        _detectionArea!.PlayerEntered += _linkedTeleporter!.TakePlayer;
    }

    public void TakePlayer() => _player.Position = Position + _teleportOffset;
}