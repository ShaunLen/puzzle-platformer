using PuzzlePlatformer.litescript.Statements;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class AccessRequirement : Requirement
{
    public override sealed string Desc { get; set; }
    public string Object { get; set; }
    
    public AccessRequirement(string desc, string obj)
    {
        Desc = desc;
        Object = obj;
    }
    
    public override bool RequirementMet(Script script)
    {
        throw new System.NotImplementedException();
    }
}