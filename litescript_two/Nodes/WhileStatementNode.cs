using System.Collections.Generic;

namespace PuzzlePlatformer.litescript_two.Nodes;

public class WhileStatementNode(IExpressionNode condition, List<IStatementNode> body, Position position) : IExpressionNode
{
    public IExpressionNode Condition { get; } = condition;
    public List<IStatementNode> Body { get; } = body;
    public Position Position { get; } = position;
    public NodeType Type => NodeType.WhileStatementNode;
}