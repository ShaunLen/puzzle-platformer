using System.Collections.Generic;
using PuzzlePlatformer.litescript_two.Nodes;
using PuzzlePlatformer.world.levels.requirements;

namespace PuzzlePlatformer.world.levels.level_1;

public partial class LevelOne : LevelRoot
{
	override protected void DefineRequirements()
	{
		Requirements = new List<Requirement>
		{
			// new NodeRequirement(NodeType.WhileStatementNode, true),
			// new ObjectPropertyRequirement("RedButton", "IsPressed", true),
			// new ObjectPropertyRequirement("BlueDoor", "Open", true, true),
			new NativeFuncRequirement("Print", false)
		};
	}
}
