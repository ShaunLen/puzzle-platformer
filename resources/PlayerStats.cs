using Godot;

namespace PuzzlePlatformer.resources;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public int Health;
    
    [ExportGroup("Enabled Abilities")] 
    [Export] public bool JumpEnabled;
    [Export] public bool DashEnabled;
    [Export] public bool WallJumpEnabled;
    
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