using System.Collections.Generic;
using PuzzlePlatformer.world.levels.requirements;

namespace PuzzlePlatformer.world.levels.level_template;

public partial class LevelTemplate : LevelRoot
{
	override protected void DefineRequirements()
	{
		Requirements = new List<Requirement>
		{
			// new NodeRequirement("Use an 'if' statement.", NodeType.IfStatement),
			// new ObjectPropertyRequirement("RedButton", "IsPressed"),
			// new ObjectPropertyRequirement("BlueDoor", "Open", true)
		};
	}
}
