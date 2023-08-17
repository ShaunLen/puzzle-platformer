using PuzzlePlatformer.litescript.Statements;

namespace PuzzlePlatformer.world.levels.requirements;

public partial class MethodRequirement : Requirement
{
    public override string Desc { get; set; }

    public override bool RequirementMet(Script script)
    {
        throw new System.NotImplementedException();
    }
}