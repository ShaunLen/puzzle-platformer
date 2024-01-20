using Godot;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.objects.boxes.heavy_box;

namespace PuzzlePlatformer.components;

[GlobalClass]
public partial class HitboxComponent : Area2D
{
    [Signal] public delegate void PlayerEnteredEventHandler();
    [Signal] public delegate void PlayerExitedEventHandler();
    [Signal] public delegate void HeavyBoxEnteredEventHandler();
    [Signal] public delegate void HeavyBoxExitedEventHandler();
    
    public override void _Ready()
    {
        BodyEntered += body =>
        {
            switch (body)
            {
                case Player:
                    EmitSignal(SignalName.PlayerEntered);
                    break;
                case HeavyBox:
                    EmitSignal(SignalName.HeavyBoxEntered);
                    break;
            }
        };
        
        BodyExited += body =>
        {
            switch (body)
            {
                case Player:
                    EmitSignal(SignalName.PlayerExited);
                    break;
                case HeavyBox:
                    EmitSignal(SignalName.HeavyBoxExited);
                    break;
            }
        };
    }
}