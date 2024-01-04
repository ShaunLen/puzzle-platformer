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
			// new NodeRequirement(NodeType.IfStatementNode),
			// new ObjectPropertyRequirement("RedButton", "IsPressed"),
			// new ObjectPropertyRequirement("BlueDoor", "Open", true),
			// new NativeFuncRequirement("Print")
		};
	}
}
