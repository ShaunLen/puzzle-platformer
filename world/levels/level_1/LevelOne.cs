using System.Collections.Generic;
using PuzzlePlatformer.litescript;
using PuzzlePlatformer.world.levels.requirements;

namespace PuzzlePlatformer.world.levels.level_1;

public partial class LevelOne : LevelRoot
{
    override protected void DefineRequirements()
    {
        Requirements = new List<Requirement>
        {
            new NodeRequirement("Use an 'if' statement.", NodeType.IfStatement),
            new ObjectPropertyRequirement("RedButton", "IsPressed")
        };
    }
}