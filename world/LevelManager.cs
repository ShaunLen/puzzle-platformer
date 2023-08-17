using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PuzzlePlatformer.addons.camera_2d_plus;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript.Runtime;
using PuzzlePlatformer.litescript.Runtime.Values;
using PuzzlePlatformer.objects.interactable;
using PuzzlePlatformer.objects.interactable.terminal;
using PuzzlePlatformer.world.levels;
using Script = PuzzlePlatformer.litescript.Statements.Script;

namespace PuzzlePlatformer.world;

public partial class LevelManager : Node
{
    public static LevelManager Instance { get; private set; }
    public Env Environment;
    public AudioStreamPlayer AudioStreamPlayer;
    public LevelRoot CurrentLevel;

    private Terminal _terminal;
    private Camera2DPlus _camera;
    
    public List<Interactable> Objects = new();

    public override void _Ready()
    {
        Instance = this;
        _terminal = (Terminal) GetTree().GetFirstNodeInGroup("Terminal");
        _camera = (Camera2DPlus) GetTree().GetFirstNodeInGroup("Camera");
        AudioStreamPlayer = GetTree().GetFirstNodeInGroup("AudioPlayer") as AudioStreamPlayer;
        CurrentLevel = (LevelRoot) GetTree().GetFirstNodeInGroup("Level");
        
        CreateEnvironment();

        CodeManager.Instance.CodeExecuted += CreateEnvironment;
    }

    public override void _Process(double delta)
    {
        if (InputManager.IsActionJustPressed(InputManager.Action.ZoomIn) && _camera.TargetZoom < new Vector2(2, 2))
            _camera.TargetZoom = new Vector2(_camera.TargetZoom.X + 0.2f, _camera.TargetZoom.Y + 0.2f);
        
        if (InputManager.IsActionJustPressed(InputManager.Action.ZoomOut) && _camera.TargetZoom > new Vector2(1, 1))
            _camera.TargetZoom = new Vector2(_camera.TargetZoom.X - 0.2f, _camera.TargetZoom.Y - 0.2f);
        
        if(InputManager.IsActionJustPressed(InputManager.Action.HighlightInteractables))
            HighlightInteractables(delta);
    }

    private void CreateEnvironment()
    {
        // Create LS environment
        Environment = Env.CreateGlobalEnvironment();
        
        // Get list of all interactable objects in level
        var interactables = GetTree().GetNodesInGroup("Interactable").ToList();

        foreach (Interactable obj in interactables)
        {
            Objects.Add(obj);
            Environment.DeclareVariable(obj.Name, new ObjectValue(GetObjectProperties(obj.Name)), false);

        }
    }

    public Dictionary<string, IRuntimeValue> GetObjectProperties(string objName) {
        
        var obj = GetObject(objName);
        Dictionary<string, IRuntimeValue> propertyList = obj.GetType().GetMethod("GetProperties")?.Invoke(obj, null) as Dictionary<string, IRuntimeValue>;

        return propertyList;
    }

    public bool ObjectExists(string objName)
    {
        return Objects.Any(obj => obj.Name.Equals(objName));
    }

    public Interactable GetObject(string objName)
    {
        return Objects.Find(obj => obj.Name.Equals(objName));
    }

    public bool CallMethod(string objName, string methodName)
    {
        if (!ObjectExists(objName))
            return false;

        var obj = GetObject(objName); 

        obj.GetType().GetMethod(methodName)?.Invoke(obj, new object[]{ new List<IRuntimeValue>(), Environment });

        return true;
    }

    public bool CheckRequirementsMet(Script script)
    {
        return CurrentLevel.Requirements.All(req => req.RequirementMet(script));
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