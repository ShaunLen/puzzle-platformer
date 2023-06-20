using Godot;
using PuzzlePlatformer.entities.player;

namespace PuzzlePlatformer.entities.common;

[GlobalClass]
public partial class HitboxComponent : Area2D
{
    [Signal] public delegate void PlayerEnteredEventHandler();
    [Signal] public delegate void PlayerExitedEventHandler();
    
    public override void _Ready()
    {
        BodyEntered += body =>
        {
            if(body is Player)
                EmitSignal(SignalName.PlayerEntered);
        };
        
        BodyExited += body =>
        {
            if(body is Player)
                EmitSignal(SignalName.PlayerExited);
        };
    }
}