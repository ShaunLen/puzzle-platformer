using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export] public int WalkSpeed;
    [Export] public int WalkAcceleration;
    [Export] public int WalkDeceleration;
}