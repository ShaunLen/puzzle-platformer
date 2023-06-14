using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class PlayerStats : Stats
{
    [Export] public int WalkSpeed;
}