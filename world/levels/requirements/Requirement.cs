using Godot;
using PuzzlePlatformer.autoloads;
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