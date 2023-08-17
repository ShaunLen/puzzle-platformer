using System.Linq;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.litescript.Statements;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class NodeRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    public NodeType NodeType;
    
    public NodeRequirement(string desc, NodeType nodeType)
    {
        Desc = desc;
        NodeType = nodeType;
    }
    
    public override bool RequirementMet(Script script)
    {
        if (script.Body.Any(stmt => stmt.Type == NodeType))
            return true;
        
        CodeManager.Instance.ConsoleWriteError("level requirement not met: " + Desc);
        return false;
    }
}