using System.Linq;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two.Nodes;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class NodeRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    private NodeType _nodeType;
    
    public NodeRequirement(NodeType nodeType)
    {
        Desc = nodeType.ToDesc();
        _nodeType = nodeType;
    }
    
    public override bool RequirementMet(ProgramNode program)
    {
        if (program.Body.Any(stmt => stmt.Type == _nodeType))
            return true;
        
        CodeManager.Instance.ConsoleWriteError("level requirement not met: " + Desc);
        return false;
    }
    
    
}