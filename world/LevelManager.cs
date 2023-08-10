using System.Collections.Generic;
using System.Linq;
using Godot;
using PuzzlePlatformer.objects.interactable;

namespace PuzzlePlatformer.world;

public partial class LevelManager : Node
{
    public List<Interactable> Objects;

    public override void _Ready()
    {
        // Get list of all interactable objects in level
        var interactables = GetTree().GetNodesInGroup("interactable").ToList();

        foreach (var obj in interactables)
        {
            Objects.Add((Interactable) obj);
        }
    }

    public bool ObjectExists(string objName)
    {
        return Objects.Any(obj => obj.Name.Equals(objName));
    }

    public Interactable GetObject(string objName)
    {
        return Objects.Find(name => Name == objName);
    }

    public bool CallMethod(string objName, string methodName)
    {
        if (!ObjectExists(objName))
            return false;

        var obj = GetObject(objName);

        obj.GetType().GetMethod(methodName)?.Invoke(obj, new []{ "" });

        return true;
    }
}