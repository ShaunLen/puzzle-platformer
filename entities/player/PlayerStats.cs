using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [ExportGroup("Grounded")]
    [Export] public int WalkSpeed;
    [Export] public int WalkAcceleration;
    [Export] public int WalkDeceleration;

    [ExportGroup("In Air")] 
    [Export] public int AirSpeed;
    [Export] public int AirAcceleration;
    [Export] public int AirDeceleration;
    [Export] public int MaxFallSpeed;
    [Export] public int FastFallSpeed;

    [ExportGroup("Jump")] 
    [Export] public int JumpHeight;
    [Export] public float JumpTimeToApex;
    [Export] public float JumpTimeToGround;
    [Export] public float JumpApexBoost;
    [Export] public int JumpCutMultiplier;

    [ExportGroup("On Wall")] 
    [Export] public int WallSlideSpeed;
    [Export] public int FastSlideMultiplier;
}