using System;
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
                    GD.Print($"Player entered HitboxComponent of {GetParent().Name}");
                    EmitSignal(SignalName.PlayerEntered);
                    break;
                case HeavyBox:
                    GD.Print($"HeavyBox entered HitboxComponent of {GetParent().Name}");
                    EmitSignal(SignalName.HeavyBoxEntered);
                    break;
            }
        };
        
        BodyExited += body =>
        {
            switch (body)
            {
                case Player:
                    GD.Print($"Player exited HitboxComponent of {GetParent().Name}");
                    EmitSignal(SignalName.PlayerExited);
                    break;
                case HeavyBox:
                    GD.Print($"HeavyBox exited HitboxComponent of {GetParent().Name}");
                    EmitSignal(SignalName.HeavyBoxExited);
                    break;
            }
        };
    }
}