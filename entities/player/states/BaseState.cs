using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player.states;

public partial class BaseState : State
{
    protected Player Player;
    protected resources.PlayerStats Stats;

    public override void _Ready()
    {
        Player = (Player) GetTree().GetFirstNodeInGroup("Player");
        Stats = Player.Stats;
    }
}