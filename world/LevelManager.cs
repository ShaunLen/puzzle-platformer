using System.Linq;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.entities.player;
using PuzzlePlatformer.litescript_two.Nodes;
using PuzzlePlatformer.world.levels;

namespace PuzzlePlatformer.world;

public partial class LevelManager : Node
{
    public static LevelManager Instance { get; private set; }
    [Export] public LevelRoot CurrentLevel;
    [Export] public GameManager.Scene NextLevel;
    [Export] public bool CodelessLevel;
    [Export] public Player Player;
    public Vector2 PlayerPosition => Player.Position;
    public ProgramNode Program;

    public override void _Ready() => Instance = this;

    public override void _Process(double delta)
    {
        if(InputManager.IsActionJustPressed(InputManager.Action.HighlightInteractables))
            HighlightInteractables(delta);
    }

    public bool CheckRequirementsMet(ProgramNode program)
    {
        Program = program;
        return !CurrentLevel.Requirements.Any(req => !req.RequirementMet(program) && req.Required);
    }

    private void HighlightInteractables(double delta)
    {
        var interactables = GetTree().GetNodesInGroup("Interactable").ToList();

        foreach (var obj in interactables)
        {
            var shaderTween = CreateTween();
            var hintTween = CreateTween();
            
            var shader = obj.GetNode<Sprite2D>("Sprite2D").Material;
            shaderTween.TweenProperty(shader, "shader_parameter/shine_progress", 0.66, 0.5);
            shaderTween.TweenProperty(shader, "shader_parameter/shine_progress", 0, 0.5);

            var hint = obj.GetNode<Label>("Hint");
            hint.Text = obj.Name;
            hintTween.TweenProperty(hint, "modulate:a", 1, 0.5);
            hintTween.TweenProperty(hint, "modulate:a", 1, 2);
            hintTween.TweenProperty(hint, "modulate:a", 0, 1.5);
        }
    }

    
}