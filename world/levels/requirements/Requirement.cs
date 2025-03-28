using Godot;
using PuzzlePlatformer.litescript_two.Nodes;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;
using HudManager = PuzzlePlatformer.ui.hud.HudManager;

namespace PuzzlePlatformer.world.levels.requirements;

public abstract partial class Requirement : Node
{
    public abstract string Desc { get; set; }
    public abstract bool Required { get; set; }

    public virtual bool RequirementMet(ProgramNode program)
    {
        if (!Required) return false;
        
        if(!CodeManager.Instance.CodeInterfaceOpen)
            HudManager.Instance.SendNotification("Requirement not met - see console for details.", true);
        
        CodeManager.Instance.ConsoleWriteError("Requirement not met - " + Desc);
        
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
            NodeType.WhileStatementNode => "Use a 'while' loop.",
            _ => "AST Node does not have a desc."
        };
    }
}