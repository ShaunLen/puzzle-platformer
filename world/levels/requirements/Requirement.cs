using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript;
using Script = PuzzlePlatformer.litescript.Statements.Script;

namespace PuzzlePlatformer.world.levels.requirements;

public abstract partial class Requirement : Node
{
    public abstract string Desc { get; set; }

    public virtual bool RequirementMet(Script script)
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
            NodeType.IfStatement => "Use an 'if' statement.",
            _ => "AST Node does not have a desc."
        };
    }
}