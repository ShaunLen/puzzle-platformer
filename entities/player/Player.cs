using System.Runtime.CompilerServices;
using Godot;
using PuzzlePlatformer.entities.common;

namespace PuzzlePlatformer.entities.player;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    [Export] public PlayerStats Stats;
    private StateMachine _stateMachine;

    public override void _Ready()
    {
        base._Ready();
        _stateMachine = GetNode<StateMachine>("StateMachine");
        _stateMachine.Init();
    }
}