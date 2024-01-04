using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.Nodes;

namespace PuzzlePlatformer.world.levels.requirements;

public abstract partial class Requirement : Node
{
    public abstract string Desc { get; set; }

    public virtual bool RequirementMet(ProgramNode program)
    {
        if(!CodeManager.Instance.EditorOpen)
            HudManager.Instance.WriteErrorNotification("level requirement not met - see console for details.");
        
        CodeManager.Instance.ConsoleWriteError("level requirement not met - " + Desc);
        
        return false;
    }
}

internal static class RequirementExtensions
{
    public static string ToDesc(this NodeType nodeType)
    {
        return nodeType switch
        {
            NodeType.IfStatementNode => "Use an 'if' statement.",
            _ => "AST Node does not have a desc."
        };
    }
}