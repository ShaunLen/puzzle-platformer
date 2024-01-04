using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Nodes;

public class IfStatementNode(Position position, IExpressionNode condition, List<IStatementNode> consequent, List<IStatementNode> alternate) : IExpressionNode
{
    public IExpressionNode Condition { get; } = condition;
    public List<IStatementNode> Consequent { get; } = consequent;
    public List<IStatementNode> Alternate { get; } = alternate;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.IfStatementNode;
}