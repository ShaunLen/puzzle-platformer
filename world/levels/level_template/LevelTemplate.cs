using System.Collections.Generic;
using PuzzlePlatformer.world.levels.requirements;

namespace PuzzlePlatformer.world.levels.level_template;

public partial class LevelTemplate : LevelRoot
{
    override protected void DefineRequirements()
    {
        Requirements = new List<Requirement>
        {
            //TODO: Can I export this to the editor? Would mean the level can be fully set up without going into code.
            // Maybe an individual export for each type of requirement (as params are different).
            
            // new NodeRequirement("Use an 'if' statement.", NodeType.IfStatement),
            // new ObjectPropertyRequirement("RedButton", "IsPressed")
        };
    }
}